using MySql.Data.MySqlClient;
using System;
using System.Collections.Generic;
using KitBox_Project.Data;
using KitBox_Project.Config;
using KitBox_Project.Models;

namespace KitBox_Project.Data
{
    public class DataAccess
    {
        private string _connectionString = DatabaseConfig.ConnectionString;

        public List<Article> GetArticles()
        {
            List<Article> articles = new List<Article>();

            using (MySqlConnection conn = new MySqlConnection(_connectionString))
            {
                try
                {
                    conn.Open();
                    string query = "SELECT PK_num_article, description_Article, FK_num_categorie FROM articles";
                    using (MySqlCommand cmd = new MySqlCommand(query, conn))
                    using (MySqlDataReader reader = cmd.ExecuteReader())
                    {
                        while (reader.Read())
                        {
                            articles.Add(new Article
                            {
                                PK_num_article = reader.GetInt32("PK_num_article"),
                                DescriptionArticle = reader.GetString("description_Article"),
                                FK_num_categorie = reader.GetInt32("FK_num_categorie")
                            });
                        }
                    }
                }
                catch (Exception ex)
                {
                    Console.WriteLine($"Erreur : {ex.Message}");
                }
            }
            return articles;
        }
    }
}