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
            var homePage = new HomePage();
            homePage.StartClicked += GoToDesignYourWardrobe; // Abonnement à l'événement StartClicked
            homePage.HelpClicked += GoToHelpSupport; // Abonnement à l'événement HelpClicked
    
            MainContent.Content = homePage; // Charger la page d'accueil
        }

        // Ouvre/ferme le menu proprement
        private void ToggleMenu(object? sender, RoutedEventArgs e)
        {
            MenuPanel.IsPaneOpen = !MenuPanel.IsPaneOpen;
        }

        // Gérer la navigation
        private void GoToHome(object? sender, RoutedEventArgs e)
        {
            var HomePage = new HomePage();
            HomePage.StartClicked += GoToDesignYourWardrobe;
            HomePage.HelpClicked += GoToHelpSupport; // Assurer que l'événement HelpClicked est aussi abonné
            MainContent.Content = HomePage;
        }

        private void GoToInspirations(object? sender, RoutedEventArgs e) => MainContent.Content = new Inspirations();
        private void GoToDesignYourWardrobe(object? sender, RoutedEventArgs e) => MainContent.Content = new DesignYourWardrobe();

        private void GoToHelpSupport(object? sender, RoutedEventArgs e)
        {
            var helpPage = new HelpSupport();
            helpPage.PasserCommandeClicked += GoToPlaceMyOrder; // Abonnement à l'événement PasserCommandeClicked
            MainContent.Content = helpPage; // Charger la page HelpSupport
        }

        private void GoToPlaceMyOrder(object? sender, RoutedEventArgs e)
        {
            MainContent.Content = new PlaceMyOrder(); // Charger la page PlaceMyOrder
        }
    }
}
