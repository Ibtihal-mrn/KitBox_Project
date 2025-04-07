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
        }

        private void ShowVendorPage()
        {
            var identification = new Identification();
            MainContent.Content = identification;
        }

        public void ShowChooseUserTypePage()
        {
            var userTypePage = new ChooseUserTypePage();
            userTypePage.ClientChosen += (_, _) => ShowHomePage();
            userTypePage.VendorChosen += (_, _) => ShowVendorPage();
            MainContent.Content = userTypePage;
        }


    }
}
