
using Avalonia.Controls;
using Avalonia.Interactivity;
using System;

namespace KitBox_Project.Views
{
    public partial class HelpSupport : UserControl
    {
        public event EventHandler<RoutedEventArgs>? PasserCommandeClicked;

        public HelpSupport()
        {
            InitializeComponent();
        }

        private void OnPasserCommandeClicked(object? sender, RoutedEventArgs e)
        {
            PasserCommandeClicked?.Invoke(this, e); // Déclenche l'événement PasserCommandeClicked
        }
    }
}
