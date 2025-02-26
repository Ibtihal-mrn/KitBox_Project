using Avalonia.Controls;

namespace KitBox_Project
{
    public partial class Color : Window
    {
        public Color()
        {
            InitializeComponent();
        }

        //Next = Door
        private void GoToDoor(object sender, Avalonia.Interactivity.RoutedEventArgs e)
        {
            var DoorWindow = new Door();  // Crée une nouvelle instance de MainWindow
            Fonctions.NavigateToPage(this, DoorWindow);  // Utilise la méthode dans NavigationHelper
        }

        private void GoToMainWindow(object sender, Avalonia.Interactivity.RoutedEventArgs e)
        {
            var mainWindow = new MainWindow();  // Crée une nouvelle instance de MainWindow
            Fonctions.NavigateToPage(this, mainWindow);  // Utilise la méthode dans NavigationHelper
        }

        //Back = Height
        private void GoToHeight(object sender, Avalonia.Interactivity.RoutedEventArgs e)
        {
            var heightWindow = new Height();  // Crée une nouvelle instance de MainWindow
            Fonctions.NavigateToPage(this, heightWindow);  // Utilise la méthode dans NavigationHelper
        }
        
    }
}