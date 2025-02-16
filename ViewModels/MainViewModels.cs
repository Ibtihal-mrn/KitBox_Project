
using ReactiveUI;
using System;
using System.Reactive;
using System.Threading.Tasks;
using MsBox.Avalonia;
using MsBox.Avalonia.Enums;

namespace KitBox_Project.ViewModel
{
    public class MainViewModel : ReactiveObject // offert par ReactiveUI -> utilisé par Avalonia pour la réactivité)
    {
        private string _title = "Bienvenue sur KitBox !";

        public string Title
        {
            get => _title;
            set => this.RaiseAndSetIfChanged(ref _title, value); // Grâce au binding et à RaiseAndSetIfChanged(), si la valeur de Title change dans le ViewModel, la View (UI) sera mise à jour automatiquement, sans qu'il soit nécessaire d'écrire du code supplémentaire pour rafraîchir l'affichage (chargé par le binding).
        }

        public ReactiveCommand<Unit, Unit> ShowMessageCommand { get; }

        public MainViewModel()
        {
            // Initialisation de la commande avec une action qui affiche un message
            ShowMessageCommand = ReactiveCommand.CreateFromTask(ShowMessage);
        }

        private async Task ShowMessage()
        {
            var messageBox = MessageBoxManager.GetMessageBoxStandard("Information", "Bouton cliqué !", ButtonEnum.Ok, Icon.Info);
            await messageBox.ShowAsync();
        }
    }
} 