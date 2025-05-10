using System;
using System.Collections.Generic;
using System.Collections.ObjectModel; // Add this for ObservableCollection
using System.Linq;
using System.Threading.Tasks;
using KitBox_Project.Data;
using KitBox_Project.Models;

namespace KitBox_Project
{
    public static class AppState
    {
        public static string SelectedColor { get; set; } = "White";
        public static int SelectedLength { get; set; }
        public static int SelectedDepth { get; set; }
        public static int SelectedHeight { get; set; }

        // Change List<Article> to ObservableCollection<Article>
        public static ObservableCollection<Article> SelectedArticles { get; private set; } = new ObservableCollection<Article>();

        public static void AddToCart(Article article)
        {
            if (article != null && article.NumberOfPiecesAvailable > 0)
            {
                SelectedArticles.Add(article);
                Console.WriteLine($"Article ajouté au panier : {article.Reference}, Color: {article.Color}, Price: {article.SellingPrice}, Stock: {article.NumberOfPiecesAvailable}");
            }
            else
            {
                Console.WriteLine($"Erreur : Impossible d'ajouter l'article {article?.Reference} (stock insuffisant ou article nul).");
            }
        }

        public static void AddRangeToCart(IEnumerable<Article> articles)
        {
            foreach (var article in articles)
            {
                if (article != null && article.NumberOfPiecesAvailable > 0)
                {
                    SelectedArticles.Add(article);
                }
            }
            Console.WriteLine($"Ajout de {articles.Count()} articles au panier.");
        }

        public static void ClearCart()
        {
            SelectedArticles.Clear();
            Console.WriteLine("Panier vidé.");
        }

        public static async Task LoadArticlesFromDatabase()
        {
            try
            {
                var dataAccess = new DataAccess();
                StaticArticleDatabase.AllArticles = await Task.Run(() => dataAccess.GetArticles());
                Console.WriteLine($"Chargement terminé. {StaticArticleDatabase.AllArticles.Count} articles chargés dans StaticArticleDatabase.");
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Erreur lors du chargement des articles : {ex.Message}");
            }
        }

        public static void AddToCartFromDatabase(string reference, string color)
        {
            var article = StaticArticleDatabase.AllArticles
                .FirstOrDefault(a => a.Reference == reference && a.Color == color && a.NumberOfPiecesAvailable > 0);

            if (article != null)
            {
                AddToCart(article);
            }
            else
            {
                Console.WriteLine($"⚠️ Article '{reference}' avec couleur '{color}' non trouvé ou en rupture de stock.");
            }
        }
    }
}