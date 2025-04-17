
using Avalonia.Controls;
using Avalonia.Interactivity;
using System;
using Views;

namespace KitBox_Project.Views
{
    public partial class HelpSupport : UserControl
    {
        public event EventHandler<RoutedEventArgs>? PasserCommandeClicked;
        public event EventHandler<RoutedEventArgs>? StudentDiscountClicked;
        public event EventHandler<RoutedEventArgs>? FindMyInvoiceClicked;
        public event EventHandler<RoutedEventArgs>? DeliveryClicked;

        public HelpSupport()
        {
            InitializeComponent();
        }

        private void OnSearchClicked(object? sender, RoutedEventArgs e)
        {
            var keyword = SearchBox.Text?.Trim();

            if (!string.IsNullOrEmpty(keyword))
            {
                // Ici tu peux gérer la logique de recherche : filtrer, rediriger, afficher un message, etc.
                Console.WriteLine($"Mot-clé recherché : {keyword}");
                // Exemple : Affiche un message temporaire (si tu veux en faire plus tard)
            }
            else
            {
                Console.WriteLine("Le champ de recherche est vide.");
            }
        }

        private void OnPasserCommandeClicked(object? sender, RoutedEventArgs e)
        {
            PasserCommandeClicked?.Invoke(this, e); // Déclenche l'événement PasserCommandeClicked
        }
        private void OnStudentDiscountClicked(object? sender, RoutedEventArgs e)
        {
            StudentDiscountClicked?.Invoke(this, e); // Déclenche l'événement StudentDiscountClicked
        }
        private void OnFindMyInvoiceClicked(object? sender, RoutedEventArgs e)
        {
            FindMyInvoiceClicked?.Invoke(this, e);
        }
        private void OnDeliveryClicked(object? sender, RoutedEventArgs e)
        {
            DeliveryClicked?.Invoke(this, e);
        }
    }
}
