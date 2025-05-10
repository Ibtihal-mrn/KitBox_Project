using System;
using System.ComponentModel;
using System.Linq;
using System.Runtime.CompilerServices;

namespace KitBox_Project.ViewModels
{
    public class CartItemViewModel : INotifyPropertyChanged
    {
        public event PropertyChangedEventHandler? PropertyChanged;

        public string? Reference { get; set; }
        public string? Code { get; set; }
        public string? Color { get; set; }
        public string? Dimensions { get; set; }
        public int Length { get; set; }
        public int Depth { get; set; }
        public int Height { get; set; }
        public int Quantity { get; set; }
        public decimal Price { get; set; }
        public int NumberOfPiecesAvailable { get; set; }

        public decimal TotalPrice => Price * Quantity;

        public string DisplayDimensions =>
            string.IsNullOrWhiteSpace(Dimensions)
                ? string.Join(" x ", new[]
                  {
                      Length > 0 ? $"L:{Length}mm" : null,
                      Depth > 0 ? $"D:{Depth}mm" : null,
                      Height > 0 ? $"H:{Height}mm" : null
                  }.Where(s => s != null))
                : Dimensions;

        public string DisplayName =>
            Reference switch
            {
                string s when s.Contains("panel_horizontal") => "Horizontal Panel",
                string s when s.Contains("panel_back") => "Back Panel",
                string s when s.Contains("panel_left") => "Left Panel",
                string s when s.Contains("panel_right") => "Right Panel",
                string s when s.Contains("angle_iron") => "Angle Iron",
                string s when s.Contains("door") => "Door",
                _ => Reference ?? "Item"
            };

        protected void OnPropertyChanged([CallerMemberName] string? name = null) =>
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(name));
    }
}