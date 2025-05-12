using Avalonia.Controls;
using KitBox_Project.Services;

namespace KitBox_Project.Views
{
    public partial class Home_vendeur : UserControl
    {
        public Home_vendeur()
        {
            InitializeComponent();
        }

        private void GoToChooseUserTypePage(object sender, Avalonia.Interactivity.RoutedEventArgs e)
        {
            if (VisualRoot is MainWindow mw)
            {
                AuthenticationService.Instance.Logout();
                mw.ShowChooseUserTypePage();
            }
        }
    }
}
