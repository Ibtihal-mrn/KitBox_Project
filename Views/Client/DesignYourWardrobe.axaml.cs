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
        public int SelectedLength { get; private set; }
        public int SelectedDepth { get; private set; }

        public DesignYourWardrobe()
        {
            InitializeComponent();
            Loaded += OnControlLoaded;
        }

        private void OnControlLoaded(object? sender, RoutedEventArgs e)
        {
            LoadLengthData();
        }

        private void LoadLengthData()
        {
            var longueurComboBox = this.FindControl<ComboBox>("Longueur");
            if (longueurComboBox == null) return;

            var dataAccess = new DataAccess();
            var uniqueLengths = dataAccess.GetLengthOfPanelHorizontal()
                .Select(panel => panel.Length)
                .Distinct()
                .OrderBy(length => length)
                .ToList();

            longueurComboBox.Items.Clear();
            foreach (var length in uniqueLengths)
            {
                longueurComboBox.Items.Add(new ComboBoxItem { Content = length.ToString() });
            }

            longueurComboBox.SelectionChanged += OnLengthSelectionChanged;
        }

        private void OnLengthSelectionChanged(object? sender, SelectionChangedEventArgs e)
        {
            if (sender is ComboBox combo && combo.SelectedItem is ComboBoxItem selectedItem &&
                int.TryParse(selectedItem.Content?.ToString(), out int selectedLength))
            {
                SelectedLength = selectedLength;
                LoadDepthData(selectedLength);
            }
        }

        private void LoadDepthData(int selectedLength)
        {
            var profondeurComboBox = this.FindControl<ComboBox>("Profondeur");
            if (profondeurComboBox == null) return;

            var dataAccess = new DataAccess();
            var uniqueDepths = dataAccess.GetDepthOfPanelHorizontal(selectedLength)
                .Select(panel => panel.Depth)
                .Distinct()
                .OrderBy(depth => depth)
                .ToList();

            profondeurComboBox.Items.Clear();
            foreach (var depth in uniqueDepths)
            {
                profondeurComboBox.Items.Add(new ComboBoxItem { Content = depth.ToString() });
            }

            profondeurComboBox.SelectionChanged += OnDepthSelectionChanged;
        }

        private void OnDepthSelectionChanged(object? sender, SelectionChangedEventArgs e)
        {
            if (sender is ComboBox combo && combo.SelectedItem is ComboBoxItem selectedItem &&
                int.TryParse(selectedItem.Content?.ToString(), out int selectedDepth))
            {
                SelectedDepth = selectedDepth;
            }
        }

        private void GoToHeightWindow(object sender, RoutedEventArgs e)
        {
            var errorMessage = this.FindControl<TextBlock>("ErrorMessage");

            if (SelectedLength == 0 || SelectedDepth == 0)
            {
                if (errorMessage != null)
                    errorMessage.IsVisible = true;
                return;
            }

            if (errorMessage != null)
                errorMessage.IsVisible = false;

            AppState.SelectedLength = SelectedLength;
            AppState.SelectedDepth = SelectedDepth;

            Console.WriteLine("[DEBUG] Recherche des panneaux horizontaux dans la base de données:");
            Console.WriteLine($"  Longueur={SelectedLength}, Profondeur={SelectedDepth}, Couleur={AppState.SelectedColor}");

            var allHorizontalPanels = StaticArticleDatabase.AllArticles
                .Where(a => a.Reference != null &&
                    (a.Reference.Contains("panel_horizontal", StringComparison.OrdinalIgnoreCase) ||
                    a.Reference.Contains("Panel horizontal", StringComparison.OrdinalIgnoreCase)))
                .ToList();

            Console.WriteLine($"[DEBUG] Nombre total de panneaux horizontaux trouvés: {allHorizontalPanels.Count}");
            foreach (var panel in allHorizontalPanels)
            {
                Console.WriteLine($"[DEBUG] Référence: '{panel.Reference}', L={panel.Length}, P={panel.Depth}, C={panel.Color}, Stock={panel.NumberOfPiecesAvailable}");
            }

            var horizontalPanel = StaticArticleDatabase.AllArticles.FirstOrDefault(a =>
                a.Reference != null &&
                (a.Reference.Contains("panel_horizontal", StringComparison.OrdinalIgnoreCase) ||
                a.Reference.Contains("Panel horizontal", StringComparison.OrdinalIgnoreCase)) &&
                a.Length == SelectedLength &&
                a.Depth == SelectedDepth &&
                a.Color != null && a.Color.Equals(AppState.SelectedColor, StringComparison.OrdinalIgnoreCase) &&
                a.NumberOfPiecesAvailable > 0);

            if (horizontalPanel != null)
            {
                // 1) Ajout du panneau horizontal
                AppState.AddToCart(horizontalPanel);
                Console.WriteLine($"[SUCCESS] Panneau horizontal ajouté: Ref='{horizontalPanel.Reference}', L={horizontalPanel.Length}, P={horizontalPanel.Depth}, C={horizontalPanel.Color}");

                Console.WriteLine("[DEBUG] Crossbars disponibles en base (FRONT et BACK) :");
                StaticArticleDatabase.AllArticles
                    .Where(a => a.Reference != null &&
                                a.Reference.IndexOf("crossbar", StringComparison.OrdinalIgnoreCase) >= 0)
                    .ToList()
                    .ForEach(a =>
                        Console.WriteLine($"    Ref='{a.Reference}', L={a.Length}, P={a.Depth}, Stock={a.NumberOfPiecesAvailable}")
                    );

                // 2) Ajout du crossbar front de même longueur
                var crossbarFront = StaticArticleDatabase.AllArticles
                    .FirstOrDefault(a =>
                        a.Reference != null &&
                        a.Reference.Contains("crossbar front", StringComparison.OrdinalIgnoreCase) &&
                        a.Length == SelectedLength &&
                        a.NumberOfPiecesAvailable > 0);
                if (crossbarFront != null)
                {
                    AppState.AddToCart(crossbarFront);
                    Console.WriteLine($"[SUCCESS] Crossbar front ajouté: Ref='{crossbarFront.Reference}', L={crossbarFront.Length}");
                }
                else
                {
                    Console.WriteLine($"[WARN] Aucun crossbar front trouvé pour L={SelectedLength}");
                }

                // 3) Ajout du crossbar back de même longueur
                var crossbarBack = StaticArticleDatabase.AllArticles
                    .FirstOrDefault(a =>
                        a.Reference != null &&
                        a.Reference.Contains("crossbar back", StringComparison.OrdinalIgnoreCase) &&
                        a.Length == SelectedLength &&
                        a.NumberOfPiecesAvailable > 0);
                if (crossbarBack != null)
                {
                    AppState.AddToCart(crossbarBack);
                    Console.WriteLine($"[SUCCESS] Crossbar back ajouté: Ref='{crossbarBack.Reference}', L={crossbarBack.Length}");
                }
                else
                {
                    Console.WriteLine($"[WARN] Aucun crossbar back trouvé pour L={SelectedLength}");
                }

                // 4) Ajout de 2 crossbar left or right de même profondeur
                var sideCrossbar = StaticArticleDatabase.AllArticles
                    .FirstOrDefault(a =>
                        a.Reference != null &&
                        a.Reference.Contains("crossbar left or right", StringComparison.OrdinalIgnoreCase) &&
                        a.Depth == SelectedDepth &&
                        a.NumberOfPiecesAvailable > 1);
                if (sideCrossbar != null)
                {
                    AppState.AddToCart(sideCrossbar);
                    AppState.AddToCart(sideCrossbar);
                    Console.WriteLine($"[SUCCESS] 2 crossbar_left_or_right ajoutés: Ref='{sideCrossbar.Reference}', P={sideCrossbar.Depth}");
                }
                else
                {
                    Console.WriteLine($"[WARN] Moins de 2 crossbar_left_or_right disponibles pour P={SelectedDepth}");
                }
            }
            else
            {
                // Si on ne trouve pas avec les dimensions exactes, affichons ce qui est disponible
                Console.WriteLine($"❌ Erreur: Aucun panneau horizontal disponible pour L={SelectedLength}, P={SelectedDepth}, C={AppState.SelectedColor}");

                var panelsWithCorrectLength = StaticArticleDatabase.AllArticles
                    .Where(a => a.Reference != null &&
                        (a.Reference.Contains("panel_horizontal", StringComparison.OrdinalIgnoreCase) ||
                        a.Reference.Contains("Panel horizontal", StringComparison.OrdinalIgnoreCase)) &&
                        a.Length == SelectedLength &&
                        a.NumberOfPiecesAvailable > 0)
                    .ToList();
                Console.WriteLine($"[DEBUG] Panneaux horizontaux avec longueur {SelectedLength} disponibles: {panelsWithCorrectLength.Count}");
                foreach (var panel in panelsWithCorrectLength)
                    Console.WriteLine($"[DEBUG] Option possible: Ref='{panel.Reference}', L={panel.Length}, P={panel.Depth}, C={panel.Color}");

                var panelsWithCorrectColor = StaticArticleDatabase.AllArticles
                    .Where(a => a.Reference != null &&
                        (a.Reference.Contains("panel_horizontal", StringComparison.OrdinalIgnoreCase) ||
                        a.Reference.Contains("Panel horizontal", StringComparison.OrdinalIgnoreCase)) &&
                        a.Color != null && a.Color.Equals(AppState.SelectedColor, StringComparison.OrdinalIgnoreCase) &&
                        a.NumberOfPiecesAvailable > 0)
                    .ToList();
                Console.WriteLine($"[DEBUG] Panneaux horizontaux avec couleur '{AppState.SelectedColor}' disponibles: {panelsWithCorrectColor.Count}");
            }

            if (VisualRoot is MainWindow mainWindow)
            {
                var heightView = new Height
                {
                    SelectedLength = SelectedLength,
                    SelectedDepth = SelectedDepth
                };
                mainWindow.MainContent.Content = heightView;
            }
        }



        private void GoToColor(object sender, RoutedEventArgs e)
        {
            if (VisualRoot is MainWindow mainWindow)
            {
                mainWindow.MainContent.Content = new Color();
            }
        }
    }
}