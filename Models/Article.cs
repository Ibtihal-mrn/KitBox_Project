using System;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using System.Text.Json.Serialization;

namespace KitBox_Project.Models
{
    public class Article : INotifyPropertyChanged
    {
        public string? Code { get; set; }
        public string? Reference { get; set; }
        public string? Color { get; set; }
        public string? Dimensions { get; set; }
        public int Length { get; set; }
        public int Height { get; set; }
        public int Depth { get; set; }
        public decimal PriceSupplierUno { get; set; }
        public int DelaySupplierUno { get; set; }
        public decimal PriceSupplierDos { get; set; }
        public int DelaySupplierDos { get; set; }
        public decimal SellingPrice { get; set; }
        public int NumberOfPiecesAvailable { get; set; }

        private int _quantity = 1;
        public int Quantity
        {
            get => _quantity;
            set
            {
                if (_quantity != value)
                {
                    _quantity = value;
                    OnPropertyChanged();
                    OnPropertyChanged(nameof(TotalPrice));
                }
            }
        }

        [JsonIgnore]
        public decimal TotalPrice => SellingPrice * Quantity;

        private void DecreaseQuantity()
        {
            if (Quantity > 1)
            {
                Quantity--;
            }
        }

        public event PropertyChangedEventHandler? PropertyChanged;
        protected void OnPropertyChanged([CallerMemberName] string? propertyName = null)
            => PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
    }
}
