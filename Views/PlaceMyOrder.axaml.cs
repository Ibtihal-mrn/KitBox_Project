using System;
using Avalonia.Controls;
using Avalonia.Interactivity;

namespace KitBox_Project.Views;

public partial class PlaceMyOrder : UserControl
{
    public event EventHandler<RoutedEventArgs>? RetourClicked;
    public PlaceMyOrder()
    {
        InitializeComponent();
    }

    private void OnRetourClick(object? sender, RoutedEventArgs e)
    {
        RetourClicked?.Invoke(this, e);
    }
}
