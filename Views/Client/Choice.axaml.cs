using Avalonia;
using Avalonia.Controls;
using Avalonia.Markup.Xaml;
using Avalonia.Interactivity;
using System;
using KitBox_Project.Services;
using KitBox_Project.Data;
using System.Linq;



namespace KitBox_Project.Views
{
    public partial class Choice : UserControl
    {
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

        private void GoToColor(object sender, RoutedEventArgs e)
        {
            var mainWindow = VisualRoot as MainWindow; // Utilisation de 'as' pour éviter une exception
            if (mainWindow != null) // Vérifie que mainWindow n'est pas null
            {
                mainWindow.MainContent.Content = new Color(); // ✅ Modifie le bon ContentControl
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


        private void GoToFirstPage(object sender, RoutedEventArgs e)
        {
            if (VisualRoot is MainWindow mainWindow)
            {
                mainWindow.ShowChooseUserTypePage(); // ✅ les événements sont rebranchés ici
            }
        }
    }
}
