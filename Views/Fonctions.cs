using Avalonia.Controls;

namespace KitBox_Project
{
    public static class Fonctions
    {
        // Méthode générique pour naviguer entre deux fenêtres
        public static void NavigateToPage(Window currentWindow, Window targetWindow)
        {
            targetWindow.Show();    // Affiche la nouvelle fenêtre
            currentWindow.Close();  // Ferme la fenêtre actuelle
        }

        // D'autres méthodes génériques pour la navigation ou des actions communes
    }
}
