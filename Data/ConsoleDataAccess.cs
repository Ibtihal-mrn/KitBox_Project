using MySql.Data.MySqlClient;
using System;
using System.Collections.Generic;
using KitBox_Project.Config;
using KitBox_Project.Models;

namespace KitBox_Project.Data
{
    public class ConsoleDataAccess
    {
        private readonly string _connectionString = DatabaseConfig.ConnectionString;

        public List<Article> GetArticles()
        {
            List<Article> articles = new List<Article>();

            using (MySqlConnection conn = new MySqlConnection(_connectionString))
            {
                try
                {
                    conn.Open();
                    string query = @"
                        SELECT Reference, Code, Color, Dimensions, Length, Width, Depth, 
                               `Price-SupplierUno`, `Delay-SupplierUno`, 
                               `Price-SupplierDos`, `Delay-SupplierDos`, 
                               `selling price`, `number of pieces available` 
                        FROM test.new_table";

                    using (MySqlCommand cmd = new MySqlCommand(query, conn))
                    using (MySqlDataReader reader = cmd.ExecuteReader())
                    {
                        while (reader.Read())
                        {
                            var article = new Article
                            {
                                Code = reader.IsDBNull(reader.GetOrdinal("Code")) ? "N/A" : reader.GetString("Code"),
                                Reference = reader.IsDBNull(reader.GetOrdinal("Reference")) ? "N/A" : reader.GetString("Reference"),
                                Color = reader.IsDBNull(reader.GetOrdinal("Color")) ? "N/A" : reader.GetString("Color"),
                                Dimensions = reader.IsDBNull(reader.GetOrdinal("Dimensions")) ? "N/A" : reader.GetString("Dimensions"),
                                Length = reader.IsDBNull(reader.GetOrdinal("Length")) ? 0 : reader.GetInt32("Length"),
                                Width = reader.IsDBNull(reader.GetOrdinal("Width")) ? 0 : reader.GetInt32("Width"),
                                Depth = reader.IsDBNull(reader.GetOrdinal("Depth")) ? 0 : reader.GetInt32("Depth"),
                                PriceSupplierUno = reader.IsDBNull(reader.GetOrdinal("Price-SupplierUno")) ? 0m : reader.GetDecimal("Price-SupplierUno"),
                                DelaySupplierUno = reader.IsDBNull(reader.GetOrdinal("Delay-SupplierUno")) ? 0 : reader.GetInt32("Delay-SupplierUno"),
                                PriceSupplierDos = reader.IsDBNull(reader.GetOrdinal("Price-SupplierDos")) ? 0m : reader.GetDecimal("Price-SupplierDos"),
                                DelaySupplierDos = reader.IsDBNull(reader.GetOrdinal("Delay-SupplierDos")) ? 0 : reader.GetInt32("Delay-SupplierDos"),
                                SellingPrice = reader.IsDBNull(reader.GetOrdinal("selling price")) ? 0m : reader.GetDecimal("selling price"),
                                NumberOfPiecesAvailable = reader.IsDBNull(reader.GetOrdinal("number of pieces available")) ? 0 : reader.GetInt32("number of pieces available")
                            };

                            articles.Add(article);
                        }
                    }
                }
                catch (Exception ex)
                {
                    Console.WriteLine($"❌ Erreur SQL dans GetArticles : {ex.Message}");
                }
            }
            return articles;
        }
    }
}
