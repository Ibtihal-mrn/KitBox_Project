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
using KitBox_Project.Views.Client;

namespace KitBox_Project.Views
{
    public partial class Order : UserControl
    {
        public Order()
        {
            InitializeComponent();

            // We use the same ViewModel as for the basket
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

            // 1.Stock update
            StockService.UpdateStock(AppState.SelectedArticles);
            Console.WriteLine("Stock mis à jour avec succès.");

            // 2. Generating a new order ID
            string orderId = ConfirmedOrderService.GenerateOrderId();

            // 3. Grouping articles from the basket + copy the SellingPrice !
            var groupedArticles = AppState.SelectedArticles
                .GroupBy(a => new { a.Reference, a.Color, a.Code, a.Dimensions, a.SellingPrice })
                .Select(g =>
                {
                    var first = g.First();
                    return new Article
                    {
                        Code           = first.Code,
                        Reference      = first.Reference,
                        Color          = first.Color,
                        Dimensions     = first.Dimensions,
                        Length         = first.Length,
                        Depth          = first.Depth,
                        Height         = first.Height,
                        SellingPrice   = first.SellingPrice,    
                        Quantity       = g.Count(),
                        NumberOfPiecesAvailable = first.NumberOfPiecesAvailable
                    };
                })
                .ToList();

            // 4. Create ConfirmedOrder with grouped articles
            var confirmedOrder = new ConfirmedOrder(orderId)
            {
                Articles = groupedArticles
            };

            // 5.Save order in the JSON folder
            ConfirmedOrderService.SaveConfirmedOrder(confirmedOrder);
            Console.WriteLine($"🗂 Commande {orderId} sauvegardée avec {confirmedOrder.Articles.Count} article(s).");


            // 6. Detailed log
            foreach (var article in confirmedOrder.Articles)
            {
                var stockArticle = StaticArticleDatabase.AllArticles
                    .FirstOrDefault(a => a.Code == article.Code);
                Console.WriteLine($"{article.Reference} ({article.Color})");
                Console.WriteLine($"Quantité déduite : {article.Quantity}");
                Console.WriteLine($"Stock restant   : {stockArticle?.NumberOfPiecesAvailable}");
                Console.WriteLine($"Prix unitaire  : {article.SellingPrice:0.00} €");       
                Console.WriteLine($"Sous-total     : {article.TotalPrice:0.00} €");     
            }

            // 7. Empty basket for next order
            AppState.ClearCart();
            Console.WriteLine("Panier vidé pour la prochaine commande.");

            // 8. Go to confirmation page
            mainWindow.MainContent.Content = new Confirmation();
        }




}}