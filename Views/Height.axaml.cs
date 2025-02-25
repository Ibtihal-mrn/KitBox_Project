using Avalonia.Controls;

namespace KitBox_Project
{
    public partial class Height : Window
    {
        public Height()
        {
            InitializeComponent();
        }

        private void GoToMainWindow(object sender, Avalonia.Interactivity.RoutedEventArgs e)
        {
            var mainWindow = new MainWindow();  // Crée une nouvelle instance de MainWindow
            Fonctions.NavigateToPage(this, mainWindow);  // Utilise la méthode dans NavigationHelper
        }

        private void GoToSize(object sender, Avalonia.Interactivity.RoutedEventArgs e)
        {
            var sizeWindow = new Size();  // Crée une nouvelle instance de MainWindow
            Fonctions.NavigateToPage(this, sizeWindow);  // Utilise la méthode dans NavigationHelper
        }

        private void GoToColor(object sender, Avalonia.Interactivity.RoutedEventArgs e)
        {
            var colorWindow = new Color();  // Crée une nouvelle instance de MainWindow
            Fonctions.NavigateToPage(this, colorWindow);  // Utilise la méthode dans NavigationHelper
        }
        
    }
}
