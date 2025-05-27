using Avalonia;
using Avalonia.Controls;
using Avalonia.Markup.Xaml;
using Avalonia.Interactivity;
using System;
using KitBox_Project.Services;
using KitBox_Project.Data;
using System.Linq;
using KitBox_Project.Models;
using KitBox_Project.ViewModels;
using System.Collections.ObjectModel;
using KitBox_Project.Views.Client;

namespace KitBox_Project.Views
{
    public partial class Choice : UserControl
    {
        // Event triggered when the color selection process starts
        public event EventHandler<RoutedEventArgs>? StartClicked;

        public Choice()
        {
            InitializeComponent();
        }


        private void GoToDoor(object sender, RoutedEventArgs e)
        {
            var mainWindow = VisualRoot as MainWindow;
            if (mainWindow != null)
            {
                mainWindow.MainContent.Content = new Door();
            }
        }

        private async void GoToColor(object sender, RoutedEventArgs e)
        {
            var mainWindow = VisualRoot as MainWindow;
            if (mainWindow == null) return;

            bool isStacking = (sender is Button button && button.Content?.ToString() == "Empiler un casier");

            // Validation pour "Empiler un casier"
            if (isStacking && (AppState.SelectedLength == 0 || AppState.SelectedDepth == 0 || AppState.SelectedHeight == 0))
            {
                var errorMessage = this.FindControl<TextBlock>("ErrorMessage");
                if (errorMessage != null)
                {
                    errorMessage.Text = "Configurez un casier avant d'empiler.";
                    errorMessage.IsVisible = true;
                }
                Console.WriteLine("Erreur : Configurez un casier avant d'empiler.");
                return;
            }

            try
            {
                await StockService.InitializeStockAsync();
                StartClicked?.Invoke(this, new RoutedEventArgs());
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
            }

            mainWindow.MainContent.Content = new Color(fromChoice: true, isStacking: isStacking);
        }

        private void GoToOrder(object sender, RoutedEventArgs e)
        {
            var mainWindow = VisualRoot as MainWindow;
            if (mainWindow != null)
            {
                mainWindow.MainContent.Content = new Order();
            }
        }

        private void GoToConfirmation(object sender, RoutedEventArgs e)
        {
            var mainWindow = VisualRoot as MainWindow;
            if (mainWindow == null) return;

            // 1. Mise à jour du stock
            StockService.UpdateStock(AppState.SelectedArticles);
            Console.WriteLine("✅ Stock mis à jour avec succès.");

            // 2. Génération d'un nouvel ID de commande
            string orderId = ConfirmedOrderService.GenerateOrderId();

            // 3. Regroupement des articles DU PANIER + on copie le SellingPrice !
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

            // 4. Création de l'objet ConfirmedOrder avec les articles regroupés
            var confirmedOrder = new ConfirmedOrder(orderId)
            {
                Articles = groupedArticles
            };

            // 5. Sauvegarde de la commande dans le JSON
            ConfirmedOrderService.SaveConfirmedOrder(confirmedOrder);
            Console.WriteLine($"🗂 Commande {orderId} sauvegardée avec {confirmedOrder.Articles.Count} article(s).");
            // 5bis. On vide les ajustements manuels (inventory_current.json)
            //InventoryModificationService.SnapshotCurrent();

            // 6. Log détaillé
            foreach (var article in confirmedOrder.Articles)
            {
                var stockArticle = StaticArticleDatabase.AllArticles
                    .FirstOrDefault(a => a.Code == article.Code);
                Console.WriteLine($"🧾 {article.Reference} ({article.Color})");
                Console.WriteLine($"    ➖ Quantité déduite : {article.Quantity}");
                Console.WriteLine($"    📦 Stock restant   : {stockArticle?.NumberOfPiecesAvailable}");
                Console.WriteLine($"    💶 Prix unitaire  : {article.SellingPrice:0.00} €");
                Console.WriteLine($"    🔢 Sous-total     : {article.TotalPrice:0.00} €");
            }

            // 7. Vidage du panier pour la prochaine commande
            AppState.ClearCart();
            Console.WriteLine("🧹 Panier vidé pour la prochaine commande.");

            // 8. Navigation vers la page de confirmation
            mainWindow.MainContent.Content = new Confirmation();
        }

        private async void GoToFirstPage(object sender, RoutedEventArgs e)
        {
            try
            {
                await StockService.ForceReloadStockAsync();
                if (VisualRoot is MainWindow mainWindow)
                    mainWindow.ShowChooseUserTypePage();
            }
            catch (Exception ex)
            {
                Console.WriteLine($"❌ Erreur pendant la confirmation : {ex.Message}");
            }
        }
    }
}