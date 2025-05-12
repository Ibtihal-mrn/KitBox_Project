using Avalonia;
using Avalonia.Controls;
using Avalonia.Markup.Xaml;
using Avalonia.Interactivity;
using KitBox_Project.Services;

namespace KitBox_Project.Views
{
    public partial class Confirmation : UserControl
    {
        public Confirmation()
        {
            InitializeComponent();
        }

        // Gestionnaire d'événements pour les boutons


        private void GoToFirstPage(object sender, RoutedEventArgs e)
        {
            // 🔄 Recharge le stock selon les commandes passées
            StockService.LoadConfirmedOrdersAndAdjustStock();

            if (VisualRoot is MainWindow mainWindow)
            {
                mainWindow.ShowChooseUserTypePage(); // ✅ Les événements sont rebranchés ici
            }
        }

    }

    
}
