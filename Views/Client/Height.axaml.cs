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
        public int SelectedHeight { get; private set; } = 0;
        
        private bool _isUpdatingItemsSource = false;

        public Height()
        {
            InitializeComponent();
            Loaded += OnControlLoaded;
        }

        private void OnControlLoaded(object? sender, RoutedEventArgs e)
        {
            LoadHeightData();

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
                
                if (!_isUpdatingItemsSource)
                {
                    // Effacer complètement les avertissements avant de vérifier le stock
                    ResetAllWarnings();
                    
                    // Maintenant vérifier si besoin d'afficher des avertissements
                    CheckAngleIronsStock();
                }
            }
        }

        // Nouvelle méthode pour réinitialiser tous les avertissements
        private void ResetAllWarnings()
        {
            var stockWarningText = this.FindControl<TextBlock>("StockWarning");
            var lowStockPanel = this.FindControl<Border>("LowStockPanel");
            var alternateColorComboBox = this.FindControl<ComboBox>("AlternateColorComboBox");

            if (stockWarningText != null)
            {
                stockWarningText.IsVisible = false;
                stockWarningText.Text = string.Empty;
            }

            if (lowStockPanel != null)
            {
                lowStockPanel.IsVisible = false;
            }

            if (alternateColorComboBox != null)
            {
                alternateColorComboBox.SelectionChanged -= OnAlternateColorSelected;
                alternateColorComboBox.IsVisible = false;
                alternateColorComboBox.ItemsSource = null;
            }
        }

        private void LoadHeightData()
        {
            try
            {
                _isUpdatingItemsSource = true;
                
                var hauteurComboBox = this.FindControl<ComboBox>("Hauteur");
                var lowStockItemsList = this.FindControl<ListBox>("LowStockItemsList");

                if (hauteurComboBox == null || lowStockItemsList == null)
                {
                    return;
                }

                var dataAccess = new DataAccess();

                // 1) Chargement des hauteurs dispos
                var uniqueHeights = dataAccess
                    .GetHeightOfPanel(SelectedLength, SelectedDepth, KitBox_Project.AppState.SelectedColor)
                    .Select(p => p.Height)
                    .Distinct()
                    .OrderBy(h => h)
                    .Select(h => h.ToString())
                    .ToList();
                hauteurComboBox.ItemsSource = uniqueHeights;
                
                var lowStockItems = dataAccess.GetLowStockItems(SelectedLength, SelectedDepth);
                if (lowStockItems.Count > 0)
                {
                    // Afficher l'avertissement de stock limité
                    AfficherAvertissementStock(lowStockItemsList, lowStockItems);
                }
                else
                {
                    // Réinitialiser les affichages
                    ResetAllWarnings();
                }
                
                // Pas de vérification du stock des angle irons ici, cela se fait dans OnHeightSelectionChanged
            }
            finally
            {
                _isUpdatingItemsSource = false;
            }
        }
        
        private void CheckAngleIronsStock()
        {
            if (SelectedHeight <= 0) return;

            var stockWarningText = this.FindControl<TextBlock>("StockWarning");
            var alternateColorComboBox = this.FindControl<ComboBox>("AlternateColorComboBox");
            if (stockWarningText == null || alternateColorComboBox == null) return;

            var dataAccess = new DataAccess();
            string desiredColor = KitBox_Project.AppState.SelectedColor;

            // 1) Récupère le dictionnaire hauteur→(couleur→quantité)
            var aiStock = dataAccess.GetAngleIronStockByHeight(SelectedHeight);

            // 2) Cherche la clé “exacte” dans aiStock (insensible à la casse)
            var actualKey = aiStock.Keys
                .FirstOrDefault(k => string.Equals(k, desiredColor, StringComparison.OrdinalIgnoreCase));

            // 3) Si on a trouvé la clé ET qu’il y a du stock → on arrête tout, pas de warning
            if (actualKey != null && aiStock[actualKey] > 0)
                return;

            // Ici, on sait que la couleur sélectionnée n’est pas dispo,
            // on peut construire la liste des autres couleurs
            var otherColors = aiStock
                .Where(kv => kv.Value > 0 && !string.Equals(kv.Key, desiredColor, StringComparison.OrdinalIgnoreCase))
                .Select(kv => kv.Key)
                .ToList();

            if (otherColors.Any())
            {
                stockWarningText.Text = $"⚠️ Plus d'angle irons en « {desiredColor} ». Veuillez choisir une autre couleur.";
                stockWarningText.Foreground = new SolidColorBrush(Colors.Red);
                stockWarningText.IsVisible = true;

                alternateColorComboBox.SelectionChanged -= OnAlternateColorSelected;
                alternateColorComboBox.ItemsSource = otherColors;
                alternateColorComboBox.IsVisible = true;
                alternateColorComboBox.SelectionChanged += OnAlternateColorSelected;
            }
            else
            {
                stockWarningText.Text = $"⚠️ Aucun angle iron disponible pour cette hauteur.";
                stockWarningText.Foreground = new SolidColorBrush(Colors.Red);
                stockWarningText.IsVisible = true;
                alternateColorComboBox.IsVisible = false;
            }
        }


        private void OnAlternateColorSelected(object? sender, SelectionChangedEventArgs e)
        {
            var alternateColorComboBox = sender as ComboBox;
            if (alternateColorComboBox == null || alternateColorComboBox.SelectedItem == null) return;
            
            string newColor = alternateColorComboBox.SelectedItem.ToString() ?? string.Empty;
            if (string.IsNullOrEmpty(newColor)) return;
            
            KitBox_Project.AppState.SelectedColor = newColor;
            
            var stockWarningText = this.FindControl<TextBlock>("StockWarning");
            if (stockWarningText == null) return;
            
            // Vérifier la disponibilité avec la nouvelle couleur
            var dataAccess = new DataAccess();
            var aiStock = dataAccess.GetAngleIronStockByHeight(SelectedHeight);
            bool hasStock = aiStock.TryGetValue(newColor, out int newColorQty) && newColorQty > 0;
            
            if (hasStock)
            {
                // Mettre à jour le message avec la nouvelle couleur (positif)
                stockWarningText.Text = $"✓ Couleur « {newColor} » sélectionnée. Stock disponible.";
                stockWarningText.Foreground = new SolidColorBrush(Colors.Green);
                
                // Cacher le message après un délai
                var timer = new DispatcherTimer { Interval = TimeSpan.FromSeconds(3) };
                timer.Tick += (s, args) =>
                {
                    ResetAllWarnings();
                    timer.Stop();
                };
                timer.Start();
            }
            else
            {
                // La nouvelle couleur n'a pas de stock non plus
                stockWarningText.Text = $"⚠️ Plus d'angle irons en « {newColor} » non plus. Essayez une autre.";
                stockWarningText.Foreground = new SolidColorBrush(Colors.Red);
            }
        }

        private void AfficherAvertissementStock(ListBox stockList, List<DataAccess.LowStockItem> items)
        {
            var stockWarningText = this.FindControl<TextBlock>("StockWarning");
            var lowStockPanel = this.FindControl<Border>("LowStockPanel");
            
            if (stockWarningText == null || lowStockPanel == null) return;
            
            stockWarningText.Text = "⚠️ Attention : Stock limité pour certains panneaux!";
            stockWarningText.IsVisible = true;
            lowStockPanel.IsVisible = true;

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

        private void GoToDoor(object sender, RoutedEventArgs e)
        {
            if (SelectedHeight == 0)
            {
                var errorMessageTextBlock = this.FindControl<TextBlock>("ErrorMessage");
                if (errorMessageTextBlock != null)
                    errorMessageTextBlock.IsVisible = true;
                return;
            }

            if (VisualRoot is MainWindow mainWindow)
            {
                mainWindow.MainContent.Content = new Door();
            }
        }

        private void GoToSize(object sender, RoutedEventArgs e)
        {
            if (VisualRoot is MainWindow mainWindow)
            {
                mainWindow.MainContent.Content = new DesignYourWardrobe();
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
