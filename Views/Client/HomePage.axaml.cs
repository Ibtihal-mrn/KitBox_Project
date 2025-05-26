using Avalonia;
using Avalonia.Controls;
using Avalonia.Markup.Xaml;
using System;
using System.Timers;
using Avalonia.Interactivity;
using KitBox_Project.Services;
using KitBox_Project.Data;

namespace KitBox_Project.Views
{
    public partial class HomePage : UserControl
    {
        private double _position = -300;
        private Timer? _timer;

        public event EventHandler<RoutedEventArgs>? StartClicked;
        public event EventHandler<RoutedEventArgs>? HelpClicked;

        public HomePage()
        {
            InitializeComponent();
            StartAnimation();
        }

        private void StartAnimation()
        {
            var movingText = this.FindControl<TextBlock>("MovingText");
            if (movingText == null) return;

            _timer?.Stop();
            _timer = new Timer(20);
            _timer.Elapsed += (s, e) =>
            {
                Avalonia.Threading.Dispatcher.UIThread.Post(() =>
                {
                    _position += 3;
                    if (_position > this.Bounds.Width)
                        _position = -movingText.Bounds.Width;
                    movingText.RenderTransform = new Avalonia.Media.TranslateTransform(_position, 30);
                });
            };
            _timer.Start();
        }

        
         private async void GoToColor(object sender, RoutedEventArgs e)
        {
            try
            {
                //StockService.ResetInitializationFlag();
                // Ne pas vider le panier
                //StaticArticleDatabase.AllArticles.Clear();
                await StockService.InitializeStockAsync();
                StartClicked?.Invoke(this, new RoutedEventArgs());
            }
            catch(Exception ex)
            {
                Console.WriteLine(ex.Message);
            }
        }
        private void GoToHelpSupport(object? sender, RoutedEventArgs e)
            => HelpClicked?.Invoke(this, new RoutedEventArgs());

        private void GoToFirstPage(object sender, RoutedEventArgs e)
        {
            if (VisualRoot is MainWindow mainWindow)
                mainWindow.ShowChooseUserTypePage();
        }
    }
}
