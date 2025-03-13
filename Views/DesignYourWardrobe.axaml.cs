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
        public DesignYourWardrobe()
        {
            InitializeComponent();
            
            // Cette méthode s'exécute après le chargement du XAML
            Loaded += OnControlLoaded;
        }
        
        private void OnControlLoaded(object? sender, RoutedEventArgs e)
        {
            // À ce stade, tous les contrôles sont chargés
            LoadLengthData();
        }
        
        private void LoadLengthData()
        {
            // Récupérer le ComboBox directement quand on en a besoin
            var longueurComboBox = this.Find<ComboBox>("Longueur");
            if (longueurComboBox == null)
            {
                Console.WriteLine("Impossible de trouver le ComboBox 'Longueur'");
                return;
            }
            
            // Créer une instance de DataAccess
            var dataAccess = new DataAccess();
            
            // Récupérer les articles horizontaux
            var horizontalPanels = dataAccess.GetLengthOfPanelHorizontal();
            
            // Vider le ComboBox des éléments par défaut
            longueurComboBox.Items.Clear();
            
            // Ajouter les longueurs au ComboBox
            foreach (var panel in horizontalPanels)
            {
                longueurComboBox.Items.Add(new ComboBoxItem { Content = panel.Length.ToString() });
            }
            
            // Afficher un message pour vérifier
            Console.WriteLine($"Chargé {horizontalPanels.Count} longueurs dans le ComboBox");

            // Ajouter un événement de sélection pour le ComboBox de longueur
            longueurComboBox.SelectionChanged += OnLengthSelectionChanged;
        }
        
        private void OnLengthSelectionChanged(object? sender, SelectionChangedEventArgs e)
        {
            var longueurComboBox = sender as ComboBox;
            if (longueurComboBox == null || longueurComboBox.SelectedItem == null)
                return;

            // Récupérer la longueur sélectionnée
            var selectedItem = longueurComboBox.SelectedItem as ComboBoxItem;
            if (selectedItem?.Content == null)
            {
                Console.WriteLine("La longueur sélectionnée n'est pas valide.");
                return;
            }
            string selectedLengthStr = selectedItem.Content?.ToString() ?? string.Empty;
            if (int.TryParse(selectedLengthStr, out int selectedLength))
            {
                // Charger les profondeurs correspondantes
                LoadDepthData(selectedLength);
            }
            else
            {
                Console.WriteLine("La longueur sélectionnée n'est pas valide.");
            }
        }

        private void LoadDepthData(int selectedLength)
        {
            // Récupérer le ComboBox de profondeur
            var profondeurComboBox = this.Find<ComboBox>("Profondeur");
            if (profondeurComboBox == null)
            {
                Console.WriteLine("Impossible de trouver le ComboBox 'Profondeur'");
                return;
            }
            
            // Créer une instance de DataAccess
            var dataAccess = new DataAccess();
            
            // Récupérer les profondeurs correspondantes à la longueur sélectionnée
            var depthPanels = dataAccess.GetDepthOfPanelHorizontal(selectedLength);
            
            // Vider le ComboBox des éléments par défaut
            profondeurComboBox.Items.Clear();
            
            // Ajouter les profondeurs au ComboBox
            foreach (var panel in depthPanels)
            {
                profondeurComboBox.Items.Add(new ComboBoxItem { Content = panel.Depth.ToString() });
            }
            
            // Afficher un message pour vérifier
            Console.WriteLine($"Chargé {depthPanels.Count} profondeurs dans le ComboBox");
        }

        // Gestionnaire d'événement pour le bouton "Next"
        private void GoToHeightWindow(object sender, RoutedEventArgs e)
        {
            var mainWindow = VisualRoot as MainWindow;
            if (mainWindow != null)
            {
                mainWindow.MainContent.Content = new Height();
            }
        }
        
        public void OnOpenConsoleClick(object sender, RoutedEventArgs e)
        {
            var consoleWindow = new ConsoleWindow();
            consoleWindow.Show();
        }
    }
}
