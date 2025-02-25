using ReactiveUI;
using System;
using System.Reactive;
using System.Threading.Tasks;
using MsBox.Avalonia;
using MsBox.Avalonia.Enums;
using KitBox_Project.ViewModel;

namespace KitBox_Project.ViewModel
{
    public class MainViewModel : ViewModelBase
    {
        private string _title = "Bienvenue sur KitBox !";
        private ViewModelBase _currentView = new HomePageViewModel(); // Vue par défaut

        public string Title
        {
            get => _title;
            set => this.RaiseAndSetIfChanged(ref _title, value);
        }

        // Propriété publique pour accéder au champ privé
        public ViewModelBase CurrentView
        {
            get => _currentView;
            set => this.RaiseAndSetIfChanged(ref _currentView, value);
        }

        public ReactiveCommand<Unit, Unit> ShowMessageCommand { get; }
        public ReactiveCommand<Unit, Unit> NavigateToHomeCommand { get; }

        public MainViewModel()
        {
            // Charge la page d'accueil au démarrage
            CurrentView = new HomePageViewModel();

            ShowMessageCommand = ReactiveCommand.CreateFromTask(ShowMessage);
            NavigateToHomeCommand = ReactiveCommand.Create(NavigateToHome);
        }

        private void NavigateToHome()
        {
            CurrentView = new HomePageViewModel(); // Chargement de la page d'accueil
        }

        private async Task ShowMessage()
        {
            var messageBox = MessageBoxManager.GetMessageBoxStandard(
                "Information", "Bouton cliqué !", ButtonEnum.Ok, Icon.Info);
            await messageBox.ShowAsync();
        }
    }
}
