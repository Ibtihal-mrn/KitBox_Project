using System.Collections.Generic;
using KitBox_Project.Models;

namespace KitBox_Project.Data
{
    public static class StaticArticleDatabase
    {
        // will contain all items retrieved from the database
        public static List<Article> AllArticles { get; set; } = new List<Article>();
    }
}