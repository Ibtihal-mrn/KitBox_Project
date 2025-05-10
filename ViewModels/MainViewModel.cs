
using ReactiveUI;
using System;
using System.Reactive;
using System.Threading.Tasks;
using MsBox.Avalonia;
using MsBox.Avalonia.Enums;
using KitBox_Project.Data;


namespace KitBox_Project.ViewModels
{
    public class MainViewModel : ReactiveObject // offert par ReactiveUI -> utilisé par Avalonia pour la réactivité)
    {
        private string _title = "Bienvenue sur KitBox !";

        public string Title
        {
            get => _title;
            set => this.RaiseAndSetIfChanged(ref _title, value); // Grâce au binding et à RaiseAndSetIfChanged(), si la valeur de Title change dans le ViewModel, la View (UI) sera mise à jour automatiquement, sans qu'il soit nécessaire d'écrire du code supplémentaire pour rafraîchir l'affichage (chargé par le binding).
        }

        public MainViewModel()
        {
            StartupLoader.LoadArticlesFromDatabase();
        }

    }
} 