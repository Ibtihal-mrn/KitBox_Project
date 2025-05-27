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
        private bool isStacking = false;

        public Height() : this(false)
        {
        }

        public Height(bool isStacking)
        {
            InitializeComponent();
            this.isStacking = isStacking;
            Initialized += OnInitialized;
        }

        private void OnInitialized(object? sender, EventArgs e)
        {
            if (SelectedLength > 0 && SelectedDepth > 0)
            {
                LoadHeightData();
            }

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
                    ResetAllWarnings();
                    CheckAngleIronsStock();
                }
            }
        }

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
                    AfficherAvertissementStock(lowStockItemsList, lowStockItems);
                }
                else
                {
                    ResetAllWarnings();
                }
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
            // Use main color for angle irons (no specific color saved)
            string desiredAngleIronColor = KitBox_Project.AppState.SelectedColor ?? string.Empty;

            var aiStock = dataAccess.GetAngleIronStockByHeight(SelectedHeight);
            var actualKey = aiStock.Keys
                .FirstOrDefault(k => string.Equals(k, desiredAngleIronColor, StringComparison.OrdinalIgnoreCase));

            if (actualKey != null && aiStock[actualKey] > 0)
                return;

            var otherColors = aiStock
                .Where(kv => kv.Value > 0 && !string.Equals(kv.Key, desiredAngleIronColor, StringComparison.OrdinalIgnoreCase))
                .Select(kv => kv.Key)
                .ToList();

            if (otherColors.Any())
            {
                stockWarningText.Text = $"Plus d'angle irons en « {desiredAngleIronColor} ». Veuillez choisir une autre couleur.";
                stockWarningText.Foreground = new SolidColorBrush(Colors.Red);
                stockWarningText.IsVisible = true;

                alternateColorComboBox.SelectionChanged -= OnAlternateColorSelected;
                alternateColorComboBox.ItemsSource = otherColors;
                alternateColorComboBox.IsVisible = true;
                alternateColorComboBox.SelectionChanged += OnAlternateColorSelected;
            }
            else
            {
                stockWarningText.Text = $" Aucun angle iron disponible pour cette hauteur.";
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

            var stockWarningText = this.FindControl<TextBlock>("StockWarning");
            if (stockWarningText == null) return;

            var dataAccess = new DataAccess();
            var aiStock = dataAccess.GetAngleIronStockByHeight(SelectedHeight);
            bool hasStock = aiStock.TryGetValue(newColor, out int newColorQty) && newColorQty > 0;

            if (hasStock)
            {
                // Save the selected color TEMPORARILY in AppState
                KitBox_Project.AppState.SelectedAngleIronColor = newColor;
                
                stockWarningText.Text = $"Couleur « {newColor} » sélectionnée. Vous pouvez maintenant continuer.";
                stockWarningText.Foreground = new SolidColorBrush(Colors.Green);

                Console.WriteLine($"[DEBUG] Couleur angle iron mise à jour TEMPORAIREMENT : '{newColor}'");
                // Hide combobox after selection
                alternateColorComboBox.IsVisible = false;
            }
            else
            {
                stockWarningText.Text = $"Plus d'angle irons en « {newColor} » non plus. Essayez une autre.";
                stockWarningText.Foreground = new SolidColorBrush(Colors.Red);
            }
        }

        private void AfficherAvertissementStock(ListBox stockList, List<DataAccess.LowStockItem> items)
        {
            var stockWarningText = this.FindControl<TextBlock>("StockWarning");
            var lowStockPanel = this.FindControl<Border>("LowStockPanel");

            if (stockWarningText == null || lowStockPanel == null) return;

            stockWarningText.Text = " Attention : Stock limité pour certains panneaux!";
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

        private void GoToSize(object? sender, RoutedEventArgs e)
        {
            var mainWindow = VisualRoot as MainWindow;
            if (mainWindow != null)
            {
                mainWindow.MainContent.Content = new DesignYourWardrobe(isStacking);
            }
        }

        private void GoToDoor(object? sender, RoutedEventArgs e)
        {
            var errorMessage = this.FindControl<TextBlock>("ErrorMessage");
            
            if (SelectedLength == 0 || SelectedDepth == 0 || SelectedHeight == 0)
            {
                if (errorMessage != null)
                    errorMessage.IsVisible = true;
                return;
            }
            if (errorMessage != null)
                errorMessage.IsVisible = false;

            // global status update
            AppState.SelectedLength = SelectedLength;
            AppState.SelectedDepth = SelectedDepth;
            AppState.SelectedHeight = SelectedHeight;

            // looking for back panels
            Console.WriteLine("[DEBUG] Recherche de panneaux BACK...");
            Console.WriteLine($"  Longueur={SelectedLength}, Hauteur={SelectedHeight}, Couleur={AppState.SelectedColor}");
            Console.WriteLine($"[DEBUG] SelectedColor (brut) = '{AppState.SelectedColor}' (Length={AppState.SelectedColor?.Length})");

            var allBackPanels = StaticArticleDatabase.AllArticles
                .Where(a => a.Reference != null &&
                            a.Reference.Replace(" ", "_").ToLower().Contains("panel_back"))
                .ToList();

            Console.WriteLine($"[DEBUG] {allBackPanels.Count} panneau(x) back trouvé(s) en base.");
            foreach (var panel in allBackPanels)
            {
                Console.WriteLine($"  → Ref: '{panel.Reference}', L={panel.Length}, H={panel.Height}, C='{panel.Color}', Stock={panel.NumberOfPiecesAvailable}");
            }

            var backPanel = allBackPanels.FirstOrDefault(a =>
                a.Length == SelectedLength &&
                a.Height == SelectedHeight &&
                a.Color != null &&
                a.Color.Trim().Equals(AppState.SelectedColor?.Trim(), StringComparison.OrdinalIgnoreCase) &&
                a.NumberOfPiecesAvailable > 0);

            if (backPanel != null)
            {
                AppState.AddToCart(backPanel);
                Console.WriteLine($"[SUCCESS] Panneau BACK ajouté au panier: '{backPanel.Reference}'");
            }
            else
            {
                Console.WriteLine($" Aucun panneau BACK trouvé pour L={SelectedLength}, H={SelectedHeight}, C={AppState.SelectedColor}");
            }

            // Panels LEFT OR RIGHT 
            Console.WriteLine("\n[DEBUG] Recherche de panneaux LEFT OR RIGHT...");
            var allLeftRightPanels = StaticArticleDatabase.AllArticles
                .Where(a => a.Reference != null &&
                            a.Reference.Replace(" ", "_").ToLower().Contains("panel_left_or_right"))
                .ToList();

            Console.WriteLine($"[DEBUG] {allLeftRightPanels.Count} panneau(x) LEFT OR RIGHT trouvé(s) en base.");
            foreach (var panel in allLeftRightPanels)
            {
                Console.WriteLine($"  → Ref: '{panel.Reference}', H={panel.Height}, D={panel.Depth}, C='{panel.Color}', Stock={panel.NumberOfPiecesAvailable}");
            }

            var leftRightPanel = allLeftRightPanels.FirstOrDefault(a =>
                a.Height == SelectedHeight &&
                a.Depth == SelectedDepth &&
                a.Color != null &&
                a.Color.Trim().Equals(AppState.SelectedColor?.Trim(), StringComparison.OrdinalIgnoreCase) &&
                a.NumberOfPiecesAvailable >= 2);

            if (leftRightPanel != null)
            {
                AppState.AddToCart(leftRightPanel);
                AppState.AddToCart(leftRightPanel);
                Console.WriteLine($"[SUCCESS] 2 panneaux LEFT OR RIGHT ajoutés au panier: '{leftRightPanel.Reference}'");
            }
            else
            {
                Console.WriteLine($" Aucun panneau LEFT OR RIGHT trouvé pour H={SelectedHeight}, D={SelectedDepth}, C={AppState.SelectedColor}");
            }

            // Full log with vertical battens 
            Console.WriteLine("\n[DEBUG] Liste complète des vertical battens en base :");
            StaticArticleDatabase.AllArticles
                .Where(a => a.Reference != null &&
                            a.Reference.IndexOf("batten", StringComparison.OrdinalIgnoreCase) >= 0)
                .ToList()
                .ForEach(a =>
                    Console.WriteLine($"    Ref='{a.Reference}', L={a.Length}, Stock={a.NumberOfPiecesAvailable}")
                );

            // Adding 4 vertical battens
            Console.WriteLine("\n[DEBUG] Ajout de 4 vertical battens...");
            var verticalBatten = StaticArticleDatabase.AllArticles
                .FirstOrDefault(a =>
                    a.Reference != null &&
                    a.Reference.IndexOf("batten", StringComparison.OrdinalIgnoreCase) >= 0 &&
                    a.Height == SelectedHeight &&
                    a.NumberOfPiecesAvailable > 3);
            if (verticalBatten != null)
            {
                for (int i = 0; i < 4; i++)
                {
                    AppState.AddToCart(verticalBatten);
                }
                Console.WriteLine($"[SUCCESS] 4 vertical battens ajoutés: '{verticalBatten.Reference}'");
            }
            else
            {
                Console.WriteLine($" Aucun vertical batten avec profondeur {SelectedDepth} et stock > 3 trouvé");
            }

            // Adding angle irons with the temporary color 
            Console.WriteLine("\n[DEBUG] Ajout angle irons...");
            
            // Determine the color to be used for the angle irons
            string angleIronColor = KitBox_Project.AppState.SelectedAngleIronColor ?? KitBox_Project.AppState.SelectedColor ?? string.Empty;
            
            Console.WriteLine($"[DEBUG] Couleur angle iron à utiliser : '{angleIronColor}'");
            Console.WriteLine($"[DEBUG] Hauteur recherchée : {SelectedHeight}");

            var angleIrons = StaticArticleDatabase.AllArticles
                .Where(a => a.Reference != null &&
                            a.Reference.IndexOf("angle iron", StringComparison.OrdinalIgnoreCase) >= 0)
                .ToList();

            Console.WriteLine($"[DEBUG] {angleIrons.Count} angle irons trouvés en base");

            // Search with exact color (case-sensitive d'abord)
            var angleIronExact = angleIrons.FirstOrDefault(a =>
                a.Color != null &&
                a.Color.Trim() == angleIronColor.Trim() &&
                a.Height == SelectedHeight &&
                a.NumberOfPiecesAvailable >= 4);

            // If not found, searching case-insensitive
            var angleIronToAdd = angleIronExact ?? angleIrons.FirstOrDefault(a =>
                a.Color != null &&
                a.Color.Trim().Equals(angleIronColor.Trim(), StringComparison.OrdinalIgnoreCase) &&
                a.Height == SelectedHeight &&
                a.NumberOfPiecesAvailable >= 4);

            Console.WriteLine($"[DEBUG] Résultat recherche exacte: {(angleIronExact != null ? "Trouvé" : "Non trouvé")}");
            Console.WriteLine($"[DEBUG] Résultat recherche insensitive: {(angleIronToAdd != null ? "Trouvé" : "Non trouvé")}");

            if (angleIronToAdd != null)
            {
                Console.WriteLine($"[DEBUG] Angle iron sélectionné : '{angleIronToAdd.Reference}', Couleur='{angleIronToAdd.Color}', Stock={angleIronToAdd.NumberOfPiecesAvailable}");

                for (int i = 0; i < 4; i++)
                {
                    AppState.AddToCart(angleIronToAdd);
                    Console.WriteLine($"[DEBUG] Angle iron {i + 1}/4 ajouté au panier");
                }
                Console.WriteLine($"[SUCCESS] 4 angle irons ajoutés en couleur '{angleIronColor}': '{angleIronToAdd.Reference}'");
            }
            else
            {
                Console.WriteLine($" Aucun angle iron trouvé en couleur '{angleIronColor}', hauteur {SelectedHeight}, stock >= 4");

                // Debug : show all possible matches
                var matches = angleIrons.Where(a =>
                    a.Height == SelectedHeight).ToList();

                Console.WriteLine($"[DEBUG] Angle irons disponibles pour hauteur {SelectedHeight}:");
                foreach (var match in matches)
                {
                    Console.WriteLine($"  → Couleur: '{match.Color}' (Stock: {match.NumberOfPiecesAvailable})");
                }
            }

            // Reset temporary color after use
            Console.WriteLine($"[DEBUG] Réinitialisation de SelectedAngleIronColor (était: '{KitBox_Project.AppState.SelectedAngleIronColor}')");
            KitBox_Project.AppState.SelectedAngleIronColor = null;
            Console.WriteLine("[DEBUG] SelectedAngleIronColor réinitialisé à null");

            // to the door view
            if (VisualRoot is MainWindow mainWindow)
            {
                var doorView = new Door
                {
                    SelectedLength = SelectedLength,
                    SelectedDepth  = SelectedDepth,
                    SelectedHeight = SelectedHeight
                };
                mainWindow.MainContent.Content = doorView;
            }
        }

        private void GoToFirstPage(object? sender, RoutedEventArgs e)
        {
            if (VisualRoot is MainWindow mainWindow)
            {
                mainWindow.ShowChooseUserTypePage();
            }
        }
    }
}