using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows.Input;
using ReactiveUI;

namespace KitBox_Project.Models
{
    public class ConfirmedOrder : ReactiveObject
    {
        public string OrderId { get; set; }
        public List<Article> Articles { get; set; }
        
        private DateTime? _paidDate;
        public DateTime? PaidDate
        {
            get => _paidDate;
            set 
            { 
                this.RaiseAndSetIfChanged(ref _paidDate, value);
                // Notifier que IsPaid a aussi changé
                this.RaisePropertyChanged(nameof(IsPaid));
            }
        }
        
        public bool IsPaid => PaidDate.HasValue;

        // Ajout de la commande directement dans le modèle
        public ICommand? PayCommand { get; set; }

        public ConfirmedOrder(string orderId)
        {
            OrderId = orderId;
            Articles = new List<Article>();
        }

        public int TotalQuantity => Articles?.Sum(a => a.Quantity) ?? 0;
        public decimal TotalPrice => Articles?.Sum(a => a.TotalPrice) ?? 0m;
    }
}