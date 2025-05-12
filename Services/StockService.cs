using System;
using System.Collections.Generic;
using System.Linq;
using KitBox_Project.Models;
using KitBox_Project.Data;
using KitBox_Project.Services; // pour accéder à ConfirmedOrderService

namespace KitBox_Project.Services
{
    /// <summary>
    /// Service responsable de la gestion du stock des articles
    /// </summary>
    public static class StockService
    {
        /// <summary>
        /// Vérifie si tous les articles du panier ont un stock suffisant
        /// </summary>
        public static (bool IsStockSufficient, List<(Article Article, int Available)> InsufficientItems) CheckStock(IEnumerable<Article> selectedArticles)
        {
            var insufficientItems = new List<(Article Article, int Available)>();

            foreach (var article in selectedArticles)
            {
                var stockArticle = StaticArticleDatabase.AllArticles
                    .FirstOrDefault(a => a.Code == article.Code && a.Color == article.Color);

                if (stockArticle == null || stockArticle.NumberOfPiecesAvailable < article.Quantity)
                {
                    insufficientItems.Add((article, stockArticle?.NumberOfPiecesAvailable ?? 0));
                }
            }

            return (insufficientItems.Count == 0, insufficientItems);
        }

        /// <summary>
        /// Met à jour le stock pour les articles commandés
        /// </summary>
        public static bool UpdateStock(IEnumerable<Article> selectedArticles)
        {
            try
            {
                foreach (var article in selectedArticles)
                {
                    var stockArticle = StaticArticleDatabase.AllArticles
                        .FirstOrDefault(a => a.Code == article.Code && a.Color == article.Color);

                    if (stockArticle != null)
                    {
                        stockArticle.NumberOfPiecesAvailable -= article.Quantity;
                        Console.WriteLine($"[DEBUG] Stock mis à jour pour {article.Reference} ({article.Color}): nouveau stock = {stockArticle.NumberOfPiecesAvailable}");
                    }
                    else
                    {
                        Console.WriteLine($"[WARNING] Article non trouvé dans la base: {article.Reference} ({article.Color})");
                    }
                }
                return true;
            }
            catch (Exception ex)
            {
                Console.WriteLine($"[ERROR] Erreur lors de la mise à jour du stock: {ex.Message}");
                return false;
            }
        }

        /// <summary>
        /// Charge les commandes confirmées depuis le fichier JSON et ajuste le stock
        /// </summary>
        public static void LoadConfirmedOrdersAndAdjustStock()
        {
            // On charge les commandes structurées
            var confirmedOrders = ConfirmedOrderService.LoadConfirmedOrders();

            if (confirmedOrders.Any())
            {
                Console.WriteLine("📦 Mise à jour du stock selon les commandes précédemment confirmées...");

                // Pour chaque commande, on déduit chaque article
                foreach (var order in confirmedOrders)
                {
                    if (order.Articles != null && order.Articles.Any())
                    {
                        UpdateStock(order.Articles);
                    }
                }
            }
            else
            {
                Console.WriteLine("Aucun article confirmé trouvé dans le fichier JSON.");
            }
        }
    }
}
