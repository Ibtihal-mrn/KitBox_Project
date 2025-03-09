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
                    sb.AppendLine($"ID: {article.PK_num_article}, Description: {article.DescriptionArticle}, Catégorie: {article.FK_num_categorie}");
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
