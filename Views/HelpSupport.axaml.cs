
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

        private void OnSearchClicked(object? sender, RoutedEventArgs e)
        {
            var keyword = SearchBox.Text?.Trim();

            if (!string.IsNullOrEmpty(keyword))
            {
                // Ici tu peux gérer la logique de recherche : filtrer, rediriger, afficher un message, etc.
                Console.WriteLine($"Mot-clé recherché : {keyword}");
                // Exemple : Affiche un message temporaire (si tu veux en faire plus tard)
            }
            else
            {
                Console.WriteLine("Le champ de recherche est vide.");
            }
        }
    }
}
