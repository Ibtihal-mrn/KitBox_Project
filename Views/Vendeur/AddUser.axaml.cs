using Avalonia;
using Avalonia.Controls;
using Avalonia.Markup.Xaml;
using Avalonia.Interactivity;
using System;
using System.Linq;
using System.Text.RegularExpressions;

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

        private void CacherMessages()
        {
            WrongID.IsVisible = false;
            WrongPassword.IsVisible = false;
            DoubleID.IsVisible = false;
        }
        private void AjouterUtilisateur(object sender, RoutedEventArgs e)
        {
            CacherMessages();

            var matricule = MatriculeBox.Text?.Trim();
            var motDePasse = PasswordBox.Text?.Trim();
            var confirmationMotDePasse = ConfirmPasswordBox.Text?.Trim();
            var role = (Rôle.SelectedItem as ComboBoxItem)?.Content?.ToString();

            if (string.IsNullOrWhiteSpace(matricule) || string.IsNullOrWhiteSpace(motDePasse) || string.IsNullOrWhiteSpace(role))
            {
                // Tu peux afficher un message à l'utilisateur ici
                EmptyInformations.IsVisible = true;
                Console.WriteLine("Tous les champs doivent être remplis.");
                return;
            }

            try
            {
                if (!Regex.IsMatch(matricule, @"^\d{5}$"))
                {
                    WrongID.IsVisible = true;
                    Console.WriteLine("Format incorrect");
                    return;
                }

                var utilisateursExistants = Services.DatabaseManager.GetAllUsers();
                if (utilisateursExistants.Any(u => u.Username == matricule))
                {
                    DoubleID.IsVisible = true ; 
                    Console.WriteLine("Ce matricule est déjà utilisé."); 
                    return;
                }

                if(confirmationMotDePasse == motDePasse){
                    KitBox_Project.Services.DatabaseManager.AjouterUtilisateur(matricule, motDePasse, role);
                    Console.WriteLine("Utilisateur ajouté avec succès !");
                    // Tu peux aussi afficher une notification, vider les champs, ou revenir en arrière.
                    MatriculeBox.Text = "";
                    PasswordBox.Text = "";
                    ConfirmPasswordBox.Text = "";
                    Rôle.SelectedIndex = -1;

                    ChargerUtilisateurs();
                }

                else{
                    WrongPassword.IsVisible = true;
                    Console.WriteLine("Les mots de passes ne correspondent pas, veuillez résaayer");
                }

                
                
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Erreur lors de l'ajout de l'utilisateur : {ex.Message}");
            }
        }

        private void ChargerUtilisateurs()
        {
            var users = KitBox_Project.Services.DatabaseManager.GetAllUsers();
            var affichage = users.Select(u => $"Employee ID : {u.Username}   Status :  {u.Role}").ToList();
            ListeUtilisateurs.ItemsSource = affichage;
        }

        private void SupprimerUtilisateur(object sender, RoutedEventArgs e)
        {
            if (sender is Button bouton && bouton.DataContext is string info)
            {
                // On suppose que info = "username | role | password"
                var username = info.Split(':')[1].Split('-')[0].Trim();
                KitBox_Project.Services.DatabaseManager.SupprimerUser(username);
                ChargerUtilisateurs(); // refresh la liste
            }
        }

        




    }
}