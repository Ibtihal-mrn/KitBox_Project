using Avalonia.Threading;
using ReactiveUI;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using KitBox_Project.Models;
using KitBox_Project.Services;

namespace KitBox_Project.ViewModels
{
    public class SellerViewModel : ReactiveObject
    {
        // Collection liée à la ListBox / ItemsControl
        public ObservableCollection<ConfirmedOrder> Orders { get; } = new();

        private DispatcherTimer _timer;

        public SellerViewModel()
        {
            // Chargement initial
            RefreshOrders();

            // Timer de 20s pour rafraîchir
            _timer = new DispatcherTimer
            {
                Interval = TimeSpan.FromSeconds(20)
            };
            _timer.Tick += (_, __) => RefreshOrders();
            _timer.Start();
        }

        private void RefreshOrders()
        {
            Orders.Clear();
            foreach (var order in ConfirmedOrderService.LoadConfirmedOrders())
            {
                // Regroupement des articles par référence et couleur
                var groupedArticles = GroupArticlesByReferenceAndColor(order.Articles);
                
                // Créer un nouveau ConfirmedOrder avec les articles regroupés
                var groupedOrder = new ConfirmedOrder(order.OrderId)
                {
                    Articles = groupedArticles.ToList()
                };

                Orders.Add(groupedOrder);
            }
        }

        // Méthode pour regrouper les articles par référence et couleur
        private ObservableCollection<Article> GroupArticlesByReferenceAndColor(IEnumerable<Article> articles)
        {
            var grouped = articles
                .GroupBy(a => new { a.Reference, a.Color, a.Code })
                .Select(g => new Article
                {
                    Reference = g.Key.Reference,
                    Color = g.Key.Color,
                    Quantity = g.Sum(a => a.Quantity),
                    Code = g.Key.Code,
                })
                .ToList();

            return new ObservableCollection<Article>(grouped);
        }
    }
}
