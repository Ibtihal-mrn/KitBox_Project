using Avalonia;
using Avalonia.Controls;
using Avalonia.Markup.Xaml;
using Avalonia.Interactivity;

namespace KitBox_Project.Views
{
    public partial class Color : UserControl
    {
        private string selectedColor = "White"; // Variable locale pour stocker la couleur temporairement

        public Color()
        {
            InitializeComponent();
        }

        // Gestionnaire d'événement pour le bouton "Next"
        private void GoToHeight(object sender, RoutedEventArgs e)
        {
            var mainWindow = VisualRoot as MainWindow; // Utilisation de 'as' pour éviter un cast direct
            if (mainWindow != null)
            {
                mainWindow.MainContent.Content = new Height(); // ✅ Modifie le bon ContentControl
            }
        }

        private void GoToDesignYourWardrobe(object sender, RoutedEventArgs e)
        {
            // MODIFIER : Vérifier si une couleur a été sélectionnée avant de passer à la vue suivante
            if (string.IsNullOrEmpty(selectedColor) || selectedColor == "Color not selected")
            {
                if (this.FindControl<TextBlock>("SelectedColorText") is TextBlock textBlock)
                {
                    textBlock.Text = "Veuillez sélectionner une couleur avant de continuer.";
                }
                return;
            }

            // MODIFIER : Stocker la couleur sélectionnée dans AppState avant de passer à DesignYourWardrobe
            KitBox_Project.AppState.SelectedColor = selectedColor;

            var mainWindow = VisualRoot as MainWindow; // Utilisation de 'as' pour éviter un cast direct
            if (mainWindow != null)
            {
                mainWindow.MainContent.Content = new DesignYourWardrobe(); // ✅ Modifie le bon ContentControl
            }
        }

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

                // MODIFIER : Stocker la couleur dans AppState dès qu'elle est sélectionnée
                KitBox_Project.AppState.SelectedColor = selectedColor;
            }
        }
    }
}