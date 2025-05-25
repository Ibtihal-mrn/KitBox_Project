using System;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Runtime.CompilerServices;

namespace KitBox_Project.ViewModels
{
    public class ConfirmationViewModel : INotifyPropertyChanged
    {
        private string _orderId = string.Empty;
        public string OrderId
        {
            get => _orderId;
            set { _orderId = value; OnPropertyChanged(); }
        }

        private DateTime _orderDate;
        /// <summary>
        /// Date et heure de la commande.
        /// </summary>
        public DateTime OrderDate
        {
            get => _orderDate;
            set { _orderDate = value; OnPropertyChanged(); }
        }

        public ObservableCollection<ConfirmedOrderItemViewModel> Items { get; }
            = new ObservableCollection<ConfirmedOrderItemViewModel>();

        public decimal GrandTotal => Items.Sum(i => i.TotalPrice);

        public ConfirmationViewModel()
        {
            Items.CollectionChanged += (s, e) => OnPropertyChanged(nameof(GrandTotal));
        }

        public event PropertyChangedEventHandler? PropertyChanged;
        protected void OnPropertyChanged([CallerMemberName] string? name = null)
            => PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(name));
    }
}
