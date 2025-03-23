using Avalonia;
using Avalonia.Controls;
using Avalonia.Markup.Xaml;
using Avalonia.Interactivity;
using Avalonia.Media;
using Avalonia.Threading;
using KitBox_Project.Data;
using System.Linq;
using System;
using System.Collections.Generic;

namespace KitBox_Project.Views
{
    public partial class Height : UserControl
    {
        public int SelectedLength { get; set; }
        public int SelectedDepth { get; set; }
        public int SelectedHeight { get; private set; } = 0;  // Stocker la hauteur sélectionnée

        public Height()
        {
            InitializeComponent();
            Loaded += OnControlLoaded;
        }

        private void OnControlLoaded(object? sender, RoutedEventArgs e)
        {
            LoadHeightData();

            // Abonne l'événement de sélection
            var hauteurComboBox = this.FindControl<ComboBox>("Hauteur");
            if (hauteurComboBox != null)
            {
                hauteurComboBox.SelectionChanged += OnHeightSelectionChanged;
            }
        }

        private void OnHeightSelectionChanged(object? sender, SelectionChangedEventArgs e)
        {
            var hauteurComboBox = sender as ComboBox;
            if (hauteurComboBox != null)
            {
                SelectedHeight = int.TryParse(hauteurComboBox.SelectedItem?.ToString(), out var height) ? height : 0;
            }
        }

        private void LoadHeightData()
        {
            var hauteurComboBox = this.FindControl<ComboBox>("Hauteur");
            var stockWarningText = this.FindControl<TextBlock>("StockWarning");
            var lowStockPanel = this.FindControl<Border>("LowStockPanel");
            var lowStockItemsList = this.FindControl<ListBox>("LowStockItemsList");

            if (hauteurComboBox == null || stockWarningText == null || lowStockPanel == null || lowStockItemsList == null)
            {
                Console.WriteLine("Impossible de trouver les éléments nécessaires");
                return;
            }

            var dataAccess = new DataAccess();

            // Mise à jour des hauteurs disponibles
            var uniqueHeights = dataAccess
                .GetHeightOfPanel(SelectedLength, SelectedDepth)
                .Select(p => p.Height)
                .Distinct()
                .OrderBy(h => h)
                .Select(h => h.ToString())
                .ToList();

            hauteurComboBox.ItemsSource = uniqueHeights;

            // Récupération des articles en stock limité
            var lowStockItems = dataAccess.GetLowStockItems(SelectedLength, SelectedDepth);

            Console.WriteLine("------ STOCK DEBUG COMPLET ------");
            Console.WriteLine($"SelectedLength = {SelectedLength}, SelectedDepth = {SelectedDepth}");
            foreach (var item in lowStockItems)
            {
                Console.WriteLine($"[ITEM] Ref: '{item.Reference}' | Length: {item.Length} | Depth: {item.Depth} | Height: {item.Height} | Stock: {item.AvailableQuantity}");
            }
            // Filtrage intelligent des panneaux en stock limité
            var relevantPanels = lowStockItems
                .Where(item =>
                {
                    if (string.IsNullOrEmpty(item.Reference))
                    {
                        Console.WriteLine("⛔ Référence vide !");
                        return false;
                    }

                    var reference = item.Reference.ToLower().Trim();
                    bool isRelevant = false;

                    // Vérifier les panneaux "back", "left", et "right" avec la profondeur sélectionnée
                    if (reference.Contains("panel back"))
                    {
                        isRelevant = item.Length == SelectedLength;
                        Console.WriteLine($"🔵 Panel Back - Ref: '{reference}' | Match Length: {item.Length == SelectedLength} ({item.Length})");
                    }
                    else if (reference.Contains("left"))
                    {
                        isRelevant = item.Depth == SelectedDepth; // Filtrage à la fois par profondeur et longueur
                        Console.WriteLine($"🟢 Panel Left/Right - Ref: '{reference}' | Match Depth: {item.Depth == SelectedDepth} ({item.Depth}) | Match Length: {item.Length == SelectedLength}");
                    }
                    else
                    {
                        Console.WriteLine($"⚪ Autre type ignoré: {reference}");
                    }

                    return isRelevant;
                })
                .ToList();

            Console.WriteLine($"✅ Nombre de panneaux en stock limité : {relevantPanels.Count}");

            if (relevantPanels.Any())
            {
                AfficherAvertissementStock(stockWarningText, lowStockPanel, lowStockItemsList, relevantPanels);
            }
            else
            {
                stockWarningText.IsVisible = false;
                lowStockPanel.IsVisible = false;
            }
        }

        private void AfficherAvertissementStock(
            TextBlock warningText,
            Border warningPanel,
            ListBox stockList,
            List<DataAccess.LowStockItem> items)
        {
            warningText.Text = "⚠️ Attention : Stock limité pour certains panneaux!";
            warningText.IsVisible = true;
            warningPanel.IsVisible = true;

            var displayList = items.Select(item =>
            {
                var stack = new StackPanel
                {
                    Orientation = Avalonia.Layout.Orientation.Horizontal,
                    Margin = new Thickness(0, 2, 0, 2)
                };

                stack.Children.Add(new TextBlock
                {
                    Text = $"{item.Reference} - Hauteur = {item.Height} mm",
                    Margin = new Thickness(0, 0, 5, 0)
                });
                stack.Children.Add(new TextBlock { Text = " - Stock restant : ", Margin = new Thickness(0, 0, 5, 0) });
                stack.Children.Add(new TextBlock
                {
                    Text = item.AvailableQuantity == 0 ? "Rupture de stock" : item.AvailableQuantity.ToString(),
                    FontWeight = FontWeight.Bold,
                    Foreground = item.AvailableQuantity == 0 ? Brushes.Red : Brushes.Black
                });
                stack.Children.Add(new TextBlock { Text = " pièce(s)" });

                return stack;
            }).ToList();

            Dispatcher.UIThread.InvokeAsync(() =>
            {
                stockList.ItemsSource = null;
                stockList.ItemsSource = displayList;
            });
        }

        private void GoToColor(object sender, RoutedEventArgs e)
        {
            // Vérifie si la hauteur est sélectionnée avant de continuer
            if (SelectedHeight == 0)
            {
                var errorMessageTextBlock = this.FindControl<TextBlock>("ErrorMessage");
                if (errorMessageTextBlock != null)
                    errorMessageTextBlock.IsVisible = true;  // Afficher le message d'erreur
                return;  // Empêcher de passer à la page suivante
            }

            // Naviguer vers la page couleur
            if (VisualRoot is MainWindow mainWindow)
            {
                mainWindow.MainContent.Content = new Color();
            }
        }

        private void GoToSize(object sender, RoutedEventArgs e)
        {
            if (VisualRoot is MainWindow mainWindow)
            {
                mainWindow.MainContent.Content = new DesignYourWardrobe();
            }
        }
    }
}
