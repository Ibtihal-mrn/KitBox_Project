using Avalonia.Controls;
using Avalonia.Interactivity;
using KitBox_Project.Data;
using KitBox_Project.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using Avalonia.Media;
using Avalonia.Controls.Documents;

namespace KitBox_Project.Views
{
    public partial class Door : UserControl
    {
        public int SelectedLength { get; set; }
        public int SelectedDepth { get; set; }
        public int SelectedHeight { get; set; }
        private ComboBox _porteComboBox;
        private ComboBox _availableDoorsComboBox;
        private TextBlock _stockWarning;
        private Border _stockWarningBorder;

        public Door()
        {
            InitializeComponent();

            _porteComboBox = this.FindControl<ComboBox>("Porte")!;
            _availableDoorsComboBox = this.FindControl<ComboBox>("AvailableDoors") ?? new ComboBox();
            _stockWarning = this.FindControl<TextBlock>("StockWarning")!;
            _stockWarningBorder = this.FindControl<Border>("StockWarningBorder")!;
        }

        private void OnPorteSelectionChanged(object? sender, SelectionChangedEventArgs e)
        {
            Console.WriteLine($"[DEBUG] OnPorteSelectionChanged - SelectedLength: {AppState.SelectedLength}, SelectedDepth: {AppState.SelectedDepth}, SelectedHeight: {AppState.SelectedHeight}");
            if (_porteComboBox.SelectedItem is ComboBoxItem item &&
                item.Content?.ToString() == "Yes")
            {
                LoadAvailableDoors();
                _availableDoorsComboBox.IsVisible = true;
                var doorPrompt = this.FindControl<TextBlock>("DoorPrompt");
                if (doorPrompt != null)
                    doorPrompt.IsVisible = true;
                CheckStockWarnings();
            }
            else
            {
                _availableDoorsComboBox.IsVisible = false;
                _availableDoorsComboBox.ItemsSource = null;
                var doorPrompt = this.FindControl<TextBlock>("DoorPrompt");
                if (doorPrompt != null)
                    doorPrompt.IsVisible = false;
                _stockWarningBorder.IsVisible = false;
            }
        }

        private void LoadAvailableDoors()
        {
            Console.WriteLine($"[DEBUG] LoadAvailableDoors - SelectedHeight: {AppState.SelectedHeight}, SelectedLength: {AppState.SelectedLength}");
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
            Console.WriteLine($"[DEBUG] DoorColors count: {doorColors.Count}");
        }

        private void CheckStockWarnings()
        {
            Console.WriteLine($"[DEBUG] CheckStockWarnings - SelectedLength: {AppState.SelectedLength}, SelectedHeight: {AppState.SelectedHeight}");
            var lowStockDoors = StaticArticleDatabase.AllArticles
                .Where(d => d.Reference?.IndexOf("door", StringComparison.OrdinalIgnoreCase) >= 0)
                .Where(d => d.Height == AppState.SelectedHeight && d.Length == AppState.SelectedLength)
                .Where(d => d.NumberOfPiecesAvailable <= 5);

            Console.WriteLine($"[DEBUG] LowStockDoors found: {lowStockDoors.Count()}");
            foreach (var door in lowStockDoors)
            {
                Console.WriteLine($"[DEBUG] Door: {door.Reference}, Stock: {door.NumberOfPiecesAvailable}, Height: {door.Height}, Length: {door.Length}, Color: {door.Color}");
            }

            if (lowStockDoors.Any())
            {
                var warnings = new List<string> { "Items with limited stock:" };
                bool hasOutOfStock = false;

                foreach (var door in lowStockDoors)
                {
                    string color = door.Color ?? "Unknown";
                    string dimensions = $"{door.Length}x{door.Height}";
                    string warning;
                    if (door.NumberOfPiecesAvailable > 0 && door.NumberOfPiecesAvailable <= 5)
                    {
                        warning = $"{door.Reference} {color} - {dimensions} - Remaining stock: <Run FontWeight=\"Bold\">{door.NumberOfPiecesAvailable}</Run> piece(s)";
                    }
                    else
                    {
                        warning = $"{door.Reference} {color} - {dimensions} - Remaining stock: <Run Foreground=\"Red\" FontWeight=\"Bold\">out of stock</Run>";
                        hasOutOfStock = true;
                    }
                    warnings.Add(warning);
                }

                if (hasOutOfStock)
                {
                    warnings.Add("Please contact the seller to order out-of-stock items.");
                }

                _stockWarning.Inlines = new InlineCollection();
                foreach (var warning in warnings)
                {
                    if (warning.Contains("<Run"))
                    {
                        var parts = warning.Split(new[] { "<Run" }, StringSplitOptions.None);
                        foreach (var part in parts)
                        {
                            if (part.Contains("</Run>"))
                            {
                                var runText = part.Substring(part.IndexOf('>') + 1).Replace("</Run>", "");
                                var run = new Run { Text = runText };
                                if (part.Contains("Foreground=\"Red\""))
                                {
                                    run.Foreground = new SolidColorBrush(Colors.Red);
                                }
                                if (part.Contains("FontWeight=\"Bold\""))
                                {
                                    run.FontWeight = FontWeight.Bold;
                                }
                                _stockWarning.Inlines.Add(run);
                            }
                            else if (!string.IsNullOrEmpty(part))
                            {
                                _stockWarning.Inlines.Add(new Run { Text = part });
                            }
                        }
                    }
                    else
                    {
                        _stockWarning.Inlines.Add(new Run { Text = warning });
                    }
                    _stockWarning.Inlines.Add(new LineBreak());
                }

                _stockWarningBorder.IsVisible = true;
            }
            else
            {
                _stockWarningBorder.IsVisible = false;
            }
        }

        private void GoToChoice(object sender, RoutedEventArgs e)
        {
            if (VisualRoot is MainWindow mw)
            {
                if (_porteComboBox.SelectedItem is ComboBoxItem item &&
                    item.Content?.ToString() == "Yes" &&
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
                        AppState.AddToCart(door);

                        if (!selectedColor.Trim().Equals("glass", StringComparison.OrdinalIgnoreCase))
                        {
                            var coupelle = StaticArticleDatabase.AllArticles.FirstOrDefault(a =>
                                a.Reference != null &&
                                a.Reference.ToLower().Contains("coupelle"));

                            if (coupelle != null)
                            {
                                AppState.AddToCart(coupelle);
                                AppState.AddToCart(coupelle);
                            }
                            else
                            {
                                Console.WriteLine("Error: 'coupelle' article not found.");
                            }
                        }
                    }
                    else
                    {
                        Console.WriteLine($"Error: No door available for Height={AppState.SelectedHeight}, Length={AppState.SelectedLength}, Color={selectedColor}.");
                    }
                }

                mw.MainContent.Content = new Choice();
            }
        }

        private void GoToHeight(object sender, RoutedEventArgs e)
        {
            if (VisualRoot is MainWindow mw)
                mw.MainContent.Content = new Height();
        }

        private void GoToFirstPage(object sender, RoutedEventArgs e)
        {
            if (VisualRoot is MainWindow mw)
                mw.ShowChooseUserTypePage();
        }
    }
}