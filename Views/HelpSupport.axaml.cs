using Avalonia;
using Avalonia.Controls;
using Avalonia.Markup.Xaml;
using Avalonia.Interactivity;

namespace KitBox_Project.Views;

public partial class HelpSupport : UserControl
{
    public HelpSupport()
    {
        InitializeComponent();
    }

    private void GoToTestPage(object? sender, RoutedEventArgs e)
    {
        // Accès à la fenêtre principale
        var mainWindow = VisualRoot as MainWindow;
        if (mainWindow != null)
        {
            // Changement de la vue
            mainWindow.MainContent.Content = new TestConnexionInverse();
        }
    }
}
