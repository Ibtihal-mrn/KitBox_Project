
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
            MainContent.Content = new HomePage(); /* MainContent.Content : Cela fait référence à l'élément de type ContentControl dans ton XAML, où tu veux afficher ta page. new HomePage() : Crée une instance de la page HomePage pour l'afficher dans le ContentControl.*/
        }

        // Ouvre/ferme le menu proprement
        private void ToggleMenu(object? sender, RoutedEventArgs e)
        {
            MenuPanel.IsPaneOpen = !MenuPanel.IsPaneOpen;
        }

        // Gérer la navigation
        private void GoToHome(object? sender, RoutedEventArgs e) => MainContent.Content = new HomePage();
        private void GoToInspirations(object? sender, RoutedEventArgs e) => MainContent.Content = new Inspirations();
        private void GoToDesignYourWardrobe(object? sender, RoutedEventArgs e) => MainContent.Content = new DesignYourWardrobe();
        private void GoToHelpSupport(object? sender, RoutedEventArgs e) => MainContent.Content = new HelpSupport();
        private void GoToHeightWindow(object? sender, RoutedEventArgs e) => MainContent.Content = new HelpSupport();
    }
}
