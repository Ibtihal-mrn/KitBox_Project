using Avalonia.Controls;

namespace KitBox_Project
{
    public partial class Size : Window
    {
        public Size()
        {
            InitializeComponent();
        }

        private void GoToMainWindow(object sender, Avalonia.Interactivity.RoutedEventArgs e)
        {
            var mainWindow = new MainWindow();  // Crée une nouvelle instance de MainWindow
            Fonctions.NavigateToPage(this, mainWindow);  // Utilise la méthode dans NavigationHelper
        }

        private void GoToHeightWindow(object sender, Avalonia.Interactivity.RoutedEventArgs e)
        {
            var heightWindow = new Height();  // Crée une nouvelle instance de MainWindow
            Fonctions.NavigateToPage(this, heightWindow);  // Utilise la méthode dans NavigationHelper
        }
    }
}