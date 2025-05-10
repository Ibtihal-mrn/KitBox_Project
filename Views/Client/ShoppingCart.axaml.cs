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

            if (mainWindow != null)
            {
                // 🛠 Mise à jour du stock
                bool success = StockService.UpdateStock(AppState.SelectedArticles);

                if (success)
                {
                    Console.WriteLine("✅ Stock mis à jour avec succès pour les articles commandés.");

                    foreach (var article in AppState.SelectedArticles)
                    {
                        var stockArticle = StaticArticleDatabase.AllArticles
                            .FirstOrDefault(a => a.Code == article.Code);

                        if (stockArticle != null)
                        {
                            Console.WriteLine($"🧾 Article : {article.Reference} ({article.Color})");
                            Console.WriteLine($"    ➖ Quantité déduite : {article.Quantity}");
                            Console.WriteLine($"    📦 Stock restant   : {stockArticle.NumberOfPiecesAvailable}");
                        }
                    }
                }
                else
                {
                    Console.WriteLine("❌ Une erreur est survenue lors de la mise à jour du stock.");
                }

                // ✅ Navigation vers la page de confirmation
                mainWindow.MainContent.Content = new Confirmation();
            }
        }


    }
}