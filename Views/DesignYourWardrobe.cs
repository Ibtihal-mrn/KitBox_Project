using Avalonia;
using Avalonia.Controls;
using Avalonia.Markup.Xaml;
using Avalonia.Interactivity;

namespace KitBox_Project.Views
{
    public partial class DesignYourWardrobe : UserControl
    {
        public DesignYourWardrobe()
        {
            InitializeComponent();
        }

        // Gestionnaire d'événement pour le bouton "Next"
        

        private void GoToHeightWindow(object sender, RoutedEventArgs e)
        {
            var mainWindow = (MainWindow)VisualRoot; // Récupère la fenêtre principale
            mainWindow.MainContent.Content = new HelpSupport(); // ✅ Modifie le bon ContentControl
        }

    }
}
