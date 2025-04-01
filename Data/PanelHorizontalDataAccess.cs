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
                    // MODIFIER : Ajout de la condition AND Color = @Color
                    string query = "SELECT Length FROM new_table WHERE reference LIKE '%panel_horizontal%' AND `number of pieces available` > 0 AND Color = @Color";

                    using (MySqlCommand cmd = new MySqlCommand(query, conn))
                    {
                        cmd.Parameters.AddWithValue("@Color", KitBox_Project.AppState.SelectedColor); // MODIFIER : Utilisation de AppState.SelectedColor

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
                    // MODIFIER : Ajout de la condition AND Color = @Color
                    string query = "SELECT Depth FROM new_table WHERE reference LIKE '%panel_horizontal%' AND Length = @Length AND `number of pieces available` > 0 AND Color = @Color";

                    using (MySqlCommand cmd = new MySqlCommand(query, conn))
                    {
                        cmd.Parameters.AddWithValue("@Length", length);
                        cmd.Parameters.AddWithValue("@Color", KitBox_Project.AppState.SelectedColor); // MODIFIER : Utilisation de AppState.SelectedColor

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
                    // MODIFIER : Ajout de la condition AND Color = @Color
                    string query = @"
                        SELECT `number of pieces available` 
                        FROM new_table 
                        WHERE (reference LIKE '%panel_back%' AND Length = @Length AND Color = @Color) 
                        OR (reference LIKE '%panel_left%' OR reference LIKE '%panel_right%' 
                            AND Dimensions LIKE CONCAT(@Depth, '%x%', @Length, '%') AND Color = @Color)";

                    using (MySqlCommand cmd = new MySqlCommand(query, conn))
                    {
                        cmd.Parameters.AddWithValue("@Length", length);
                        cmd.Parameters.AddWithValue("@Depth", depth);
                        cmd.Parameters.AddWithValue("@Color", KitBox_Project.AppState.SelectedColor); // MODIFIER : Utilisation de AppState.SelectedColor

                        using (MySqlDataReader reader = cmd.ExecuteReader())
                        {
                            while (reader.Read())
                            {
                                int stockAvailable = reader.IsDBNull(reader.GetOrdinal("number of pieces available")) ? 0 : reader.GetInt32("number of pieces available");
                                if (stockAvailable <= 5)
                                {
                                    isLowStock = true;
                                    break;
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
                    // MODIFIER : Ajout de la condition AND Color = @Color
                    string query = @"
                        SELECT DISTINCT Height 
                        FROM new_table 
                        WHERE (reference LIKE '%panel_back%' AND Length = @Length AND Color = @Color AND `number of pieces available` > 0)
                        OR (reference LIKE '%panel_left%' OR reference LIKE '%panel_right%' 
                            AND Dimensions LIKE CONCAT(@Depth, '%x%', @Length, '%') AND Color = @Color AND `number of pieces available` > 0)";

                    using (MySqlCommand cmd = new MySqlCommand(query, conn))
                    {
                        cmd.Parameters.AddWithValue("@Length", length);
                        cmd.Parameters.AddWithValue("@Depth", depth);
                        cmd.Parameters.AddWithValue("@Color", KitBox_Project.AppState.SelectedColor); // MODIFIER : Utilisation de AppState.SelectedColor

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

        // Classe LowStockItem mise à jour
        public class LowStockItem
        {
            public string? Reference { get; set; }
            public int AvailableQuantity { get; set; }
            public int Length { get; set; }
            public int Depth { get; set; }
            public int Height { get; set; }
        }

        public List<LowStockItem> GetLowStockItems(int length, int depth)
        {
            List<LowStockItem> lowStockItems = new List<LowStockItem>();

            using (MySqlConnection conn = new MySqlConnection(_connectionString))
            {
                try
                {
                    conn.Open();
                    // MODIFIER : Ajout de la condition AND Color = @Color
                    string query = @"
                        SELECT Reference, `number of pieces available`, Length, Depth, Height 
                        FROM new_table 
                        WHERE (Reference LIKE '%panel back%' 
                            OR Reference LIKE '%panel left%')
                        AND (`number of pieces available` IS NULL 
                            OR `number of pieces available` <= 5)
                        AND Color = @Color";

                    using (MySqlCommand cmd = new MySqlCommand(query, conn))
                    {
                        cmd.Parameters.AddWithValue("@Length", length);
                        cmd.Parameters.AddWithValue("@Depth", depth);
                        cmd.Parameters.AddWithValue("@Color", KitBox_Project.AppState.SelectedColor); // MODIFIER : Utilisation de AppState.SelectedColor

                        using (MySqlDataReader reader = cmd.ExecuteReader())
                        {
                            while (reader.Read())
                            {
                                var lowStockItem = new LowStockItem
                                {
                                    Reference = reader.GetString("Reference"),
                                    AvailableQuantity = reader.IsDBNull(reader.GetOrdinal("number of pieces available"))
                                                        ? 0 // Si la valeur est nulle, on l'assume à 0
                                                        : reader.GetInt32("number of pieces available"),
                                    Length = reader.IsDBNull(reader.GetOrdinal("Length")) ? 0 : reader.GetInt32("Length"),
                                    Depth = reader.IsDBNull(reader.GetOrdinal("Depth")) ? 0 : reader.GetInt32("Depth"),
                                    Height = reader.IsDBNull(reader.GetOrdinal("Height")) ? 0 : reader.GetInt32("Height")
                                };

                                lowStockItems.Add(lowStockItem);
                            }
                        }
                    }
                }
                catch (Exception ex)
                {
                    Console.WriteLine($"Erreur : {ex.Message}");
                }
            }

            return lowStockItems;
        }
    }
}