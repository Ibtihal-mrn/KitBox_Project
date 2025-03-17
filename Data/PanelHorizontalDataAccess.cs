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

        // Récupérer la longueur des panneaux horizontaux
        public List<Article> GetLengthOfPanelHorizontal()
        {
            List<Article> articles = new List<Article>();

            using (MySqlConnection conn = new MySqlConnection(_connectionString))
            {
                try
                {
                    conn.Open();
                    string query = "SELECT Length FROM new_table WHERE reference LIKE '%panel_horizontal%' AND `number of pieces available` > 0";

                    using (MySqlCommand cmd = new MySqlCommand(query, conn))
                    using (MySqlDataReader reader = cmd.ExecuteReader())
                    {
                        while (reader.Read())
                        {
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

        // Récupérer la profondeur des panneaux horizontaux en fonction de la longueur
        public List<Article> GetDepthOfPanelHorizontal(int length)
        {
            List<Article> articles = new List<Article>();

            using (MySqlConnection conn = new MySqlConnection(_connectionString))
            {
                try
                {
                    conn.Open();
                    string query = "SELECT Depth FROM new_table WHERE reference LIKE '%panel_horizontal%' AND Length = @Length AND `number of pieces available` > 0";

                    using (MySqlCommand cmd = new MySqlCommand(query, conn))
                    {
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

       public bool IsStockLow(int length, int depth)
        {
            bool isLowStock = false;

            using (MySqlConnection conn = new MySqlConnection(_connectionString))
            {
                try
                {
                    conn.Open();
                    string query = @"
                        SELECT `number of pieces available` 
                        FROM new_table 
                        WHERE (reference LIKE '%panel_back%' AND Length = @Length) 
                        OR (reference LIKE '%panel_left%' OR reference LIKE '%panel_right%' 
                            AND Dimensions LIKE CONCAT(@Depth, '%x%', @Length, '%'))";

                    using (MySqlCommand cmd = new MySqlCommand(query, conn))
                    {
                        cmd.Parameters.AddWithValue("@Length", length);
                        cmd.Parameters.AddWithValue("@Depth", depth);

                        using (MySqlDataReader reader = cmd.ExecuteReader())
                        {
                            while (reader.Read())
                            {
                                int stockAvailable = reader.IsDBNull(reader.GetOrdinal("number of pieces available")) ? 0 : reader.GetInt32("number of pieces available");
                                if (stockAvailable <= 5)
                                {
                                    isLowStock = true; // Si stock inférieur ou égal à 5, on met l'indicateur à true
                                    break; // Une fois qu'on trouve un article en stock faible, on arrête la recherche
                                }
                            }
                        }
                    }
                }
                catch (Exception ex)
                {
                    Console.WriteLine($"Erreur : {ex.Message}");
                }
            }

            return isLowStock;
        }
        public List<Article> GetHeightOfPanel(int length, int depth)
        {
            List<Article> articles = new List<Article>();

            using (MySqlConnection conn = new MySqlConnection(_connectionString))
            {
                try
                {
                    conn.Open();
                    string query = @"
                        SELECT DISTINCT Height 
                        FROM new_table 
                        WHERE (reference LIKE '%panel_back%' AND Length = @Length) AND `number of pieces available` > 0
                        OR (reference LIKE '%panel_left%' OR reference LIKE '%panel_right%' 
                            AND Dimensions LIKE CONCAT(@Depth, '%x%', @Length, '%')) AND `number of pieces available` > 0";

                    using (MySqlCommand cmd = new MySqlCommand(query, conn))
                    {
                        cmd.Parameters.AddWithValue("@Length", length);
                        cmd.Parameters.AddWithValue("@Depth", depth);

                        using (MySqlDataReader reader = cmd.ExecuteReader())
                        {
                            while (reader.Read())
                            {
                                articles.Add(new Article
                                {
                                    Height = reader.IsDBNull(reader.GetOrdinal("Height")) ? 0 : reader.GetInt32("Height")
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

    }}
