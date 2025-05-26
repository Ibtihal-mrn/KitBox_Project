using System;
using System.Collections.ObjectModel;
using System.Linq;
using System.Threading.Tasks;
using KitBox_Project.Data;
using KitBox_Project.Models;
using KitBox_Project.Services;

namespace KitBox_Project
{
    public static class AppState
    {
        public static string SelectedColor { get; set; } = "White";
        public static string? SelectedAngleIronColor { get; set; }

        public static int SelectedLength { get; set; }
        public static int SelectedDepth { get; set; }
        public static int SelectedHeight { get; set; }

        public static ObservableCollection<Article> SelectedArticles { get; } = new();

        public static void AddToCart(Article article)
        {
            if (article == null) return;

            int alreadyInCart = SelectedArticles.Count(a =>
                a.Code == article.Code && a.Color == article.Color && a.Height == article.Height);

            if (alreadyInCart >= article.NumberOfPiecesAvailable)
            {
                Console.WriteLine($"🚫 Stock épuisé pour {article.Reference} ({article.Color})");
                return;
            }

            SelectedArticles.Add(article);
            Console.WriteLine($"✅ Article ajouté : {article.Reference}, {alreadyInCart+1}/{article.NumberOfPiecesAvailable}");
        }

        public static void ClearCart()
        {
            SelectedArticles.Clear();
            Console.WriteLine("🧹 Panier vidé.");
        }

        public static async Task ReloadAllAsync()
        {
            // Ne pas vider le panier ici
            StaticArticleDatabase.AllArticles.Clear();
            StockService.ResetInitializationFlag();
            await StockService.InitializeStockAsync();
            Console.WriteLine("🔄 AppState rechargé.");
        }
    }
}