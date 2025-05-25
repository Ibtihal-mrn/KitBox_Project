using System;
using Avalonia.Controls;
using Avalonia.Interactivity;
using Avalonia.Markup.Xaml;
using KitBox_Project.Services;
using KitBox_Project.ViewModels;

namespace KitBox_Project.Views.Client
{
    public partial class Confirmation : UserControl
    {
        public Confirmation()
        {
            // Charge le XAML généré
            InitializeComponent();
            LoadConfirmation();
        }

        // Méthode générée manuellement pour Avalonia <= v11
        private void InitializeComponent()
        {
            AvaloniaXamlLoader.Load(this);
        }

        private void LoadConfirmation()
        {
            var lastOrder = ConfirmedOrderService.GetLastConfirmedOrder();
            if (lastOrder == null)
                return;

            var vm = new ConfirmationViewModel
            {
                OrderId   = lastOrder.OrderId,
                OrderDate = DateTime.Now
            };

            foreach (var art in lastOrder.Articles)
            {
                vm.Items.Add(new ConfirmedOrderItemViewModel
                {
                    Reference    = art.Reference ?? "",
                    Color        = art.Color ?? "",
                    Dimensions   = art.Dimensions ?? "",
                    SellingPrice = art.SellingPrice,
                    Quantity     = art.Quantity
                });
            }

            DataContext = vm;
        }

        private void GoToFirstPage(object sender, RoutedEventArgs e)
        {
            if (VisualRoot is MainWindow mainWindow)
                mainWindow.ShowChooseUserTypePage();
        }
    }
}
