using Avalonia;
using Avalonia.Controls;
using Avalonia.Markup.Xaml;
using Avalonia.Interactivity;
using KitBox_Project.Data;
using System.Linq;
using System;

namespace KitBox_Project.Views
{
    public partial class DesignYourWardrobe : UserControl
    {
        // Propriétés pour stocker la longueur et la profondeur sélectionnées
        public int SelectedLength { get; private set; }
        public int SelectedDepth { get; private set; }

        public DesignYourWardrobe()
        {
            InitializeComponent();
            Loaded += OnControlLoaded;
        }

        private void OnControlLoaded(object? sender, RoutedEventArgs e)
        {
            LoadLengthData();
        }

        private void LoadLengthData()
        {
            var longueurComboBox = this.Find<ComboBox>("Longueur");
            if (longueurComboBox == null)
            {
                Console.WriteLine("Impossible de trouver le ComboBox 'Longueur'");
                return;
            }

            var dataAccess = new DataAccess();
            var uniqueLengths = dataAccess.GetLengthOfPanelHorizontal()
                .Select(panel => panel.Length)
                .Distinct()
                .OrderBy(length => length)
                .ToList();

            longueurComboBox.Items.Clear();
            foreach (var length in uniqueLengths)
            {
                longueurComboBox.Items.Add(new ComboBoxItem { Content = length.ToString() });
            }

            Console.WriteLine($"Chargé {uniqueLengths.Count} longueurs uniques dans le ComboBox");
            longueurComboBox.SelectionChanged += OnLengthSelectionChanged;
        }

        private void OnLengthSelectionChanged(object? sender, SelectionChangedEventArgs e)
        {
            var longueurComboBox = sender as ComboBox;
            if (longueurComboBox == null || longueurComboBox.SelectedItem == null)
                return;

            var selectedItem = longueurComboBox.SelectedItem as ComboBoxItem;
            if (selectedItem?.Content == null)
            {
                Console.WriteLine("La longueur sélectionnée n'est pas valide.");
                return;
            }
            string selectedLengthStr = selectedItem.Content?.ToString() ?? string.Empty;
            if (int.TryParse(selectedLengthStr, out int selectedLength))
            {
                SelectedLength = selectedLength;
                LoadDepthData(selectedLength);
            }
            else
            {
                Console.WriteLine("La longueur sélectionnée n'est pas valide.");
            }
        }

        private void LoadDepthData(int selectedLength)
        {
            var profondeurComboBox = this.Find<ComboBox>("Profondeur");
            if (profondeurComboBox == null)
            {
                Console.WriteLine("Impossible de trouver le ComboBox 'Profondeur'");
                return;
            }

            var dataAccess = new DataAccess();
            var uniqueDepths = dataAccess.GetDepthOfPanelHorizontal(selectedLength)
                .Select(panel => panel.Depth)
                .Distinct()
                .OrderBy(depth => depth)
                .ToList();

            profondeurComboBox.Items.Clear();
            foreach (var depth in uniqueDepths)
            {
                profondeurComboBox.Items.Add(depth.ToString());
            }

            Console.WriteLine($"Chargé {uniqueDepths.Count} profondeurs uniques dans le ComboBox");
        }

        // Gestionnaire d'événement pour le bouton "Next"
        private void GoToHeightWindow(object sender, RoutedEventArgs e)
        {
            var mainWindow = VisualRoot as MainWindow;
            if (mainWindow != null)
            {
                var heightView = new Height();
                heightView.SelectedLength = SelectedLength;
                heightView.SelectedDepth = SelectedDepth;
                mainWindow.MainContent.Content = heightView;
            }
        }

        public void OnOpenConsoleClick(object sender, RoutedEventArgs e)
        {
            var consoleWindow = new ConsoleWindow();
            consoleWindow.Show();
        }
    }
}
