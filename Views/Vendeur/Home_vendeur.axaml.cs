using Avalonia;
using Avalonia.Controls;
using Avalonia.Markup.Xaml;
using Avalonia.Interactivity;

namespace KitBox_Project.Views
{
    public partial class Home_vendeur : UserControl
    {
        public Home_vendeur()
        {
            InitializeComponent();
        }

        private void GoToAddUser(object sender, RoutedEventArgs e)
        {
            if (VisualRoot is MainWindow mainWindow) // Vérifie si VisualRoot est une MainWindow
            {
                mainWindow.MainContent.Content = new AddUser(); // ✅ Modifie le bon ContentControl
            }
        }

        

        private void GoToFirstPage(object sender, RoutedEventArgs e)
        {
            if (VisualRoot is MainWindow mainWindow)
            {
                mainWindow.ShowChooseUserTypePage(); // ✅ les événements sont rebranchés ici
            }
        }

        

        private void GoToCalendar(object sender, RoutedEventArgs e)
        {
            if (VisualRoot is MainWindow mainWindow)
            {
                mainWindow.MainContent.Content = new WeeklyCalendar(); // ✅ les événements sont rebranchés ici
            }
        }
        
    }
}
