using ReactiveUI;
using System;
using System.Reactive;
using System.Threading.Tasks;
using MsBox.Avalonia;
using MsBox.Avalonia.Enums;
using KitBox_Project.Data;
using KitBox_Project.Services; // Ajout pour accéder à StockService

namespace KitBox_Project.ViewModels
{
    public class MainViewModel : ReactiveObject
    {
        private string _title = "Bienvenue sur KitBox !";

        public string Title
        {
            get => _title;
            set => this.RaiseAndSetIfChanged(ref _title, value);
        }

        public MainViewModel()
        {
            StartupLoader.LoadArticlesFromDatabase(); // Charge les articles depuis la base de données
            StockService.LoadConfirmedOrdersAndAdjustStock(); // 🔁 Ajuste le stock en fonction du fichier JSON
        }
    }
}
