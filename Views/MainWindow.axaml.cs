using System;
using Avalonia.Controls;
using Avalonia.Interactivity;
using KitBox_Project.Services ; 

namespace KitBox_Project.Views
{
    public partial class MainWindow : Window
    {
        private HelpSupport? _helpPage;
        public MainWindow()
        {
            InitializeComponent();
            ShowChooseUserTypePage(); // ✅ un seul point d’entrée
            this.Show();
         


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

        private void GoToAddUser(object? sender, RoutedEventArgs e) => MainContent.Content = new AddUser();

        private void GoToCalendar(object? sender, RoutedEventArgs e) => MainContent.Content = new WeeklyCalendar();






        private void ShowHomePage()
        {
            var homePage = new HomePage();
            homePage.StartClicked += GoToDesignYourWardrobe;
            MainContent.Content = homePage;
            MenuPanel.IsVisible = true;
            MenuButton.IsVisible = true;
            HomeButton.IsVisible = true;
            InspiButton.IsVisible = true;
            DesignButton.IsVisible = true;
            SupportButton.IsVisible = true;
            OrderButton.IsVisible = false;
            StockButton.IsVisible = false;
            UserButton.IsVisible = false;
            ScheduleButton.IsVisible = false;
            QuitButton.IsVisible = false;
            PanierButton.IsVisible = true ; 
        }

        private void ShowVendor(){
            if (AuthenticationService.Instance.IsAuthenticated)
            {
                // Déjà connecté → montrer interface vendeur directement
                ShowVendorPage();
            }
            else
            {
                // Pas connecté → demander login
                ShowLoginDialog();
            }
        }

        private async void ShowLoginDialog()
        {
            var dialog = new LoginDialog(); // Fenêtre modale
            var result = await dialog.ShowDialog<bool>(this); // Passer 'this' pour désigner la fenêtre parente

            if (result)
            {
                ShowVendorPage(); // connecté, on peut y aller
            }
        }

        private void ShowVendorPage()
        {
            var identification = new Home_vendeur();
            MainContent.Content = identification;
            MenuPanel.IsVisible = true;
            MenuButton.IsVisible = true;
            HomeButton.IsVisible = false;
            InspiButton.IsVisible = false;
            DesignButton.IsVisible = false;
            SupportButton.IsVisible = false;
            OrderButton.IsVisible = true;
            StockButton.IsVisible = true;
            UserButton.IsVisible = true;
            ScheduleButton.IsVisible = true;
            QuitButton.IsVisible = true;
            PanierButton.IsVisible = false ; 
        }

        public void ShowChooseUserTypePage()
        {
            var userTypePage = new ChooseUserTypePage();
            userTypePage.ClientChosen += (_, _) => ShowHomePage();
            userTypePage.VendorChosen += (_, _) => ShowVendor();
            MainContent.Content = userTypePage;

            MenuPanel.IsVisible = false;
            MenuButton.IsVisible = false;
            HomeButton.IsVisible = false;
            InspiButton.IsVisible = false;
            DesignButton.IsVisible = false;
            SupportButton.IsVisible = false;
            PanierButton.IsVisible = false ; 

            OrderButton.IsVisible = false;
            StockButton.IsVisible = false;
            UserButton.IsVisible = false;
            ScheduleButton.IsVisible = false;
            QuitButton.IsVisible = false;
        }


    }
}
