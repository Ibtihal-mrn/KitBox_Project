using Avalonia.Controls;
using Avalonia.Interactivity;
using System;

namespace KitBox_Project.Views
{
    public partial class ChooseUserTypePage : UserControl
    {
        public event EventHandler? ClientChosen;
        public event EventHandler? VendorChosen;

        public ChooseUserTypePage()
        {
            InitializeComponent();
        }

        private void OnClientClicked(object? sender, RoutedEventArgs e)
        {
            ClientChosen?.Invoke(this, EventArgs.Empty);
        }

        private void OnVendorClicked(object? sender, RoutedEventArgs e)
        {
            VendorChosen?.Invoke(this, EventArgs.Empty);
        }
    }
}
