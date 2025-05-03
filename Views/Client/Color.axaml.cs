using Avalonia;
using Avalonia.Controls;
using Avalonia.Markup.Xaml;
using Avalonia.Interactivity;

namespace KitBox_Project.Views
{
    public partial class Color : UserControl
    {
        public Color()
        {
            InitializeComponent();
        }

        // Gestionnaire d'événement pour le bouton "Next"
        
    
        private void GoToDesign(object sender, RoutedEventArgs e)
        {
            var mainWindow = VisualRoot as MainWindow; // Utilisation de 'as' pour éviter un cast direct
            if (mainWindow != null)
            {
                mainWindow.MainContent.Content = new DesignYourWardrobe(); // ✅ Modifie le bon ContentControl
            }
        }

        private string selectedColor = "White";
        private void SelectColor(object sender, Avalonia.Interactivity.RoutedEventArgs e)
        {
            if (sender is Button button && button.Tag is not null)
            {
                selectedColor = button.Tag?.ToString() ?? "Color not selected";  // Stocke la couleur du bouton

        // Vérifie que SelectedColorText n'est pas null
                if (this.FindControl<TextBlock>("SelectedColorText") is TextBlock textBlock)
                    {
                        textBlock.Text = $"Couleur sélectionnée : {selectedColor}";
                    }
            }
        }

        private void GoToFirstPage(object sender, RoutedEventArgs e)
        {
            if (VisualRoot is MainWindow mainWindow)
            {
                mainWindow.ShowChooseUserTypePage(); // ✅ les événements sont rebranchés ici
            }
        }

        

    }
}
