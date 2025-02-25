using ReactiveUI;

namespace KitBox_Project.ViewModel
{
    public class HomePageViewModel : ViewModelBase
    {
        public string WelcomeMessage => "Bienvenue sur la page d'accueil !";

        // Pas besoin de créer une nouvelle instance ici
        public HomePageViewModel()
        {
            // Initialisation si nécessaire
        }
    }
}
