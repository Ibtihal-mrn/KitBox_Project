using MySql.Data.MySqlClient;
using System;
using System.Collections.Generic;
using AvaloniaApplication1.Models;
using AvaloniaApplication1.Config;

namespace AvaloniaApplication1.Data
{
    public class DataAccess
    {
        private string _connectionString = DatabaseConfig.ConnectionString;

        public List<Kitbox_components> GetArticles()
        {
            List<Kitbox_components> articles = new List<Kitbox_components>();

            using (MySqlConnection conn = new MySqlConnection(_connectionString))
            {
                try
                {
                    conn.Open();
                    string query = "SELECT id, reference, selling_price FROM kitbox_components";
                    using (MySqlCommand cmd = new MySqlCommand(query, conn))
                    using (MySqlDataReader reader = cmd.ExecuteReader())
                    {
                        while (reader.Read())
                        {
                            articles.Add(new Kitbox_components
                            {
                                Id = reader.GetInt32("id"),
                                Reference = reader.GetString("reference"),
                                Code = reader.GetString("code"),
                                Dimensions = reader.GetString("dimensions"),
                                LengthCm = reader.GetFloat("length_cm"),
                                WidthCm = reader.GetFloat("width_cm"),
                                DepthCm = reader.GetFloat("depth_cm"),
                                PriceSupplier1 = reader.GetFloat("price_supplier1"),
                                DelaySupplier1 = reader.GetInt32("delay_supplier1"),
                                PriceSupplier2 = reader.GetFloat("price_supplier2"),
                                DelaySupplier2 = reader.GetInt32("delay_supplier2"),
                                SellingPrice = reader.GetFloat("selling_price"),
                                NumberOfPiecesAvailable = reader.GetInt32("number_of_pieces_available"),
                                Color = reader.GetString("color")

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