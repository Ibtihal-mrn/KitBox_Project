using Avalonia;
using Avalonia.Controls;
using Avalonia.Markup.Xaml;
using Avalonia.Interactivity;

namespace KitBox_Project.Views
{
    public partial class Identification : UserControl
    {
        public Identification()
        {
            InitializeComponent();
        }

        // Gestionnaire d'événements pour les boutons

        private void GoToFirstPage(object sender, RoutedEventArgs e)
        {
            if (VisualRoot is MainWindow mainWindow)
            {
                mainWindow.ShowChooseUserTypePage(); // ✅ les événements sont rebranchés ici
            }
        }

    
        private void GoToHomeVendeur(object sender, RoutedEventArgs e)
        {
            if (VisualRoot is MainWindow mainWindow) // Vérifie si VisualRoot est une MainWindow
            {
                mainWindow.MainContent.Content = new Home_vendeur(); // ✅ Modifie le bon ContentControl
            }
        }
        

        //private void GoToFirstPage(object? sender, RoutedEventArgs e) => MainContent.Content = new ChooseUserTypePage();

        
    }
}
