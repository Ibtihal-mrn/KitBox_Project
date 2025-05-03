using Avalonia.Controls;
using Avalonia.Interactivity;
using System;
using Avalonia;
using KitBox_Project.Services;

namespace KitBox_Project.Views
{
    public partial class LoginDialog : Window
    {
        public LoginDialog()
        {
            InitializeComponent();
        }

        private void OnLoginClicked(object? sender, RoutedEventArgs e)
        {
            string username = UsernameBox.Text ?? "";
            string password = PasswordBox.Text ?? "";

            if (AuthenticationService.Instance.Login(username, password))
            {
                Close(true); // Succès
            }
            else
            {
                ErrorText.IsVisible = true; // Pour rendre visible l'erreur
            }
        }
    }
}
