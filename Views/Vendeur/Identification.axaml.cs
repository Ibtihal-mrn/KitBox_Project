using Avalonia;
using Avalonia.Controls;
using Avalonia.Markup.Xaml;
using Avalonia.Interactivity;
using KitBox_Project.Data;
using System;


namespace KitBox_Project.Views
{
    public partial class Identification : UserControl
    {
        public Identification()
        {
            InitializeComponent();
            DatabaseManager.InitializeDatabase();
            DatabaseManager.InsertTestUser(); //uniquement pour le test
        }

        // Gestionnaire d'événements pour les boutons

        private void GoToFirstPage(object sender, RoutedEventArgs e)
        {
            if (VisualRoot is MainWindow mainWindow)
            {
                mainWindow.ShowChooseUserTypePage(); // ✅ les événements sont rebranchés ici
            }
        }

    
        private void GoToHomeVendeur(object sender, RoutedEventArgs e)
        {
            string matricule = MatriculeBox.Text?.Trim() ?? "";
            string motDePasse = PasswordBox.Text?.Trim() ?? "";

            string? role = DatabaseManager.AuthenticateUser(matricule, motDePasse);

            if (role == "vendeur"){
                if (VisualRoot is MainWindow mainWindow) // Vérifie si VisualRoot est une MainWindow
                {
                    mainWindow.MainContent.Content = new Home_vendeur(); // ✅ Modifie le bon ContentControl
                }
            }

            else if (role == "superviseur"){
                if (VisualRoot is MainWindow mainWindow) // Vérifie si VisualRoot est une MainWindow
                {
                    mainWindow.MainContent.Content = new Home_vendeur(); // ✅ Modifie le bon ContentControl
                }
            }
            else {
                //afficher un message d'erreur simple
                Console.WriteLine("Identifiants incorrects!");
            }
            
        }
        

        //private void GoToFirstPage(object? sender, RoutedEventArgs e) => MainContent.Content = new ChooseUserTypePage();

        
    }
}
