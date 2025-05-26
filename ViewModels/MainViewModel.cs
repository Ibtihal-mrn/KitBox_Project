using ReactiveUI;
using System;
using System.Threading.Tasks;
using KitBox_Project.Services;
using Avalonia.Interactivity;

namespace KitBox_Project.ViewModels
{
    public class MainViewModel : ReactiveObject
    {
        private string _title = "Bienvenue sur KitBox !";
        private bool _isInitializing = true;
        // Removed unused event StartClicked to resolve warning


         
        public string Title
        {
            get => _title;
            set => this.RaiseAndSetIfChanged(ref _title, value);
        }

        public bool IsInitializing
        {
            get => _isInitializing;
            set => this.RaiseAndSetIfChanged(ref _isInitializing, value);
        }

        public MainViewModel()
        {
            // Lance l'initialisation en arrière-plan
            _ = InitializeAsync();
        }

        private async Task InitializeAsync()
        {
            try
            {
                IsInitializing = true;
                Title = "Chargement en cours...";

                
                // Initialiser le stock de manière asynchrone
                await StockService.InitializeStockAsync();

                Title = "Bienvenue sur KitBox !";
            }
            catch (Exception ex)
            {
                Console.WriteLine($"❌ Erreur initialisation: {ex.Message}");
                Title = "Erreur de chargement";
            }
            finally
            {
                IsInitializing = false;
            }
        }
    }
}