using System;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Windows.Input;
using KitBox_Project.Models;
using MsBox.Avalonia.ViewModels.Commands;

namespace KitBox_Project.ViewModels
{
    public class ShoppingCartViewModel : INotifyPropertyChanged
    {
        private ObservableCollection<Article> _cartItems = new ObservableCollection<Article>();

        public ObservableCollection<Article> CartItems
        {
            get => _cartItems;
            set
            {
                if (_cartItems != value)
                {
                    _cartItems = value;
                    OnPropertyChanged();
                    OnPropertyChanged(nameof(TotalPrice));
                    OnPropertyChanged(nameof(TotalItemCount));
                    OnPropertyChanged(nameof(IsCartEmpty));
                }
            }
        }

        public decimal TotalPrice => CartItems?.Sum(item => item.TotalPrice) ?? 0m;
        public int TotalItemCount => CartItems?.Sum(item => item.Quantity) ?? 0;
        public bool IsCartEmpty => TotalItemCount == 0;

        public ICommand ClearCartCommand { get; }
        public ICommand DecreaseQuantityCommand { get; }
        public ICommand IncreaseQuantityCommand { get; }
        public ICommand RemoveItemCommand { get; }

        public ShoppingCartViewModel()
        {
            UpdateCartItems(); // premier affichage groupé

            AppState.SelectedArticles.CollectionChanged += (s, e) =>
            {
                UpdateCartItems(); // mise à jour à chaque changement
            };

            ClearCartCommand = new RelayCommand(_ => ClearCart(), _ => !IsCartEmpty);
            DecreaseQuantityCommand = new RelayCommand(DecreaseQuantity, CanDecreaseQuantity);
            IncreaseQuantityCommand = new RelayCommand(IncreaseQuantity, CanIncreaseQuantity); // Added CanExecute
            RemoveItemCommand = new RelayCommand<Article>(RemoveItem);  // Commande générique pour supprimer un item
        }

        private void RemoveItem(Article item)
        {
            if (item != null)
            {
                // Suppression dans AppState.SelectedArticles (source de données)
                var articlesToRemove = AppState.SelectedArticles
                    .Where(a => a.Reference == item.Reference && 
                           a.Color == item.Color && 
                           a.Dimensions == item.Dimensions)
                    .ToList();

                foreach (var article in articlesToRemove)
                {
                    AppState.SelectedArticles.Remove(article);
                }

                // La mise à jour de CartItems se fera automatiquement via l'événement CollectionChanged
            }
        }


        private void UpdateCartItems()
        {
            var grouped = AppState.SelectedArticles
                .GroupBy(a => new { a.Reference, a.Color, a.Dimensions })
                .Select(group =>
                {
                    var first = group.First();
                    return new Article
                    {
                        Reference = first.Reference,
                        Color = first.Color,
                        Dimensions = first.Dimensions,
                        Length = first.Length,
                        Depth = first.Depth,
                        Height = first.Height,
                        SellingPrice = first.SellingPrice,
                        Quantity = group.Sum(a => a.Quantity)
                    };
                });

            CartItems = new ObservableCollection<Article>(grouped);
        }

        private void DecreaseQuantity(object? parameter)
        {
            if (parameter is Article article && article.Quantity > 1)
            {
                article.Quantity--;
                NotifyPriceChange();
            }
        }

        private void IncreaseQuantity(object? parameter)
        {
            if (parameter is Article article)
            {
                article.Quantity++;
                NotifyPriceChange();
            }
        }

        private bool CanDecreaseQuantity(object? parameter)
        {
            return parameter is Article article && article.Quantity > 1;
        }

        private bool CanIncreaseQuantity(object? parameter)
        {
            return parameter is Article; // Always allow increase unless additional logic is needed
        }

        private void ClearCart()
        {
            AppState.ClearCart();
            CartItems.Clear();
            NotifyPriceChange();
        }

        private void NotifyPriceChange()
        {
            OnPropertyChanged(nameof(TotalPrice));
            OnPropertyChanged(nameof(TotalItemCount));
            OnPropertyChanged(nameof(IsCartEmpty));
        }

        public event PropertyChangedEventHandler? PropertyChanged;
        protected void OnPropertyChanged([CallerMemberName] string? propertyName = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }

    // Classe RelayCommand générique
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
