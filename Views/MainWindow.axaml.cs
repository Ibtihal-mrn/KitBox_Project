
using Avalonia.Controls;
using Avalonia.Interactivity;

namespace KitBox_Project.Views;

public partial class MainWindow : Window
{
    public MainWindow()
    {
        InitializeComponent();
    }

    // Ouvre/ferme le menu coulissant
    private void ToggleMenu(object? sender, RoutedEventArgs e)
    {
        MenuPanel.IsPaneOpen = !MenuPanel.IsPaneOpen;
    }

    // Affiche la page Home
    private void GoToHome(object? sender, RoutedEventArgs e)
    {
        MainContent.Content = new HomePage();
    }

    // Affiche la page Order
    private void GoToOrder(object? sender, RoutedEventArgs e)
    {
        MainContent.Content = new Order();
    }

    // Affichage la page Order Tracking
    private void GoToOrderTracking(object? sender, RoutedEventArgs e)
    { MainContent.Content = new OrderTracking();
    }

    // Affichage de la page Locker Configuration
    private void GoToLockerConfiguration(object? sender, RoutedEventArgs e)
    { MainContent.Content = new LockerConfiguration();
    }

    // Affichage de la page Help Support
    private void GoToHelpSupport(object? sender, RoutedEventArgs e)
    { MainContent.Content = new HelpSupport();
    }
}