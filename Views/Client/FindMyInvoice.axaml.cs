using Avalonia.Controls;
using Avalonia.Interactivity;
using Avalonia.Threading;
using KitBox_Project.Helpers;
using System.Threading.Tasks;
using System;

namespace KitBox_Project.Views
{
    public partial class FindMyInvoice : UserControl
    {
        public event EventHandler<RoutedEventArgs>? RetourClicked;

        public FindMyInvoice()
        {
            InitializeComponent();
            Console.WriteLine("🔧 Constructor called");
            DataContext = new ViewModels.FindMyInvoiceViewModel();
        }

        private void OnRetourClick(object? sender, RoutedEventArgs e)
        {
            RetourClicked?.Invoke(this, e);
        }

        // Ajout du gestionnaire d'événement pour le clic sur "Download Invoice"
        private async void OnDownloadInvoiceClick(object sender, RoutedEventArgs e)
        {
            // Démarre la génération de la facture sur un thread de travail séparé
            await Task.Run(() =>
            {
                // Appel à la logique de génération de la facture dans un thread secondaire
                InvoiceGenerator.GenerateSampleInvoice();
            });

            // Une fois la génération terminée, met à jour l'UI sur le thread principal
            await Dispatcher.UIThread.InvokeAsync(() =>
            {
                // Exemple de mise à jour de l'interface utilisateur (bouton)
                var button = (Button)sender;
                button.Content = "Invoice Generated";  // Change le texte du bouton après la génération
            });
        }
    }
}