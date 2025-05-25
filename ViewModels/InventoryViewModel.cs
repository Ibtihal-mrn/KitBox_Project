// File: ViewModels/InventoryViewModel.cs
using KitBox_Project.Data;
using KitBox_Project.Models;
using KitBox_Project.Services;
using MsBox.Avalonia.ViewModels.Commands;
using System;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Threading.Tasks;
using System.Windows.Input;

namespace KitBox_Project.ViewModels
{
    public class InventoryViewModel : INotifyPropertyChanged
    {
        public ObservableCollection<Article> Articles { get; } = new();

        private bool _isLoading = true;
        public bool IsLoading
        {
            get => _isLoading;
            set
            {
                if (_isLoading != value)
                {
                    _isLoading = value;
                    OnPropertyChanged();
                    OnPropertyChanged(nameof(NoArticlesVisible));
                    OnPropertyChanged(nameof(ArticlesGridVisible));
                }
            }
        }

        public bool NoArticlesVisible => !IsLoading && Articles.Count == 0;
        public bool ArticlesGridVisible => !IsLoading && Articles.Count > 0;

        public ICommand SaveArticlesCommand { get; }

        public InventoryViewModel()
        {
            SaveArticlesCommand = new RelayCommand(async _ => await SaveArticlesAsync());
            _ = LoadArticlesAsync();
        }

        private async Task LoadArticlesAsync()
        {
            IsLoading = true;
            try
            {
                StockService.ResetInitializationFlag();
                await StockService.InitializeStockAsync();

                Articles.Clear();
                foreach (var art in StaticArticleDatabase.AllArticles)
                {
                    Articles.Add(new Article
                    {
                        Code = art.Code,
                        Reference = art.Reference,
                        Color = art.Color,
                        Dimensions = art.Dimensions,
                        Length = art.Length,
                        Depth = art.Depth,
                        Height = art.Height,
                        NumberOfPiecesAvailable = art.NumberOfPiecesAvailable,
                        SellingPrice = art.SellingPrice
                    });
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"❌ Erreur chargement articles : {ex.Message}");
            }
            finally
            {
                IsLoading = false;
            }
        }

        private async Task SaveArticlesAsync()
        {
            IsLoading = true;
            try
            {
                // 1) Versionner les commandes et l'inventaire actuel
                ConfirmedOrderService.SnapshotCurrentOrders();
                InventoryModificationService.SnapshotCurrent();

                // 2) ⚠️ CORRECTION: Calculer le stock de BASE (stock BDD brut)
                var dataAccess = new DataAccess();

                var baseStockFromDB = await Task.Run(() => dataAccess.GetArticles());

                var baseStockCalculated = baseStockFromDB
                    .Select(a => new
                    {
                        a.Code,
                        a.Color,
                        a.Height,
                        BaseQty = a.NumberOfPiecesAvailable
                    })
                    .ToList();

                // 3) Calcul des ajustements relatifs (deltas) depuis le stock de BASE
                var adjustments = Articles
                    .Select(a =>
                    {
                        var baseStock = baseStockCalculated.FirstOrDefault(x =>
                            x.Code == a.Code && x.Color == a.Color && x.Height == a.Height);

                        if (baseStock == null) return null;

                        var currentVisibleStock = a.NumberOfPiecesAvailable;

                        var oldVisibleStock = StaticArticleDatabase.AllArticles
                            .FirstOrDefault(x => x.Code == a.Code && x.Color == a.Color && x.Height == a.Height)
                            ?.NumberOfPiecesAvailable ?? 0;

                        var delta = currentVisibleStock - oldVisibleStock;

                        return new InventoryAdjustment
                        {
                            Code = a.Code ?? string.Empty,
                            Color = a.Color ?? string.Empty,
                            Height = a.Height,
                            Delta = delta
                        };
                    })
                    .Where(adj => adj != null && adj.Delta != 0)
                    .Cast<InventoryAdjustment>()
                    .ToList();

                // 4) Persister chaque ajustement en base
                foreach (var adj in adjustments)
                {
                    var baseStock = baseStockCalculated
                        .First(x => x.Code == adj.Code && x.Color == adj.Color && x.Height == adj.Height);

                    var newBaseStockQty = baseStock.BaseQty + adj.Delta;

                    await Task.Run(() => dataAccess.UpdateStock(
                        reference: string.Empty,
                        color: adj.Color,
                        newQuantity: newBaseStockQty,
                        height: adj.Height));

                    Console.WriteLine($"🔄 Base stock updated: {adj.Code} {adj.Color} base: {baseStock.BaseQty} + {adj.Delta} = {newBaseStockQty}");
                }

                // 5) Sauvegarde des ajustements
                InventoryModificationService.SaveCurrent(adjustments);

                Console.WriteLine($"✅ Ajustements de stock de base enregistrés: {adjustments.Count}");
            }
            catch (Exception ex)
            {
                Console.WriteLine($"❌ Erreur sauvegarde ajustements : {ex.Message}");
            }
            finally
            {
                // 6) Rechargement
                StockService.ResetInitializationFlag();
                await StockService.InitializeStockAsync();
                await LoadArticlesAsync();
            }
        }

        public event PropertyChangedEventHandler? PropertyChanged;
        protected void OnPropertyChanged([CallerMemberName] string? propertyName = null)
            => PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
    }
}
