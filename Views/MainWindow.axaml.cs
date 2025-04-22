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
            ShowChooseUserTypePage(); // ✅ un seul point d’entrée


        }

        // Ouvre/ferme le menu proprement
        private void ToggleMenu(object? sender, RoutedEventArgs e)
        {
            MenuPanel.IsPaneOpen = !MenuPanel.IsPaneOpen;
        }

        // Gérer la navigation
        private void GoToHome(object? sender, RoutedEventArgs e)
        {
            var HomePage=new HomePage();
            HomePage.StartClicked += GoToDesignYourWardrobe;
            MainContent.Content = HomePage;
        }
        private void GoToInspirations(object? sender, RoutedEventArgs e) => MainContent.Content = new Inspirations();
        private void GoToDesignYourWardrobe(object? sender, RoutedEventArgs e) => MainContent.Content = new DesignYourWardrobe();
        private void GoToHelpSupport(object? sender, RoutedEventArgs e) => MainContent.Content = new HelpSupport();

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
        }

        private void ShowVendorPage()
        {
            var identification = new Identification();
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
        }

        public void ShowChooseUserTypePage()
        {
            var userTypePage = new ChooseUserTypePage();
            userTypePage.ClientChosen += (_, _) => ShowHomePage();
            userTypePage.VendorChosen += (_, _) => ShowVendorPage();
            MainContent.Content = userTypePage;
            MenuPanel.IsVisible = false;
            MenuButton.IsVisible = false;
            HomeButton.IsVisible = false;
            InspiButton.IsVisible = false;
            DesignButton.IsVisible = false;
            SupportButton.IsVisible = false;
            OrderButton.IsVisible = false;
            StockButton.IsVisible = false;
            UserButton.IsVisible = false;
            ScheduleButton.IsVisible = false;
            QuitButton.IsVisible = false;
        }


    }
}
