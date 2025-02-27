using Avalonia;
using Avalonia.Controls;
using Avalonia.Markup.Xaml;
using Avalonia.Interactivity;

namespace KitBox_Project.Views
{
    public partial class Height : UserControl
    {
        public Height()
        {
            InitializeComponent();
        }

        // Gestionnaire d'événement pour le bouton "Next"
        

        private void GoToColor(object sender, RoutedEventArgs e)
        {
            var mainWindow = (MainWindow)VisualRoot; // Récupère la fenêtre principale
            mainWindow.MainContent.Content = new Color(); // ✅ Modifie le bon ContentControl
        }

        private void GoToSize(object sender, RoutedEventArgs e)
        {
            var mainWindow = (MainWindow)VisualRoot; // Récupère la fenêtre principale
            mainWindow.MainContent.Content = new DesignYourWardrobe(); // ✅ Modifie le bon ContentControl
        }

    }
}
