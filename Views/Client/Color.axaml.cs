using Avalonia;
using Avalonia.Controls;
using Avalonia.Markup.Xaml;
using Avalonia.Interactivity;
using System;

namespace KitBox_Project.Views
{
    public partial class Color : UserControl
    {
        private bool fromChoice = false; 

        public Color() : this(false)
        {
        }

        public Color(bool fromChoice)
        {
            InitializeComponent();
            this.fromChoice = fromChoice; 
        }

        private void SelectColor(object sender, Avalonia.Interactivity.RoutedEventArgs e)
        {
            if (sender is Button button && button.Tag is not null)
            {
                // Mettre à jour AppState.SelectedColor directement
                AppState.SelectedColor = button.Tag?.ToString() ?? "White"; // Valeur par défaut si Tag est null

                // Afficher la couleur sélectionnée dans l'interface
                if (this.FindControl<TextBlock>("SelectedColorText") is TextBlock textBlock)
                {
                    textBlock.Text = $"Couleur sélectionnée : {AppState.SelectedColor}";
                }

                Console.WriteLine($"Couleur sélectionnée : {AppState.SelectedColor}");
            }
        }

        private void GoToDesign(object sender, RoutedEventArgs e)
        {
            // Vérifier si une couleur a été sélectionnée
            if (string.IsNullOrEmpty(AppState.SelectedColor))
            {
                if (this.FindControl<TextBlock>("SelectedColorText") is TextBlock textBlock)
                {
                    textBlock.Text = "Veuillez sélectionner une couleur avant de continuer.";
                }
                return;
            }

            // Passer à la vue suivante
            var mainWindow = VisualRoot as MainWindow;
            if (mainWindow != null)
            {
                mainWindow.MainContent.Content = new DesignYourWardrobe();
            }
        }

        private void GoToPrevious(object sender, RoutedEventArgs e)
        {
            if (VisualRoot is MainWindow mainWindow)
            {
                //Navigue vers Choice si fromChoice est vrai, sinon vers la page initiale
                if (fromChoice)
                {
                    mainWindow.MainContent.Content = new Choice();
                }
                else
                {
                    mainWindow.ShowChooseUserTypePage();
                }
            }
        }

        private void GoToFirstPage(object sender, RoutedEventArgs e)
        {
            if (VisualRoot is MainWindow mainWindow)
            {
                mainWindow.ShowChooseUserTypePage();
            }
        }
    }
}