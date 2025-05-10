using System;
using System.Collections.Generic;
using System.Linq;
using KitBox_Project.Models;
using KitBox_Project.Data;

namespace KitBox_Project.Services
{
    /// <summary>
    /// Service responsable de la gestion du stock des articles
    /// </summary>
    public class StockService
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
                    .FirstOrDefault(a => a.Code == article.Code);

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
                        .FirstOrDefault(a => a.Code == article.Code);

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
    }
}
