using System;
using Avalonia.Controls;
using Avalonia.Interactivity;

namespace KitBox_Project.Views;

public partial class FindMyInvoice : UserControl
{
    public event EventHandler<RoutedEventArgs>? RetourClicked;
    public FindMyInvoice()
    {
        InitializeComponent();
        DataContext = new KitBox_Project.ViewModels.FindMyInvoiceViewModel();
    }
    private void OnRetourClick(object? sender, RoutedEventArgs e)
    {
        RetourClicked?.Invoke(this, e);
    }
}