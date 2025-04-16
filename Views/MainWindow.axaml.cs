using System;
using Avalonia.Controls;
using Avalonia.Interactivity;
using Views;

namespace KitBox_Project.Views
{
    public partial class MainWindow : Window
    {
        private HelpSupport? _helpPage; // Instance conservée de HelpSupport

        public MainWindow()
        {
            InitializeComponent();

            var homePage = new HomePage();
            homePage.StartClicked += GoToDesignYourWardrobe;
            homePage.HelpClicked += GoToHelpSupport;

            MainContent.Content = homePage;
        }

        private void ToggleMenu(object? sender, RoutedEventArgs e)
        {
            MenuPanel.IsPaneOpen = !MenuPanel.IsPaneOpen;
        }

        private void GoToHome(object? sender, RoutedEventArgs e)
        {
            var homePage = new HomePage();
            homePage.StartClicked += GoToDesignYourWardrobe;
            homePage.HelpClicked += GoToHelpSupport;

            MainContent.Content = homePage;
        }

        private void GoToInspirations(object? sender, RoutedEventArgs e) =>
            MainContent.Content = new Inspirations();

        private void GoToDesignYourWardrobe(object? sender, RoutedEventArgs e) =>
            MainContent.Content = new DesignYourWardrobe();

        private void GoToHelpSupport(object? sender, RoutedEventArgs e)
        {
            if (_helpPage == null)
            {
                _helpPage = new HelpSupport();
                _helpPage.PasserCommandeClicked += GoToPlaceMyOrder;
            }

            MainContent.Content = _helpPage;
        }

        private void GoToPlaceMyOrder(object? sender, RoutedEventArgs e)
        {
            var placeMyOrder = new PlaceMyOrder();
            placeMyOrder.RetourClicked += OnRetourClick; // Utilise ton nom
            MainContent.Content = placeMyOrder;
        }

        private void OnRetourClick(object? sender, RoutedEventArgs e)
        {
            GoToHelpSupport(sender, e);
        }

        private void OnShoppingCartClick(object? sender, RoutedEventArgs e)
        {
            var shoppingCart = new ShoppingCart();
            shoppingCart.HomeClicked += GoToHome; // Lien avec ta méthode existante
            MainContent.Content = shoppingCart;
        }
    }
}
