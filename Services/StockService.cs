using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using KitBox_Project.Data;
using KitBox_Project.Models;

namespace KitBox_Project.Services
{
    public static class StockService
    {
        private static bool _stockInitialized;

        public static async Task InitializeStockAsync()
        {
            if (!_stockInitialized)
                await ForceReloadStockAsync();
        }

        public static void ResetInitializationFlag() => _stockInitialized = false;

        /// <summary>
        /// UNIQUEMENT pour le démarrage de l'application ou cas exceptionnels
        /// </summary>
        public static async Task ForceReloadStockAsync()
        {
            Console.WriteLine("🔄 Rechargement complet du stock...");

            await ResetStockToOriginalAsync();

            // 1) On applique d’abord les ajustements manuels (+5, etc.)
            ApplyInventoryModifications();

            // 2) Puis on applique toutes les commandes confirmées (−30, −3, etc.)
            var allOrders = ConfirmedOrderService.LoadAllOrders();
            foreach (var o in allOrders)
                UpdateStock(o.Articles);

            // 3) Enfin, on applique le panier en cours (s’il y en a)
            ApplyPendingCartToStock();

            _stockInitialized = true;

        }

        /// <summary>
        /// NOUVELLE MÉTHODE : Recalcule uniquement le stock sans recharger la BDD
        /// </summary>
        public static async Task RefreshStockCalculationAsync()
        {
            if (!_stockInitialized)
            {
                await ForceReloadStockAsync();
                return;
            }

            Console.WriteLine("🔢 Recalcul du stock en cours...");

            await ResetStockToOriginalAsync();

            // 2) Appliquer d’abord les ajustements manuels
            ApplyInventoryModifications();

            // 3) Puis toutes les commandes (snapshots + current)
            var allOrders = ConfirmedOrderService.LoadAllOrders();
            foreach (var o in allOrders)
                UpdateStock(o.Articles);

            // 4) Enfin, déduire le contenu du panier courant
            ApplyPendingCartToStock();


            Console.WriteLine($"✅ Stock recalculé sans rechargement BDD");
        }

        public static async Task ResetStockToOriginalAsync()
        {
            var data = new DataAccess();
            StaticArticleDatabase.AllArticles = await Task.Run(() => data.GetArticles());
        }

        public static void ResetStockToOriginalSync()
        {
            var data = new DataAccess();
            StaticArticleDatabase.AllArticles = data.GetArticles();
        }

        public static void ApplyInventoryModifications()
        {
            var adjustments = InventoryModificationService.LoadAll();
            foreach (var adj in adjustments)
            {
                var art = StaticArticleDatabase.AllArticles.FirstOrDefault(a =>
                    a.Code == adj.Code && a.Color == adj.Color && a.Height == adj.Height);
                if (art != null)
                    art.NumberOfPiecesAvailable = Math.Max(0, art.NumberOfPiecesAvailable + adj.Delta);
            }
        }

        public static void ApplyPendingCartToStock()
        {
            var pending = AppState.SelectedArticles;
            UpdateStock(pending);
            Console.WriteLine($"[Stock] Panier appliqué : {pending.Count} articles déduits");
        }

        public static void UpdateStock(IEnumerable<Article> items)
        {
            foreach (var i in items)
            {
                var s = StaticArticleDatabase.AllArticles.FirstOrDefault(a =>
                    a.Code == i.Code && a.Color == i.Color && a.Height == i.Height);
                if (s != null)
                    s.NumberOfPiecesAvailable = Math.Max(0, s.NumberOfPiecesAvailable - i.Quantity);
            }
        }

        /// <summary>
        /// NOUVELLE MÉTHODE : À appeler après validation d'une commande
        /// </summary>
        public static async Task OnOrderConfirmedAsync()
        {
            // Les commandes sont déjà sauvegardées par ConfirmedOrderService
            // On recalcule juste le stock sans recharger la BDD
            await RefreshStockCalculationAsync();
            Console.WriteLine("📦 Stock mis à jour après validation de commande");
        }
    }
}