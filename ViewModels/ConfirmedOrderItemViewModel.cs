using System.ComponentModel;
using System.Runtime.CompilerServices;

namespace KitBox_Project.ViewModels
{
    public class ConfirmedOrderItemViewModel : INotifyPropertyChanged
    {
        public string Reference  { get; set; } = string.Empty;
        public string Color      { get; set; } = string.Empty;
        public string Dimensions { get; set; } = string.Empty;
        public decimal SellingPrice { get; set; }
        public int Quantity        { get; set; }

        public decimal TotalPrice => SellingPrice * Quantity;

        public event PropertyChangedEventHandler? PropertyChanged;
        protected void OnPropertyChanged([CallerMemberName] string? name = null)
            => PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(name));
    }
}
