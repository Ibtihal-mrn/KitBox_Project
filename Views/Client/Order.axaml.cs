using Avalonia;
using Avalonia.Controls;
using Avalonia.Markup.Xaml;
using Avalonia.Interactivity;
using KitBox_Project.ViewModels;
using System;
using KitBox_Project.Services;
using KitBox_Project.Data;
using System.Linq;

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
