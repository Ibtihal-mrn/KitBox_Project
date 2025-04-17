using System;
using Avalonia.Controls;
using Avalonia.Interactivity;

namespace KitBox_Project.Views;

public partial class StudentDiscount : UserControl
{
    public event EventHandler<RoutedEventArgs>? RetourClicked;
    public StudentDiscount()
    {
        InitializeComponent();
    }
    private void OnRetourClick(object? sender, RoutedEventArgs e)
    {
        RetourClicked?.Invoke(this, e);
    }
}