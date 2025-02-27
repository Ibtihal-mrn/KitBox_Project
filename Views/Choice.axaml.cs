using Avalonia;
using Avalonia.Controls;
using Avalonia.Markup.Xaml;
using Avalonia.Interactivity;

namespace KitBox_Project.Views
{
    public partial class Choice : UserControl
    {
        public Choice()
        {
            InitializeComponent();
        }

        // Gestionnaire d'événement pour le bouton "Next"
        

        private void GoToHeight(object sender, RoutedEventArgs e)
        {
            var mainWindow = (MainWindow)VisualRoot; // Récupère la fenêtre principale
            mainWindow.MainContent.Content = new Height(); // ✅ Modifie le bon ContentControl
        }

        private void GoToDoor(object sender, RoutedEventArgs e)
        {
            var mainWindow = (MainWindow)VisualRoot; // Récupère la fenêtre principale
            mainWindow.MainContent.Content = new Door(); // ✅ Modifie le bon ContentControl
        }

        private void GoToSize(object sender, RoutedEventArgs e)
        {
            var mainWindow = (MainWindow)VisualRoot; // Récupère la fenêtre principale
            mainWindow.MainContent.Content = new DesignYourWardrobe(); // ✅ Modifie le bon ContentControl
        }

        private void GoToOrder(object sender, RoutedEventArgs e)
        {
            var mainWindow = (MainWindow)VisualRoot; // Récupère la fenêtre principale
            mainWindow.MainContent.Content = new Order(); // ✅ Modifie le bon ContentControl
        }

    }
}
