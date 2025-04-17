using System;
using Avalonia.Controls;
using Avalonia.Interactivity;

namespace KitBox_Project.Views;

public partial class Delivery : UserControl
{
    public event EventHandler<RoutedEventArgs>? RetourClicked;
    public Delivery()
    {
        InitializeComponent();
    }
    private void OnRetourClick(object? sender, RoutedEventArgs e)
    {
        RetourClicked?.Invoke(this, e);
    }
}