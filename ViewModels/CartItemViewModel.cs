using System;
using System.ComponentModel;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Windows.Input;

namespace KitBox_Project.ViewModels
{
    public class CartItemViewModel : INotifyPropertyChanged
    {
        public event PropertyChangedEventHandler? PropertyChanged;

        private int _quantity;

        public string? Code { get; set; }

        public string? Reference { get; set; }
        public string? Color { get; set; }
        public string? Dimensions { get; set; }
        public int Length { get; set; }
        public int Depth { get; set; }
        public int Height { get; set; }
        public decimal SellingPrice { get; set; }
        public int NumberOfPiecesAvailable { get; set; }

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
                    QuantityChanged?.Invoke();
                }
            }
        }

        public decimal TotalPrice => SellingPrice * Quantity;

        public event Action? QuantityChanged;

        protected void OnPropertyChanged([CallerMemberName] string? name = null) =>
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(name));
    }


    // Classe RelayCommand générique, déjà fournie dans ton code
    public class RelayCommand<T> : ICommand
    {
        private readonly Action<T> _execute;
        private readonly Func<T, bool>? _canExecute;

        public RelayCommand(Action<T> execute, Func<T, bool>? canExecute = null)
        {
            _execute = execute ?? throw new ArgumentNullException(nameof(execute));
            _canExecute = canExecute;
        }

        public bool CanExecute(object? parameter)
        {
            if (parameter is null || !(parameter is T))
                return false;

            return _canExecute?.Invoke((T)parameter) ?? true;
        }

        public void Execute(object? parameter)
        {
            if (parameter is T validParameter)
            {
                _execute(validParameter);
            }
        }

        public event EventHandler? CanExecuteChanged;
        public void RaiseCanExecuteChanged() => CanExecuteChanged?.Invoke(this, EventArgs.Empty);
    }
}