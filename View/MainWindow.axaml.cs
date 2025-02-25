
using Avalonia.Controls;
using Avalonia.Interactivity;
using KitBox_Project.ViewModel;

namespace KitBox_Project.View;

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

    // Affiche la page Settinfs
    private void GoToSettings(object? sender, RoutedEventArgs e)
    {
        MainContent.Content = new SettingsPage();
    }
}