using Avalonia.Controls;
using Avalonia.Markup.Xaml;
using Avalonia.Interactivity;
using Avalonia.Threading; // Ajout nécessaire
using KitBox_Project.ViewModels;

namespace KitBox_Project.Views
{
    public partial class ConsoleWindow : Window
    {
        private ConsoleViewModel ViewModel { get; }

        public ConsoleWindow()
        {
            InitializeComponent();
            ViewModel = new ConsoleViewModel();
            ViewModel.AppendText = AppendText;
            DataContext = ViewModel;
        }

        public void OnLoadArticlesClick(object sender, RoutedEventArgs e)
        {
            ViewModel.LoadArticles();
        }

        public void AppendText(string message)
        {
            Dispatcher.UIThread.InvokeAsync(() =>
            {
                if (this.FindControl<TextBox>("ConsoleOutput") is TextBox consoleOutput)
                {
                    consoleOutput.Text += "\n" + message;
                    consoleOutput.CaretIndex = consoleOutput.Text.Length; // Scroll automatique
                }
            });
        }
    }
}
