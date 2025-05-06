using Avalonia.Controls;

namespace KitBox_Project
{
    public partial class Door : Window
    {
        public Door()
        {
            InitializeComponent();
        }

        //Next = Choice
        private void GoToChoice(object sender, Avalonia.Interactivity.RoutedEventArgs e)
        {
            var choiceWindow = new Choice();  // Crée une nouvelle instance de MainWindow
            Fonctions.NavigateToPage(this, choiceWindow);  // Utilise la méthode dans NavigationHelper
        }

        private void GoToMainWindow(object sender, Avalonia.Interactivity.RoutedEventArgs e)
        {
            var mainWindow = new MainWindow();  // Crée une nouvelle instance de MainWindow
            Fonctions.NavigateToPage(this, mainWindow);  // Utilise la méthode dans NavigationHelper
        }

        //Back = Color
        private void GoToColor(object sender, Avalonia.Interactivity.RoutedEventArgs e)
        {
            var colorWindow = new Color();  // Crée une nouvelle instance de MainWindow
            Fonctions.NavigateToPage(this, colorWindow);  // Utilise la méthode dans NavigationHelper
        }

        
        
    }
}