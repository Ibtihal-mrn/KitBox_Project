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
            if (longueurComboBox == null) return;

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

            longueurComboBox.SelectionChanged += OnLengthSelectionChanged;
        }

        private void OnLengthSelectionChanged(object? sender, SelectionChangedEventArgs e)
        {
            var longueurComboBox = sender as ComboBox;
            if (longueurComboBox?.SelectedItem is ComboBoxItem selectedItem &&
                int.TryParse(selectedItem.Content?.ToString(), out int selectedLength))
            {
                SelectedLength = selectedLength;
                LoadDepthData(selectedLength);
            }
        }

        private void LoadDepthData(int selectedLength)
        {
            var profondeurComboBox = this.Find<ComboBox>("Profondeur");
            if (profondeurComboBox == null) return;

            var dataAccess = new DataAccess();
            var uniqueDepths = dataAccess.GetDepthOfPanelHorizontal(selectedLength)
                .Select(panel => panel.Depth)
                .Distinct()
                .OrderBy(depth => depth)
                .ToList();

            profondeurComboBox.Items.Clear();
            foreach (var depth in uniqueDepths)
            {
                profondeurComboBox.Items.Add(new ComboBoxItem { Content = depth.ToString() });
            }

            profondeurComboBox.SelectionChanged += OnDepthSelectionChanged;
        }

        private void OnDepthSelectionChanged(object? sender, SelectionChangedEventArgs e)
        {
            var profondeurComboBox = sender as ComboBox;
            if (profondeurComboBox?.SelectedItem is ComboBoxItem selectedItem &&
                int.TryParse(selectedItem.Content?.ToString(), out int selectedDepth))
            {
                SelectedDepth = selectedDepth;
            }
        }

        private void GoToHeightWindow(object sender, RoutedEventArgs e)
        {
            var errorMessageTextBlock = this.Find<TextBlock>("ErrorMessage");

            if (SelectedLength == 0 || SelectedDepth == 0)
            {
                if (errorMessageTextBlock != null)
                {
                    errorMessageTextBlock.IsVisible = true;
                }
                return;
            }

            if (errorMessageTextBlock != null)
            {
                errorMessageTextBlock.IsVisible = false;
            }

            var mainWindow = VisualRoot as MainWindow;
            if (mainWindow != null)
            {
                var heightView = new Height
                {
                    SelectedLength = SelectedLength,
                    SelectedDepth = SelectedDepth
                };
                mainWindow.MainContent.Content = heightView;
            }
        }
        private void GoToColor(object sender, RoutedEventArgs e)
        {
            if (VisualRoot is MainWindow mainWindow)
            {
                mainWindow.MainContent.Content = new Color();
            }
        }

        public void OnOpenConsoleClick(object sender, RoutedEventArgs e)
        {
            var consoleWindow = new ConsoleWindow();
            consoleWindow.Show();
        }
    }
}
