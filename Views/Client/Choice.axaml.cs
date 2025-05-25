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

        // Gestionnaire d'événements pour les boutons

        private void GoToHeight(object sender, RoutedEventArgs e)
        {
            var mainWindow = VisualRoot as MainWindow; // Utilisation de 'as' pour éviter une exception
            if (mainWindow != null) // Vérifie que mainWindow n'est pas null
            {
                mainWindow.MainContent.Content = new Height(); // ✅ Modifie le bon ContentControl
            }
        }

        private void GoToDoor(object sender, RoutedEventArgs e)
        {
            var mainWindow = VisualRoot as MainWindow; // Utilisation de 'as' pour éviter une exception
            if (mainWindow != null) // Vérifie que mainWindow n'est pas null
            {
                mainWindow.MainContent.Content = new Door(); // ✅ Modifie le bon ContentControl
            }
        }

        private async void GoToColor(object sender, RoutedEventArgs e)
        {
            try
            {
                //StockService.ResetInitializationFlag();
                // Ne pas vider le panier
                //StaticArticleDatabase.AllArticles.Clear();
                await StockService.InitializeStockAsync();
                StartClicked?.Invoke(this, new RoutedEventArgs());
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
            }
            var mainWindow = VisualRoot as MainWindow;
            if (mainWindow != null)
            {
                mainWindow.MainContent.Content = new Color(true);
            }
        }

        private void GoToOrder(object sender, RoutedEventArgs e)
        {
            var mainWindow = VisualRoot as MainWindow; // Utilisation de 'as' pour éviter une exception
            if (mainWindow != null) // Vérifie que mainWindow n'est pas null
            {
                mainWindow.MainContent.Content = new Order(); // ✅ Modifie le bon ContentControl
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
                        SellingPrice   = first.SellingPrice,    // ← on récupère enfin le vrai prix unitaire
                        Quantity       = g.Count(),
                        // si tu veux, ton modèle calcule déjà TotalPrice = SellingPrice * Quantity
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
                Console.WriteLine($"    💶 Prix unitaire  : {article.SellingPrice:0.00} €");       // <— vérification avant vidage
                Console.WriteLine($"    🔢 Sous-total     : {article.TotalPrice:0.00} €");      // <— idem
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
                // 🔄 Force le rechargement complet du stock : BDD + commandes + UI
                await StockService.ForceReloadStockAsync();

                // Puis repasse à la page de choix
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