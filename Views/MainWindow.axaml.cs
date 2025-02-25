
using System;
using Avalonia.Controls;
using Avalonia.Interactivity;

namespace KitBox_Project.Views
{
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            InitializeComponent();
        }

        // ✅ Ouvre/ferme le menu proprement
        private void ToggleMenu(object? sender, RoutedEventArgs e)
        {
            MenuPanel.IsPaneOpen = !MenuPanel.IsPaneOpen;
        }

        // ✅ Gérer la navigation
        private void GoToHome(object? sender, RoutedEventArgs e) => MainContent.Content = new HomePage();
        private void GoToOrder(object? sender, RoutedEventArgs e) => MainContent.Content = new Order();
        private void GoToOrderTracking(object? sender, RoutedEventArgs e) => MainContent.Content = new OrderTracking();
        private void GoToLockerConfiguration(object? sender, RoutedEventArgs e) => MainContent.Content = new LockerConfiguration();
        private void GoToHelpSupport(object? sender, RoutedEventArgs e) => MainContent.Content = new HelpSupport();
    }
}
