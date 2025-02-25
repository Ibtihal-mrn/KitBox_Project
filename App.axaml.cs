
using Avalonia;
using Avalonia.Controls.ApplicationLifetimes;
using Avalonia.Markup.Xaml;
using KitBox_Project.View;
using KitBox_Project.ViewModel;
using Avalonia.Controls;

namespace KitBox_Project
{
    public partial class App : Application
    {
        public override void Initialize()
        {
            AvaloniaXamlLoader.Load(this);
            DataTemplates.Add(new ViewLocator());
        }

        public override void OnFrameworkInitializationCompleted()
        {
            if (ApplicationLifetime is IClassicDesktopStyleApplicationLifetime desktop)
            {
                // Création d'une nouvelle fenêtre de type Window, avec MainWindow comme contenu
                desktop.MainWindow = new Window
                {
                    // MainWindow est ici un UserControl
                    DataContext = new MainViewModel() // Lier MainViewModel au DataContext
                };
                desktop.MainWindow.Show();
            }

            base.OnFrameworkInitializationCompleted();
        }
    }
}
