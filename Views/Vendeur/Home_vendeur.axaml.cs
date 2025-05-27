using System;
using Avalonia.Controls;
using Avalonia.Interactivity;
using KitBox_Project.Models;
using KitBox_Project.Services;
using KitBox_Project.ViewModels;

namespace KitBox_Project.Views.Vendeur
{
    public partial class Home_vendeur : UserControl
    {
        public Home_vendeur()
        {
            InitializeComponent();
            DataContext = new SellerViewModel();
        }

        private void GoToChooseUserTypePage(object sender, RoutedEventArgs e)
        {
            if (VisualRoot is MainWindow mw)
            {
                AuthenticationService.Instance.Logout();
                mw.ShowChooseUserTypePage();
            }
        }
        private void PayButton_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                if (sender is Button button && button.Tag is ConfirmedOrder order)
                {
                    if (this.DataContext is SellerViewModel viewModel)
                    {
                        viewModel.MarkOrderAsPaid(order);
                    }
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"ERREUR dans PayButton_Click: {ex.Message}");
            }
        }
    }
}