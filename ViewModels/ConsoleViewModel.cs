using System;
using System.Collections.ObjectModel;
using System.Text;
using KitBox_Project.Data;
using KitBox_Project.Models;

namespace KitBox_Project.ViewModels
{
    public class ConsoleViewModel
    {
        private readonly ConsoleDataAccess _dataAccess = new ConsoleDataAccess();
        public ObservableCollection<string> ConsoleMessages { get; } = new ObservableCollection<string>();

        public void LoadArticles()
        {
            try
            {
                // Récupération des articles depuis la base de données
                var articles = _dataAccess.GetArticles();
                StringBuilder sb = new StringBuilder();

                if (articles.Count == 0)
                {
                    sb.AppendLine("Aucun article trouvé.");
                }
                else
                {
                    foreach (var article in articles)
                    {
                        sb.AppendLine($"Code: {article.Code}, Référence: {article.Reference}, Couleur: {article.Color}, " +
                                      $"Dimensions: {article.Length}x{article.Width}x{article.Depth} cm, " +
                                      $"Prix Vente: {article.SellingPrice}€, Stock: {article.NumberOfPiecesAvailable}");
                    }
                }

                // Affichage du résultat
                AppendText(sb.ToString());
            }
            catch (Exception ex)
            {
                AppendText($"❌ Erreur lors du chargement des articles: {ex.Message}");
            }
        }

        // Action pour afficher les messages dans la console
        public Action<string> AppendText { get; set; } = (msg) => { };
    }
}
