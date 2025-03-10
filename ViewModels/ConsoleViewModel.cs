using System;
using System.Collections.ObjectModel;
using System.Text;
using KitBox_Project.Data;
using KitBox_Project.Models;

namespace KitBox_Project.ViewModels
{
    public class ConsoleViewModel
    {
        private readonly DataAccess _dataAccess = new DataAccess();
        public ObservableCollection<string> ConsoleMessages { get; } = new ObservableCollection<string>();

        public void LoadArticles()
        {
            try
            {
                var articles = _dataAccess.GetArticles();
                StringBuilder sb = new StringBuilder();

                foreach (var article in articles)
                {
                    sb.AppendLine($"Code: {article.Code}, Référence: {article.Reference}, Couleur: {article.Color}, " +
                                  $"Dimensions: {article.Length}x{article.Width}x{article.Depth} cm, " +
                                  $"Prix Vente: {article.SellingPrice}€, Stock: {article.NumberOfPiecesAvailable}");
                }

                if (articles.Count == 0)
                {
                    sb.AppendLine("Aucun article trouvé.");
                }

                AppendText(sb.ToString());
            }
            catch (Exception ex)
            {
                AppendText($"Erreur lors du chargement des articles: {ex.Message}");
            }
        }

        public Action<string> AppendText { get; set; } = (msg) => { };
    }
}
