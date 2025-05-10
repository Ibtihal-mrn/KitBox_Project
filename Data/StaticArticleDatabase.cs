using System.Collections.Generic;
using KitBox_Project.Models;

namespace KitBox_Project.Data
{
    public static class StaticArticleDatabase
    {
        // Contiendra tous les articles récupérés depuis la base de données
        public static List<Article> AllArticles { get; set; } = new List<Article>();
    }
}
