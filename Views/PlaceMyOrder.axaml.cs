using Avalonia.Controls;
using Avalonia.Interactivity;

namespace KitBox_Project.Views;

public partial class PlaceMyOrder : UserControl
{
    public PlaceMyOrder()
    {
        InitializeComponent();
    }

    private void GoBack_Click(object? sender, RoutedEventArgs e)
    {
        // On remonte jusqu'à la MainWindow
        if (VisualRoot is MainWindow mainWindow)
        {
            mainWindow.MainContent.Content = new HelpSupport();
        }
    }
}
