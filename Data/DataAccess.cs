using MySql.Data.MySqlClient;
using System;
using System.Collections.Generic;
using KitBox_Project.Config;
using KitBox_Project.Models;

namespace KitBox_Project.Data
{
    public class DataAccess
    {
        private string _connectionString = DatabaseConfig.ConnectionString;

        public List<Article> GetLengthOfPanelHorizontal()
        {
            List<Article> articles = new List<Article>();

            using (MySqlConnection conn = new MySqlConnection(_connectionString))
            {
                try
                {
                    conn.Open();
                    string query = "SELECT Length FROM new_table WHERE reference LIKE '%panel_horizontal%'";

                    using (MySqlCommand cmd = new MySqlCommand(query, conn))
                    using (MySqlDataReader reader = cmd.ExecuteReader())
                    {
                        // Vérifiez si le reader contient des données
                        while (reader.Read())
                        {
                            // Vérifier si la colonne "Length" n'est pas nulle
                            if (!reader.IsDBNull(reader.GetOrdinal("Length")))
                            {
                                articles.Add(new Article
                                {
                                    Length = reader.GetInt32("Length")
                                });
                            }
                            else
                            {
                                Console.WriteLine("La colonne 'Length' est nulle pour cette ligne.");
                            }
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
        public List<Article> GetDepthOfPanelHorizontal(int length)
        {
            List<Article> articles = new List<Article>();

            using (MySqlConnection conn = new MySqlConnection(_connectionString))
            {
                try
                {
                    conn.Open();
                    // Utilisation d'un paramètre pour éviter les injections SQL
                    string query = "SELECT Depth FROM new_table WHERE reference LIKE '%panel_horizontal%' AND Length = @Length";

                    using (MySqlCommand cmd = new MySqlCommand(query, conn))
                    {
                        // Ajout du paramètre pour la longueur
                        cmd.Parameters.AddWithValue("@Length", length);

                        using (MySqlDataReader reader = cmd.ExecuteReader())
                        {
                            while (reader.Read())
                            {
                                articles.Add(new Article
                                {
                                    Depth = reader.IsDBNull(reader.GetOrdinal("Depth")) ? 0 : reader.GetInt32("Depth")
                                });
                            }
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
