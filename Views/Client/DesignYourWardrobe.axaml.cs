using Avalonia;
using Avalonia.Controls;
using Avalonia.Markup.Xaml;
using Avalonia.Interactivity;

namespace KitBox_Project.Views
{
    public partial class DesignYourWardrobe : UserControl
    {
        public DesignYourWardrobe()
        {
            InitializeComponent();
        }

        // Gestionnaire d'événement pour le bouton "Next"
        
        private void GoToHeightWindow(object sender, RoutedEventArgs e)
        {
            var mainWindow = VisualRoot as MainWindow; // Utilisation de 'as' pour éviter un cast direct
            if (mainWindow != null)
            {
                mainWindow.MainContent.Content = new Height(); // ✅ Modifie le bon ContentControl
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
