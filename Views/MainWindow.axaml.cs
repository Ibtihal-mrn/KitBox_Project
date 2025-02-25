using Avalonia.Controls;

namespace KitBox_Project
{
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            InitializeComponent();
        }

        private void GoToSize(object sender, Avalonia.Interactivity.RoutedEventArgs e)
        {
            var sizeWindow = new Size();  // Crée la fenêtre Size
            Fonctions.NavigateToPage(this, sizeWindow);  // Utilise la méthode dans NavigationHelper
        }
    }
}
