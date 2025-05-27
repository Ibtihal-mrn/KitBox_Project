using Avalonia.Threading;
using ReactiveUI;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.IO;
using System.Linq;
using System.Reactive;
using System.Reactive.Linq;
using System.Text.Json;
using KitBox_Project.Models;
using KitBox_Project.Services;

namespace KitBox_Project.ViewModels
{
    public class SellerViewModel : ReactiveObject, IDisposable
    {
        public ObservableCollection<ConfirmedOrder> FilteredOrders { get; } = new();

        private string _searchText = string.Empty;
        public string SearchText
        {
            get => _searchText;
            set => this.RaiseAndSetIfChanged(ref _searchText, value);
        }

        public ReactiveCommand<Unit, Unit> ClearSearchCommand { get; }
        public ReactiveCommand<ConfirmedOrder, Unit> PayOrderCommand { get; }

        private readonly DispatcherTimer _timer;
        private List<ConfirmedOrder> _allOrders = new();
        private Dictionary<string, DateTime> _paidStatus = new();
        private const string PaidStatusFile = "paid_status.json";

        public SellerViewModel()
        {
            Console.WriteLine("SellerViewModel ctor");

            ClearSearchCommand = ReactiveCommand.Create(() =>
            {
                SearchText = string.Empty;
                return Unit.Default;
            });

            // Créer la commande avec gestion d'erreur
            PayOrderCommand = ReactiveCommand.Create<ConfirmedOrder>(PayOrder);

            // Souscrire aux erreurs pour les déboguer
            PayOrderCommand.ThrownExceptions.Subscribe(ex =>
            {
                Console.WriteLine($"Erreur dans PayOrderCommand: {ex.Message}");
                Console.WriteLine($"StackTrace: {ex.StackTrace}");
            });

            LoadPaidStatus();

            _timer = new DispatcherTimer { Interval = TimeSpan.FromSeconds(20) };
            _timer.Tick += (_, __) => RefreshOrders();
            _timer.Start();

            this.WhenAnyValue(x => x.SearchText)
                .Throttle(TimeSpan.FromMilliseconds(300))
                .ObserveOn(RxApp.MainThreadScheduler)
                .Subscribe(_ => ApplyFilter());

            RefreshOrders();
        }

        private void PayOrder(ConfirmedOrder order)
        {
            try
            {
                Console.WriteLine($"PayOrder appelé pour: {order?.OrderId}");

                if (order == null)
                {
                    Console.WriteLine("Order est null!");
                    return;
                }

                // Tout faire sur le thread UI
                Dispatcher.UIThread.InvokeAsync(() =>
                {
                    try
                    {
                        order.PaidDate = DateTime.Now;
                        _paidStatus[order.OrderId] = order.PaidDate.Value;
                        SavePaidStatus();

                        Console.WriteLine($"Order {order.OrderId} marqué comme payé");

                        // Mettre à jour l'UI
                        this.RaisePropertyChanged(nameof(FilteredOrders));
                    }
                    catch (Exception innerEx)
                    {
                        Console.WriteLine($"Erreur interne dans PayOrder: {innerEx.Message}");
                    }
                });
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Erreur dans PayOrder: {ex.Message}");
                Console.WriteLine($"StackTrace: {ex.StackTrace}");
            }
        }

        private void RefreshOrders()
        {
            var rawOrders = ConfirmedOrderService.LoadAllOrders();

            var orderGroups = rawOrders
                .GroupBy(o => o.OrderId, StringComparer.OrdinalIgnoreCase)
                .ToList();

            _allOrders = orderGroups
                .Select(orderGroup =>
                {
                    var baseOrder = orderGroup.First();
                    var allArticlesForOrder = orderGroup
                        .SelectMany(o => o.Articles ?? Enumerable.Empty<Article>())
                        .ToList();

                    var groupedArticles = GroupArticlesByReferenceAndColor(allArticlesForOrder);

                    var order = new ConfirmedOrder(baseOrder.OrderId)
                    {
                        Articles = groupedArticles.ToList(),
                        // Assigner la commande à chaque order
                        PayCommand = PayOrderCommand
                    };

                    if (_paidStatus.TryGetValue(order.OrderId, out var paidDate))
                    {
                        order.PaidDate = paidDate;
                    }

                    return order;
                })
                .ToList();

            ApplyFilter();
        }

        private void ApplyFilter()
        {
            var term = SearchText?.Trim() ?? string.Empty;

            IEnumerable<ConfirmedOrder> filtered = string.IsNullOrEmpty(term)
                ? _allOrders
                : _allOrders.Where(o =>
                      o.OrderId.IndexOf(term, StringComparison.OrdinalIgnoreCase) >= 0);

            Dispatcher.UIThread.InvokeAsync(() =>
            {
                FilteredOrders.Clear();
                foreach (var order in filtered)
                {
                    FilteredOrders.Add(order);
                }
            });
        }

        private void LoadPaidStatus()
        {
            if (File.Exists(PaidStatusFile))
            {
                var json = File.ReadAllText(PaidStatusFile);
                _paidStatus = JsonSerializer.Deserialize<Dictionary<string, DateTime>>(json)
                             ?? new Dictionary<string, DateTime>();
            }
        }

        private void SavePaidStatus()
        {
            var json = JsonSerializer.Serialize(_paidStatus);
            File.WriteAllText(PaidStatusFile, json);
        }

        private ObservableCollection<Article> GroupArticlesByReferenceAndColor(IEnumerable<Article> articles)
        {
            if (articles == null)
                return new ObservableCollection<Article>();

            var grouped = articles
                .Where(a => a != null)
                .GroupBy(a => new
                {
                    Reference = a.Reference ?? string.Empty,
                    Color = a.Color ?? string.Empty,
                    Code = a.Code ?? string.Empty,
                    SellingPrice = a.SellingPrice
                })
                .Select(g => new Article
                {
                    Reference = g.Key.Reference,
                    Color = g.Key.Color,
                    Code = g.Key.Code,
                    SellingPrice = g.Key.SellingPrice,
                    Quantity = g.Sum(a => a.Quantity)
                })
                .OrderBy(a => a.Reference)
                .ThenBy(a => a.Color)
                .ToList();

            return new ObservableCollection<Article>(grouped);
        }

        public void Dispose()
        {
            _timer?.Stop();
        }
        public void MarkOrderAsPaid(ConfirmedOrder order)
        {
            if (order == null) return;
            
            if (Dispatcher.UIThread.CheckAccess())
            {
                order.PaidDate = DateTime.Now;
                _paidStatus[order.OrderId] = order.PaidDate.Value;
                SavePaidStatus();
                
                Console.WriteLine($"Order {order.OrderId} marqué comme payé");
            }
            else
            {
                Dispatcher.UIThread.InvokeAsync(() => MarkOrderAsPaid(order));
            }
        }
    }
}