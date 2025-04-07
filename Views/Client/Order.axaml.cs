using Avalonia;
using Avalonia.Controls;
using Avalonia.Markup.Xaml;
using Avalonia.Interactivity;

namespace KitBox_Project.Views
{
    public partial class Order : UserControl
    {
        public Order()
        {
            InitializeComponent();
        }

        // Gestionnaire d'événement pour le bouton "Next"
        
        private void GoToChoice(object sender, RoutedEventArgs e)
        {
            var mainWindow = VisualRoot as MainWindow; // Utilisation de 'as' pour éviter un cast direct
            if (mainWindow != null)
            {
                mainWindow.MainContent.Content = new Choice(); // ✅ Modifie le bon ContentControl
            }
        }
    }
}
