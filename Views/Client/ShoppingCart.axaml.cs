using System;
using Avalonia.Controls;
using Avalonia.Interactivity;
using KitBox_Project.ViewModels;
using KitBox_Project.Models;
using KitBox_Project.Views;
using KitBox_Project.Services;
using KitBox_Project.Data;
using System.Linq;

namespace KitBox_Project.Views.Client
{
    public partial class ShoppingCart : UserControl
    {
        public event EventHandler<RoutedEventArgs>? HomeClicked;

        public ShoppingCart()
        {
            InitializeComponent();
            DataContext = new ShoppingCartViewModel();
            Loaded += OnLoaded;
        }

        private void OnLoaded(object? sender, RoutedEventArgs e)
        {
            Console.WriteLine($"DataContext: {DataContext?.GetType().Name ?? "null"}");
            if (DataContext is ShoppingCartViewModel)
            {
                Console.WriteLine("DataContext is correctly set to ShoppingCartViewModel");
            }
            else
            {
                Console.WriteLine("DataContext is incorrect!");
            }
        }

        private void OnHomeButtonClick(object? sender, RoutedEventArgs e)
        {
            HomeClicked?.Invoke(this, e);
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





    }
}