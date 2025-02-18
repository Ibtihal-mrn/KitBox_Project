using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.Extensions.Configuration;
using MySql.Data.MySqlClient;
using Backend.Models;

namespace Backend.Services
{
    public class DatabaseService
    {
        private readonly string _connectionString;

        public DatabaseService(IConfiguration configuration)
        {
            _connectionString = configuration.GetConnectionString("DefaultConnection") ?? string.Empty;
            Console.WriteLine($"📌 Chaîne de connexion récupérée : {_connectionString}");

            if (string.IsNullOrEmpty(_connectionString))
            {
                Console.WriteLine("⚠️ La chaîne de connexion est vide. Vérifiez votre appsettings.json !");
            }
            else
            {
                TestConnexion();
            }
        }

        private void TestConnexion()
        {
            try
            {
                using (var connection = new MySqlConnection(_connectionString))
                {
                    connection.Open();
                    Console.WriteLine("✅ Connexion MySQL test OK !");
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"❌ Erreur de connexion MySQL : {ex.Message}");
            }
        }

        public async Task<List<Fournisseur>> GetFournisseursAsync()
        {
            var fournisseurs = new List<Fournisseur>();

            try
            {
                using (var connection = new MySqlConnection(_connectionString))
                {
                    Console.WriteLine("🔄 Tentative d'ouverture de la connexion...");
                    await connection.OpenAsync();
                    Console.WriteLine("✅ Connexion réussie à la base de données.");

                    string query = "SELECT * FROM fournisseurs";
                    using (var command = new MySqlCommand(query, connection))
                    using (var reader = await command.ExecuteReaderAsync())
                    {
                        Console.WriteLine("🔍 Exécution de la requête : " + query);

                        while (await reader.ReadAsync())
                        {
                            Console.WriteLine("📌 Lecture d'une ligne de résultat...");
                            
                            var fournisseur = new Fournisseur
                            {
                                PK_Numero_fournisseur = reader.GetInt32(reader.GetOrdinal("PK_Numero_fournisseur")), // Numérique (INT)
                                Nom = reader.GetString(reader.GetOrdinal("Nom")), // Texte (VARCHAR)
                                Adresse = reader.GetString(reader.GetOrdinal("Adresse")), // Texte (VARCHAR)
                                Code_postal = reader.GetString(reader.GetOrdinal("Code_postal")), // Texte (VARCHAR)
                                Ville = reader.GetString(reader.GetOrdinal("Ville")), // Texte (VARCHAR)
                                Tel = reader.GetString(reader.GetOrdinal("Tel")) // Texte (VARCHAR)
                            };
                            fournisseurs.Add(fournisseur);
                            Console.WriteLine($"✅ Fournisseur récupéré : {fournisseur.Nom}");
                        }
                    }
                }
            }
            catch (Exception ex) // 🛠 Ajout du catch ici
            {
                Console.WriteLine($"❌ Erreur SQL : {ex.Message}");
            } // 🛠 Assurez-vous que le catch ferme bien le try

            return fournisseurs;
        }
    }
    
}