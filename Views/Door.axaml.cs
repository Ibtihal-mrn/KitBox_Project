using Avalonia;
using Avalonia.Controls;
using Avalonia.Markup.Xaml;
using Avalonia.Interactivity;

namespace KitBox_Project.Views
{
    public partial class Door : UserControl
    {
        public string SelectedDoorOption { get; private set; } = string.Empty;

        public Door()
        {
            InitializeComponent();
            Loaded += OnControlLoaded;
        }

        private void OnControlLoaded(object? sender, RoutedEventArgs e)
        {
            var porteComboBox = this.FindControl<ComboBox>("Porte");
            if (porteComboBox != null)
            {
                porteComboBox.SelectionChanged += OnPorteSelectionChanged;
            }
        }

        private void OnPorteSelectionChanged(object? sender, SelectionChangedEventArgs e)
        {
            var porteComboBox = sender as ComboBox;
            if (porteComboBox?.SelectedItem is ComboBoxItem selectedItem)
            {
                SelectedDoorOption = selectedItem.Content?.ToString() ?? string.Empty;
            }
            else
            {
                SelectedDoorOption = string.Empty;
            }
        }

        private void GoToHeight(object sender, RoutedEventArgs e)
        {
            if (VisualRoot is MainWindow mainWindow)
            {
                mainWindow.MainContent.Content = new Height();
            }
        }

        private void GoToChoice(object sender, RoutedEventArgs e)
        {
            var errorMessageTextBlock = this.FindControl<TextBlock>("ErrorMessage");

            // Vérification de la sélection
            if (string.IsNullOrWhiteSpace(SelectedDoorOption))
            {
                if (errorMessageTextBlock != null)
                    errorMessageTextBlock.IsVisible = true;
                return;
            }

            if (errorMessageTextBlock != null)
                errorMessageTextBlock.IsVisible = false;

            if (VisualRoot is MainWindow mainWindow)
            {
                mainWindow.MainContent.Content = new Choice();
            }
        }
    }
}

