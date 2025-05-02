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
                    // Requête proprement formatée
                    string query = @"
                        SELECT p.Length
                        FROM new_table AS p
                        WHERE p.reference LIKE '%panel_horizontal%'
                        AND p.`number of pieces available` > 0
                        AND p.Color = @Color
                        AND EXISTS (
                            SELECT 1
                            FROM new_table AS c
                            WHERE c.reference LIKE '%Crossbar front%'
                                AND c.Length = p.Length
                                AND c.`number of pieces available` > 0
                        )
                    ";

                    using (MySqlCommand cmd = new MySqlCommand(query, conn))
                    {
                        cmd.Parameters.AddWithValue("@Color", KitBox_Project.AppState.SelectedColor);

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
                    string query = @"
                    SELECT p.Depth
                    FROM new_table AS p
                    WHERE p.reference LIKE '%panel_horizontal%'
                    AND p.Length = @Length
                    AND p.`number of pieces available` > 0
                    AND p.Color = @Color
                    AND EXISTS (
                        SELECT 1
                        FROM new_table AS c
                        WHERE c.reference LIKE '%Crossbar_left%'
                            AND c.Depth = p.Depth
                            AND c.`number of pieces available` > 0
                    );
                ";


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

        public List<Article> GetHeightOfPanel(int length, int depth, string color)
        {
            List<Article> articles = new List<Article>();

            using (MySqlConnection conn = new MySqlConnection(_connectionString))
            {
                try
                {
                    conn.Open();
                    string query = @"
                        SELECT DISTINCT p.Height
                        FROM new_table AS p
                        WHERE p.Color = @Color
                        AND (
                            (
                                p.reference LIKE '%panel_back%'
                                AND p.Length = @Length
                                AND p.`number of pieces available` > 0
                                )
                                OR
                                (
                                    (p.reference LIKE '%panel_left%')
                                    AND p.Dimensions LIKE CONCAT(@Depth, 'x%', @Length, '%')
                                    AND p.`number of pieces available` > 0
                                )
                            )
                            AND EXISTS (
                                SELECT 1
                                FROM new_table AS ai
                                WHERE ai.reference LIKE '%Angle_iron%'
                                AND ai.Height = p.Height
                                AND ai.`number of pieces available` > 0
                            );
                    ";

                    using (MySqlCommand cmd = new MySqlCommand(query, conn))
                    {
                        cmd.Parameters.AddWithValue("@Length", length);
                        cmd.Parameters.AddWithValue("@Depth", depth);
                        cmd.Parameters.AddWithValue("@Color", color);

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

    public List<Article> GetAvailableDoors(int height, int length)
    {
        List<Article> doors = new List<Article>();

        using (MySqlConnection conn = new MySqlConnection(_connectionString))
        {
            try
            {
                conn.Open();
                string query = @"
                    SELECT Reference, Code, Color, Height, Length, Depth, `number of pieces available`
                    FROM new_table 
                    WHERE reference LIKE '%door%' 
                    AND (COALESCE(`number of pieces available`, 0) > 0)";

                // Ajouter les filtres seulement si height et length ne sont pas 0
                if (height != 0)
                    query += " AND Height = @Height";
                if (length != 0)
                    query += " AND Length = @Length";

                using (MySqlCommand cmd = new MySqlCommand(query, conn))
                {
                    if (height != 0)
                        cmd.Parameters.AddWithValue("@Height", height);
                    if (length != 0)
                        cmd.Parameters.AddWithValue("@Length", length);

                    using (MySqlDataReader reader = cmd.ExecuteReader())
                    {
                        while (reader.Read())
                        {
                            int stock = reader.IsDBNull(reader.GetOrdinal("number of pieces available")) ? 0 : reader.GetInt32("number of pieces available");
                            Console.WriteLine($"[DEBUG] Porte candidate - Reference: {reader.GetString("Reference")}, Height: {reader.GetInt32("Height")}, Length: {reader.GetInt32("Length")}, Stock: {stock}");

                            doors.Add(new Article
                            {
                                Reference = reader.IsDBNull(reader.GetOrdinal("Reference")) ? "N/A" : reader.GetString("Reference"),
                                Code = reader.IsDBNull(reader.GetOrdinal("Code")) ? "N/A" : reader.GetString("Code"),
                                Color = reader.IsDBNull(reader.GetOrdinal("Color")) ? "N/A" : reader.GetString("Color"),
                                Height = reader.IsDBNull(reader.GetOrdinal("Height")) ? 0 : reader.GetInt32("Height"),
                                Length = reader.IsDBNull(reader.GetOrdinal("Length")) ? 0 : reader.GetInt32("Length"),
                                Depth = reader.IsDBNull(reader.GetOrdinal("Depth")) ? 0 : reader.GetInt32("Depth")
                            });
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Erreur dans GetAvailableDoors : {ex.Message}");
            }
        }

        Console.WriteLine($"[DEBUG] GetAvailableDoors - Height: {height}, Length: {length}");
        Console.WriteLine($"[DEBUG] Nombre de portes trouvées : {doors.Count}");
        foreach (var door in doors)
        {
            Console.WriteLine($"[DEBUG] Porte - Reference: {door.Reference}, Color: {door.Color}, Height: {door.Height}, Length: {door.Length}");
        }

        return doors;
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
                        WHERE (
                            (Reference LIKE '%panel_back%' AND Length = @Length)
                            OR 
                            (Reference LIKE '%panel_left%' AND Dimensions LIKE CONCAT(@Depth, 'x%', @Length, '%'))
                        )
                        AND (`number of pieces available` IS NULL OR `number of pieces available` <= 5)
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

        public Dictionary<string,int> GetAngleIronStockByHeight(int height)
        {
            var result = new Dictionary<string,int>();
            using (var conn = new MySqlConnection(_connectionString))
            {
                conn.Open();
                string sql = @"
                    SELECT Color, SUM(`number of pieces available`) AS Qty
                    FROM new_table
                    WHERE reference LIKE '%angle_iron%'
                    AND Height = @Height
                    GROUP BY Color;
                ";
                using (var cmd = new MySqlCommand(sql, conn))
                {
                    cmd.Parameters.AddWithValue("@Height", height);
                    using (var reader = cmd.ExecuteReader())
                    {
                        while (reader.Read())
                        {
                            var c = reader.GetString("Color");
                            var q = reader.GetInt32("Qty");
                            result[c] = q;
                        }
                    }
                }
            }
            return result;
        }

    }
}