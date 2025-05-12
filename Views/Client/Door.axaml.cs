using Avalonia.Controls;
using Avalonia.Interactivity;
using KitBox_Project.Data;
using KitBox_Project.Models;
using System;
using System.Collections.Generic;
using System.Linq;

namespace KitBox_Project.Views
{
    public partial class Door : UserControl
    {
        public int SelectedLength { get; set; }
        public int SelectedDepth { get; set; }
        public int SelectedHeight { get; set; }
        private ComboBox _porteComboBox;
        private ComboBox _availableDoorsComboBox;

        public Door()
        {
            InitializeComponent();

            _porteComboBox = this.FindControl<ComboBox>("Porte")!;
            _availableDoorsComboBox = this.FindControl<ComboBox>("AvailableDoors") ?? new ComboBox();
        }

        private void OnPorteSelectionChanged(object? sender, SelectionChangedEventArgs e)
        {
            if (_porteComboBox.SelectedItem is ComboBoxItem item &&
                item.Content?.ToString() == "Oui")
            {
                LoadAvailableDoors();
                _availableDoorsComboBox.IsVisible = true;
                var doorPrompt = this.FindControl<TextBlock>("DoorPrompt");
                if (doorPrompt != null)
                    doorPrompt.IsVisible = true;
            }
            else
            {
                _availableDoorsComboBox.IsVisible = false;
                _availableDoorsComboBox.ItemsSource = null;
                var doorPrompt = this.FindControl<TextBlock>("DoorPrompt");
                if (doorPrompt != null)
                    doorPrompt.IsVisible = false;
            }
        }

        private void LoadAvailableDoors()
        {
            int height = AppState.SelectedHeight;
            int length = AppState.SelectedLength;

            var db = new DataAccess();
            List<Article> doors = db.GetAvailableDoors(height, length);

            var doorColors = doors
                .Select(d => d.Color?.Trim())
                .Where(c => !string.IsNullOrEmpty(c))
                .Distinct()
                .OrderBy(c => c)
                .ToList();

            _availableDoorsComboBox.ItemsSource = doorColors;
        }

        private void GoToHeight(object sender, RoutedEventArgs e)
        {
            if (VisualRoot is MainWindow mw)
                mw.MainContent.Content = new Height();
        }

       private void GoToChoice(object sender, RoutedEventArgs e)
        {
            if (VisualRoot is MainWindow mw)
            {
                if (_porteComboBox.SelectedItem is ComboBoxItem item &&
                    item.Content?.ToString() == "Oui" &&
                    _availableDoorsComboBox.SelectedItem is string selectedColor)
                {
                    var door = StaticArticleDatabase.AllArticles.FirstOrDefault(a =>
                        a.Reference != null &&
                        a.Reference.ToLower().Contains("door") &&
                        a.Height == AppState.SelectedHeight &&
                        a.Length == AppState.SelectedLength &&
                        a.Color?.Trim().Equals(selectedColor.Trim(), StringComparison.OrdinalIgnoreCase) == true &&
                        a.NumberOfPiecesAvailable > 0);

                    if (door != null)
                    {
                        AppState.AddToCart(door);

                        // ✅ Si la couleur n’est pas "glass", on ajoute deux "coupelles"
                        if (!selectedColor.Trim().Equals("glass", StringComparison.OrdinalIgnoreCase))
                        {
                            var coupelle = StaticArticleDatabase.AllArticles.FirstOrDefault(a =>
                                a.Reference != null &&
                                a.Reference.ToLower().Contains("coupelle"));

                            if (coupelle != null)
                            {
                                AppState.AddToCart(coupelle);
                                AppState.AddToCart(coupelle); // Ajout une deuxième fois
                            }
                            else
                            {
                                Console.WriteLine("Erreur : Article 'coupelle' non trouvé.");
                            }
                        }
                    }
                    else
                    {
                        Console.WriteLine($"Erreur : Aucune porte disponible pour Hauteur={AppState.SelectedHeight}, Longueur={AppState.SelectedLength}, Couleur={selectedColor}.");
                    }
                }

                mw.MainContent.Content = new Choice();
            }
        }


        private void GoToFirstPage(object sender, RoutedEventArgs e)
        {
            if (VisualRoot is MainWindow mw)
                mw.ShowChooseUserTypePage();
        }
    }
}
