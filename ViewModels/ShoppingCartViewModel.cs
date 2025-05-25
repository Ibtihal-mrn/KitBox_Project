using System;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Windows.Input;
using KitBox_Project.Models;
using MsBox.Avalonia.ViewModels.Commands;
using KitBox_Project.Data;

namespace KitBox_Project.ViewModels
{
    public class ShoppingCartViewModel : INotifyPropertyChanged
    {
        private ObservableCollection<CartItemViewModel> _cartItems = new();
        private string _orderId = string.Empty;
        public string OrderId
        {
            get => _orderId;
            set
            {
                if (_orderId != value)
                {
                    _orderId = value;
                    OnPropertyChanged();
                }
            }
        }
        public ObservableCollection<CartItemViewModel> CartItems
        {
            get => _cartItems;
            set
            {
                _cartItems = value;
                OnPropertyChanged();
                NotifyPriceChange();
            }
        }

        public decimal TotalPrice => CartItems.Sum(item => item.TotalPrice);
        public int TotalItemCount => CartItems.Sum(item => item.Quantity);
        public bool IsCartEmpty => TotalItemCount == 0;

        public ICommand ClearCartCommand { get; }
        public ICommand DecreaseQuantityCommand { get; }
        public ICommand IncreaseQuantityCommand { get; }
        public ICommand RemoveItemCommand { get; }

        public ShoppingCartViewModel()
        {
            LoadCartItems();
            AppState.SelectedArticles.CollectionChanged += (s, e) => LoadCartItems();

            ClearCartCommand = new RelayCommand(_ => ClearCart(), _ => !IsCartEmpty);
            DecreaseQuantityCommand = new RelayCommand(param => DecreaseQuantity(param as CartItemViewModel), param => CanDecrease(param as CartItemViewModel));
            IncreaseQuantityCommand = new RelayCommand(param => IncreaseQuantity(param as CartItemViewModel));
            RemoveItemCommand = new RelayCommand(param => RemoveItem(param as CartItemViewModel));
        }

        private void LoadCartItems()
        {
            var grouped = AppState.SelectedArticles
                .GroupBy(a => new { a.Reference, a.Color, a.Dimensions })
                .Select(g =>
                {
                    var first = g.First();
                    return new CartItemViewModel
                    {
                        Reference = first.Reference,
                        Color = first.Color,
                        Dimensions = first.Dimensions,
                        Length = first.Length,
                        Depth = first.Depth,
                        Height = first.Height,
                        SellingPrice = first.SellingPrice,
                        Quantity = g.Count(),
                        NumberOfPiecesAvailable = first.NumberOfPiecesAvailable
                    };
                }).ToList();

            foreach (var item in CartItems)
                item.QuantityChanged -= OnCartItemQuantityChanged;

            CartItems = new ObservableCollection<CartItemViewModel>(grouped);

            foreach (var item in CartItems)
                item.QuantityChanged += OnCartItemQuantityChanged;

            NotifyPriceChange();
        }

        private void OnCartItemQuantityChanged()
        {
            SyncAppStateFromCartItems();
            NotifyPriceChange();
        }

        private void SyncAppStateFromCartItems()
        {
            AppState.ClearCart();
            foreach (var item in CartItems)
            {
                var article = StaticArticleDatabase.AllArticles.FirstOrDefault(a =>
                    a.Reference == item.Reference &&
                    a.Color == item.Color &&
                    a.Dimensions == item.Dimensions);

                if (article != null)
                {
                    for (int i = 0; i < item.Quantity; i++)
                        AppState.AddToCart(article);
                }
            }
        }

        private void IncreaseQuantity(object? parameter)
        {
            if (parameter is CartItemViewModel item)
            {
                var totalInCart = AppState.SelectedArticles
                    .Count(a => a.Reference == item.Reference && a.Color == item.Color && a.Dimensions == item.Dimensions);

                if (totalInCart >= item.NumberOfPiecesAvailable)
                {
                    Console.WriteLine($"❌ Pas assez de stock pour {item.Reference} ({item.Color}) !");
                    return;
                }

                var firstArticle = StaticArticleDatabase.AllArticles.FirstOrDefault(a =>
                    a.Reference == item.Reference &&
                    a.Color == item.Color &&
                    a.Dimensions == item.Dimensions);

                if (firstArticle != null)
                {
                    AppState.AddToCart(firstArticle);
                    LoadCartItems();
                    NotifyPriceChange();
                }
            }
        }


        private void DecreaseQuantity(CartItemViewModel? item)
        {
            if (item == null || item.Quantity <= 1) return;

            var toRemove = AppState.SelectedArticles.FirstOrDefault(a =>
                a.Reference == item.Reference && a.Color == item.Color && a.Dimensions == item.Dimensions);

            if (toRemove != null)
            {
                AppState.SelectedArticles.Remove(toRemove);
                LoadCartItems();
            }
        }

        private bool CanDecrease(CartItemViewModel? item) => item != null && item.Quantity > 1;

        private void RemoveItem(CartItemViewModel? item)
        {
            if (item == null) return;

            var toRemove = AppState.SelectedArticles
                .Where(a => a.Reference == item.Reference && a.Color == item.Color && a.Dimensions == item.Dimensions)
                .ToList();

            foreach (var article in toRemove)
                AppState.SelectedArticles.Remove(article);

            LoadCartItems();
        }

        private void ClearCart()
        {
            AppState.ClearCart();
            CartItems.Clear();
        }

        private void NotifyPriceChange()
        {
            OnPropertyChanged(nameof(TotalPrice));
            OnPropertyChanged(nameof(TotalItemCount));
            OnPropertyChanged(nameof(IsCartEmpty));
        }

        public event PropertyChangedEventHandler? PropertyChanged;
        protected void OnPropertyChanged([CallerMemberName] string? name = null)
            => PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(name));
    }
}
