using Avalonia.Controls;

namespace KitBox_Project
{
    public partial class Order : Window
    {
        public Order()
        {
            InitializeComponent();
        }

        private void GoToMainWindow(object sender, Avalonia.Interactivity.RoutedEventArgs e)
        {
            var mainWindow = new MainWindow();  // Crée une nouvelle instance de MainWindow
            Fonctions.NavigateToPage(this, mainWindow);  // Utilise la méthode dans NavigationHelper
        }

        //Back = Choice
        private void GoToChoice(object sender, Avalonia.Interactivity.RoutedEventArgs e)
        {
            var choiceWindow = new Choice();  // Crée une nouvelle instance de MainWindow
            Fonctions.NavigateToPage(this, choiceWindow);  // Utilise la méthode dans NavigationHelper
        }

        
        
    }
}