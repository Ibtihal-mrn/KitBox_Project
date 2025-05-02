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
                var userTypePage = new ChooseUserTypePage();
                userTypePage.ClientChosen += (_, _) => ShowHomePage();
                userTypePage.VendorChosen += (_, _) => ShowVendorPage();
                MainContent.Content = userTypePage;

        }

        private void ToggleMenu(object? sender, RoutedEventArgs e)
        {
            MenuPanel.IsPaneOpen = !MenuPanel.IsPaneOpen;
        }

        private void GoToHome(object? sender, RoutedEventArgs e)
        {
            var homePage = new HomePage();
            homePage.StartClicked += GoToColor;
            homePage.HelpClicked += GoToHelpSupport;

            MainContent.Content = homePage;
        }

        private void GoToInspirations(object? sender, RoutedEventArgs e) =>
            MainContent.Content = new Inspirations();

        private void GoToColor(object? sender, RoutedEventArgs e) =>
            MainContent.Content = new Color();

        private void GoToHelpSupport(object? sender, RoutedEventArgs e)
        {
            if (_helpPage == null)
            {
                _helpPage = new HelpSupport();
                _helpPage.PasserCommandeClicked += GoToPlaceMyOrder;
                _helpPage.StudentDiscountClicked += GoToStudentDiscount; // si tu n'as pas cette ligne, en cliquant tu n'accèdes pas à la page même si tu as tout bien configuré, que tout est relié ...
                _helpPage.FindMyInvoiceClicked += GoToFindMyInvoice;
                _helpPage.DeliveryClicked += GoToDelivery;
            }

            MainContent.Content = _helpPage;
        }

        private void OnShoppingCartClick(object? sender, RoutedEventArgs e)
        {
            var shoppingCart = new ShoppingCart();
            shoppingCart.HomeClicked += GoToHome; // Lien avec ta méthode existante
            MainContent.Content = shoppingCart;
        }

        private void GoToPlaceMyOrder(object? sender, RoutedEventArgs e)
        {
            var placeMyOrder = new PlaceMyOrder();
            placeMyOrder.RetourClicked += OnRetourClick;
            MainContent.Content = placeMyOrder;
        }
        private void GoToStudentDiscount(object? sender, RoutedEventArgs e)
        {
            var studentDiscount = new StudentDiscount();
            studentDiscount.RetourClicked += OnRetourClick;
            MainContent.Content = studentDiscount;
        }

        private void GoToFindMyInvoice(object? sender, RoutedEventArgs e)
        {
            var findMyInvoice = new FindMyInvoice();
            findMyInvoice.RetourClicked += OnRetourClick;
            MainContent.Content = findMyInvoice;
        }

        private void GoToDelivery(object? sender, RoutedEventArgs e)
        {
            var delivery = new Delivery();
            delivery.RetourClicked += OnRetourClick;
            MainContent.Content = delivery;
        }

        private void OnRetourClick(object? sender, RoutedEventArgs e)
        {
            GoToHelpSupport(sender, e);
        }
        private void GoToInspirations(object? sender, RoutedEventArgs e) => MainContent.Content = new Inspirations();
        private void GoToDesignYourWardrobe(object? sender, RoutedEventArgs e) => MainContent.Content = new DesignYourWardrobe();
        private void GoToHelpSupport(object? sender, RoutedEventArgs e) => MainContent.Content = new HelpSupport();

        private void ShowHomePage()
        {
            var homePage = new HomePage();
            homePage.StartClicked += GoToDesignYourWardrobe;
            MainContent.Content = homePage;
        }

        private void ShowVendorPage()
        {
            MainContent.Content = new TextBlock
            {
                Text = "Interface vendeur à venir...",
                FontSize = 24,
                HorizontalAlignment = Avalonia.Layout.HorizontalAlignment.Center,
                VerticalAlignment = Avalonia.Layout.VerticalAlignment.Center
            };
        }

    }
}
