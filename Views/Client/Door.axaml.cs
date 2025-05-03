using Avalonia;
using Avalonia.Controls;
using Avalonia.Markup.Xaml;
using Avalonia.Interactivity;
using KitBox_Project.Data;
using System.Linq;
using System;
using KitBox_Project.Models;

namespace KitBox_Project.Views
{
    public partial class Door : UserControl
    {
        public string SelectedDoorOption { get; private set; } = string.Empty; // Stocke la couleur sélectionnée
        public int SelectedHeight { get; private set; }
        public int SelectedLength { get; private set; }
        public string SelectedColor { get; private set; } = string.Empty;

        public Door()
        {
            InitializeComponent();
            Loaded += OnControlLoaded;
        }

        private void InitializeComponent()
        {
            AvaloniaXamlLoader.Load(this);
        }

        private void OnControlLoaded(object? sender, RoutedEventArgs e)
        {
            LoadDoorData();

            var porteComboBox = this.FindControl<ComboBox>("Porte");
            if (porteComboBox != null)
            {
                porteComboBox.SelectionChanged += OnPorteSelectionChanged;
            }
        }

        private void OnPorteSelectionChanged(object? sender, SelectionChangedEventArgs e)
        {
            var porteComboBox = sender as ComboBox;
            if (porteComboBox?.SelectedItem is ComboBoxItem selectedItem && selectedItem.Tag is Article selectedDoor)
            {
                SelectedDoorOption = selectedDoor.Color ?? string.Empty; // Stocke la couleur sélectionnée
                SelectedHeight = selectedDoor.Height;
                SelectedLength = selectedDoor.Length;
                SelectedColor = selectedDoor.Color ?? string.Empty;
                Console.WriteLine($"[DEBUG] Porte sélectionnée : Couleur: {SelectedDoorOption}, Height: {SelectedHeight}, Length: {SelectedLength}");
            }
            else
            {
                SelectedDoorOption = string.Empty;
                SelectedHeight = 0;
                SelectedLength = 0;
                SelectedColor = string.Empty;
                Console.WriteLine("[DEBUG] Aucune porte sélectionnée, valeurs réinitialisées.");
            }
        }

        private void LoadDoorData()
        {
            var porteComboBox = this.FindControl<ComboBox>("Porte");
            var errorMessageTextBlock = this.FindControl<TextBlock>("ErrorMessage");

            if (porteComboBox == null)
            {
                Console.WriteLine("Erreur : ComboBox 'Porte' non trouvé");
                return;
            }

            var dataAccess = new DataAccess();

            // Récupérer toutes les portes disponibles
            var availableDoors = dataAccess.GetAvailableDoors(0, 0)
                .Select(d => new ComboBoxItem
                {
                    Content = d.Color, // Afficher uniquement la couleur
                    Tag = d // Stocker l'article pour accéder aux détails
                })
                .DistinctBy(item => item.Content?.ToString()) // Éliminer les doublons de couleurs
                .OrderBy(item => item.Content?.ToString())
                .ToList();

            Console.WriteLine($"[DEBUG] Nombre de portes récupérées : {availableDoors.Count}");
            foreach (var door in availableDoors)
            {
                Console.WriteLine($"[DEBUG] Porte : {door.Content}");
            }

            porteComboBox.Items.Clear();
            foreach (var door in availableDoors)
            {
                porteComboBox.Items.Add(door);
            }

            if (!availableDoors.Any() && errorMessageTextBlock != null)
            {
                errorMessageTextBlock.Text = "⚠️ Aucune porte disponible dans la base de données.";
                errorMessageTextBlock.IsVisible = true;
                porteComboBox.Items.Clear();
            }
            else if (errorMessageTextBlock != null)
            {
                errorMessageTextBlock.IsVisible = false;
            }

            // Restaurer la sélection précédente si applicable
            if (!string.IsNullOrEmpty(SelectedDoorOption))
            {
                var selectedDoorItem = availableDoors.FirstOrDefault(item => item.Content?.ToString() == SelectedDoorOption);
                porteComboBox.SelectedItem = selectedDoorItem;
                Console.WriteLine($"[DEBUG] Porte restaurée : {SelectedDoorOption}");
            }
        }

        private void GoToHeight(object sender, RoutedEventArgs e)
        {
            if (VisualRoot is MainWindow mainWindow)
            {
                mainWindow.MainContent.Content = new Height
                {
                    SelectedLength = SelectedLength,
                    SelectedDepth = 0 // Depth n'est pas utilisé ici
                };
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

        private void GoToFirstPage(object sender, RoutedEventArgs e)
        {
            if (VisualRoot is MainWindow mainWindow)
            {
                mainWindow.ShowChooseUserTypePage(); // ✅ les événements sont rebranchés ici
            }
        }
    }
}