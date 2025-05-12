using Avalonia;
using Avalonia.Controls;
using Avalonia.Markup.Xaml;
using Avalonia.Interactivity;
using KitBox_Project.ViewModels;
using System;
using KitBox_Project.Services;
using KitBox_Project.Data;
using System.Linq;
using KitBox_Project.Models;

namespace KitBox_Project.Views
{
    public partial class Order : UserControl
    {
        public Order()
        {
            InitializeComponent();

            // ✅ On utilise directement le même ViewModel que pour le panier
            DataContext = new ShoppingCartViewModel();
        }

        private void GoToChoice(object sender, RoutedEventArgs e)
        {
            if (VisualRoot is MainWindow mainWindow)
            {
                mainWindow.MainContent.Content = new Choice();
            }
        }

        private void GoToFirstPage(object sender, RoutedEventArgs e)
        {
            if (VisualRoot is MainWindow mainWindow)
            {
                mainWindow.ShowChooseUserTypePage();
            }
        }
        private void GoToConfirmation(object sender, RoutedEventArgs e)
        {
            var mainWindow = VisualRoot as MainWindow;
            if (mainWindow == null) return;

            // 1. Mise à jour du stock
            bool success = StockService.UpdateStock(AppState.SelectedArticles);
            if (!success)
            {
                Console.WriteLine("❌ Erreur lors de la mise à jour du stock.");
                return;
            }
            Console.WriteLine("✅ Stock mis à jour avec succès.");

            // 2. Génération d'un nouvel ID de commande
            string orderId = ConfirmedOrderService.GenerateOrderId();

            // 3. Regroupement des articles du panier actuel (évite doublons)
            var groupedArticles = AppState.SelectedArticles
                .GroupBy(a => new { a.Reference, a.Color, a.Code })
                .Select(g => new Article
                {
                    Code      = g.Key.Code,
                    Reference = g.Key.Reference,
                    Color     = g.Key.Color,
                    Quantity  = g.Sum(a => a.Quantity)
                })
                .ToList();

            // 4. Création de l'objet ConfirmedOrder avec les articles regroupés
            var confirmedOrder = new ConfirmedOrder(orderId)
            {
                Articles = groupedArticles
            };

            // 5. Sauvegarde de la commande dans le JSON
            ConfirmedOrderService.SaveConfirmedOrder(confirmedOrder);
            Console.WriteLine($"🗂 Commande {orderId} sauvegardée avec {confirmedOrder.Articles.Count} article(s).");

            // 6. Log détaillé
            foreach (var article in confirmedOrder.Articles)
            {
                var stockArticle = StaticArticleDatabase.AllArticles
                    .FirstOrDefault(a => a.Code == article.Code);
                Console.WriteLine($"🧾 {article.Reference} ({article.Color})");
                Console.WriteLine($"    ➖ Quantité déduite : {article.Quantity}");
                Console.WriteLine($"    📦 Stock restant   : {stockArticle?.NumberOfPiecesAvailable}");
            }

            // 7. Vidage du panier pour la prochaine commande
            AppState.ClearCart();
            Console.WriteLine("🧹 Panier vidé pour la prochaine commande.");

            // 8. Navigation vers la page de confirmation
            mainWindow.MainContent.Content = new Confirmation();
        }


}}