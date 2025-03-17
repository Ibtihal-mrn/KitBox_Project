using Avalonia;
using Avalonia.Controls;
using Avalonia.Markup.Xaml;
using Avalonia.Interactivity;
using KitBox_Project.Data;
using System.Linq;
using System;

namespace KitBox_Project.Views
{
    public partial class Height : UserControl
    {
        public int SelectedLength { get; set; }
        public int SelectedDepth { get; set; }

        public Height()
        {
            InitializeComponent();
            Loaded += OnControlLoaded;
        }

        private void OnControlLoaded(object? sender, RoutedEventArgs e)
        {
            LoadHeightData();
        }

        private void LoadHeightData()
        {
            var hauteurComboBox = this.FindControl<ComboBox>("Hauteur");
            var stockWarningText = this.FindControl<TextBlock>("StockWarning");

            if (hauteurComboBox == null || stockWarningText == null)
            {
                Console.WriteLine("Impossible de trouver les éléments nécessaires");
                return;
            }

            var dataAccess = new DataAccess();
            var panels = dataAccess.GetHeightOfPanel(SelectedLength, SelectedDepth);
            var uniqueHeights = panels.Select(panel => panel.Height).Distinct().OrderBy(height => height).ToList();

            hauteurComboBox.ItemsSource = null;  // Réinitialiser pour éviter les erreurs
            hauteurComboBox.ItemsSource = uniqueHeights.Select(h => h.ToString()).ToList(); // Utilisation correcte

            // Vérifier si le stock est faible (inférieur à 5)
            bool isLowStock = dataAccess.IsStockLow(SelectedLength, SelectedDepth);
            stockWarningText.Text = isLowStock ? "⚠️ Attention : Stock limité (moins de 5 pièces disponibles) !" : "";
            stockWarningText.IsVisible = isLowStock;
        }


        private void GoToColor(object sender, RoutedEventArgs e)
        {
            var mainWindow = VisualRoot as MainWindow;
            if (mainWindow != null)
            {
                mainWindow.MainContent.Content = new Color();
            }
        }

        private void GoToSize(object sender, RoutedEventArgs e)
        {
            var mainWindow = VisualRoot as MainWindow;
            if (mainWindow != null)
            {
                mainWindow.MainContent.Content = new DesignYourWardrobe();
            }
        }
    }
}
