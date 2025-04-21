using Avalonia;
using Avalonia.Controls;
using Avalonia.Markup.Xaml;
using System;
using Avalonia.Interactivity;

namespace Views;

public partial class ShoppingCart : UserControl
{
    public event EventHandler<RoutedEventArgs>? HomeClicked; // Déclare un événement
    public ShoppingCart()
    {
        InitializeComponent();
    }

    private void GoToHome(object? sender, RoutedEventArgs e)
        {
            HomeClicked?.Invoke(this, new RoutedEventArgs()); // Déclenche l'événement
        }
}
