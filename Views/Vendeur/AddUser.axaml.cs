using Avalonia;
using Avalonia.Controls;
using Avalonia.Markup.Xaml;
using Avalonia.Interactivity;
using System;
using System.Linq;

namespace KitBox_Project.Views
{
    public partial class AddUser : UserControl
    {
        public AddUser()
        {
            InitializeComponent();
            ChargerUtilisateurs();
        }

        // Gestionnaire d'événement pour le bouton "Next"
        private void GoToHomeVendeur(object sender, RoutedEventArgs e)
        {
            if (VisualRoot is MainWindow mainWindow) // Vérifie si VisualRoot est une MainWindow
            {
                mainWindow.MainContent.Content = new Home_vendeur(); // ✅ Modifie le bon ContentControl
            }
        }

        private void AjouterUtilisateur(object sender, RoutedEventArgs e)
        {
            var matricule = MatriculeBox?.Text?.Trim() ?? string.Empty;
            var motDePasse = PasswordBox?.Text?.Trim() ?? string.Empty;
            var role = (Rôle.SelectedItem as ComboBoxItem)?.Content?.ToString();

            if (string.IsNullOrWhiteSpace(matricule) || string.IsNullOrWhiteSpace(motDePasse) || string.IsNullOrWhiteSpace(role))
            {
                // Tu peux afficher un message à l'utilisateur ici
                Console.WriteLine("Tous les champs doivent être remplis.");
                return;
            }

            try
            {
                KitBox_Project.Services.DatabaseManager.AjouterUtilisateur(matricule, motDePasse, role);
                Console.WriteLine("Utilisateur ajouté avec succès !");
                // Tu peux aussi afficher une notification, vider les champs, ou revenir en arrière.
                if (MatriculeBox != null)
                {
                    MatriculeBox.Text = "";
                }
                if (PasswordBox != null)
                {
                    PasswordBox.Text = "";
                }
                Rôle.SelectedIndex = -1;

                ChargerUtilisateurs();
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Erreur lors de l'ajout de l'utilisateur : {ex.Message}");
            }
        }

        private void ChargerUtilisateurs()
        {
            var users = KitBox_Project.Services.DatabaseManager.GetAllUsers();
            var affichage = users.Select(u => $"{u.Username} | {u.Role} | {u.Password}").ToList();
            ListeUtilisateurs.ItemsSource = affichage;
        }

        private void SupprimerUtilisateur(object sender, RoutedEventArgs e)
        {
            if (sender is Button bouton && bouton.DataContext is string info)
            {
                // On suppose que info = "username | role | password"
                var username = info.Split(" | ")[0];
                KitBox_Project.Services.DatabaseManager.SupprimerUser(username);
                ChargerUtilisateurs(); // refresh la liste
            }
        }

        




    }
}