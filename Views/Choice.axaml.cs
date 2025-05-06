using Avalonia.Controls;

namespace KitBox_Project
{
    public partial class Choice : Window
    {
        public Choice()
        {
            InitializeComponent();
        }

        //Back = Door
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

        private void GoToHeight(object sender, Avalonia.Interactivity.RoutedEventArgs e)
        {
            var heightWindow = new Height();  // Crée une nouvelle instance de MainWindow
            Fonctions.NavigateToPage(this, heightWindow);  // Utilise la méthode dans NavigationHelper
        }

        private void  GoToSize(object sender, Avalonia.Interactivity.RoutedEventArgs e)
        {
            var sizeWindow = new Size();  // Crée une nouvelle instance de MainWindow
            Fonctions.NavigateToPage(this, sizeWindow);  // Utilise la méthode dans NavigationHelper
        }

        private void  GoToOrder(object sender, Avalonia.Interactivity.RoutedEventArgs e)
        {
            var orderWindow = new Order();  // Crée une nouvelle instance de MainWindow
            Fonctions.NavigateToPage(this, orderWindow);  // Utilise la méthode dans NavigationHelper
        }
        
       
    }
}