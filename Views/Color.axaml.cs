using Avalonia;
using Avalonia.Controls;
using Avalonia.Markup.Xaml;
using Avalonia.Interactivity;

namespace KitBox_Project.Views
{
    public partial class Color : UserControl
    {
        public Color()
        {
            InitializeComponent();
        }

        // Gestionnaire d'événement pour le bouton "Next"
        
        private void GoToHeight(object sender, RoutedEventArgs e)
        {
            var mainWindow = VisualRoot as MainWindow; // Utilisation de 'as' pour éviter un cast direct
            if (mainWindow != null)
            {
                mainWindow.MainContent.Content = new Height(); // ✅ Modifie le bon ContentControl
            }
        }

        private void GoToDoor(object sender, RoutedEventArgs e)
        {
            var mainWindow = VisualRoot as MainWindow; // Utilisation de 'as' pour éviter un cast direct
            if (mainWindow != null)
            {
                mainWindow.MainContent.Content = new Door(); // ✅ Modifie le bon ContentControl
            }
        }
    }
}
