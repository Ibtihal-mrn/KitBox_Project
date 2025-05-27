using Avalonia.Controls;
using Avalonia.Interactivity;
using KitBox_Project.Services;

namespace KitBox_Project.Views.Vendeur
{
    public partial class Home_vendeur : UserControl
    {
        public Home_vendeur()
        {
            InitializeComponent();
        }

        private void GoToChooseUserTypePage(object sender, RoutedEventArgs e)
        {
            if (VisualRoot is MainWindow mw)
            {
                AuthenticationService.Instance.Logout();
                mw.ShowChooseUserTypePage();
            }
        }
    }
}