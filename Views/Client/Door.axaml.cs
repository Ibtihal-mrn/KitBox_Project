using Avalonia.Controls;
using Avalonia.Interactivity;
using KitBox_Project.Data;
using KitBox_Project.Models;
using System.Collections.Generic;
using System.Linq;

namespace KitBox_Project.Views
{
    public partial class Door : UserControl
    {
        private ComboBox _porteComboBox;
        private ComboBox _availableDoorsComboBox;

        public Door()
        {
            InitializeComponent();

            // Récupère les références aux ComboBox
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
                {
                    doorPrompt.IsVisible = true;  // Affiche le message
                }
            }
            else
            {
                _availableDoorsComboBox.IsVisible = false;
                _availableDoorsComboBox.ItemsSource = null;
                var doorPrompt = this.FindControl<TextBlock>("DoorPrompt");
                if (doorPrompt != null)
                {
                    doorPrompt.IsVisible = false; // Cache le message
                }
            }
        }


        private void LoadAvailableDoors()
        {
            // Récupère les dimensions sélectionnées dans AppState
            int height = AppState.SelectedHeight;
            int length = AppState.SelectedLength;

            var db = new DataAccess();
            List<Article> doors = db.GetAvailableDoors(height, length);

            // Ne prendre que la couleur, et enlever les doublons
            var doorColors = doors
                .Select(d => d.Color)
                .Distinct()
                .OrderBy(c => c)
                .ToList();

            // Lie la liste des couleurs au ComboBox
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
                mw.MainContent.Content = new Choice();
        }

        private void GoToFirstPage(object sender, RoutedEventArgs e)
        {
            if (VisualRoot is MainWindow mw)
                mw.ShowChooseUserTypePage();
        }
    }
}
