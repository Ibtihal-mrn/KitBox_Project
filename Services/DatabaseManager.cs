using Microsoft.Data.Sqlite;
using System;
using System.Collections.Generic;

namespace KitBox_Project.Data
{
    public static class DatabaseManager
    {
        public class Utilisateur
        {
            public string Username { get; set; } = "";
            public string Password { get; set; } = "";
            public string Role { get; set; } = "";
        }

        public static void InitializeDatabase()
        {
            using var connection = new SqliteConnection("Data Source=utilisateurs.db");
            connection.Open();

            var command = connection.CreateCommand();
            command.CommandText = @"
                CREATE TABLE IF NOT EXISTS utilisateurs (
                    Id INTEGER PRIMARY KEY AUTOINCREMENT,
                    Username TEXT NOT NULL UNIQUE,
                    Password TEXT NOT NULL,
                    Role TEXT NOT NULL
                );
            ";
            command.ExecuteNonQuery();
        }

        public static void InsertTestUser()
        {
            using var connection = new SqliteConnection("Data Source=utilisateurs.db");
            connection.Open();

            var command = connection.CreateCommand();
            command.CommandText = @"
                INSERT OR IGNORE INTO utilisateurs (Username, Password, Role)
                VALUES ('1', 'mdp123', 'superviseur');
            ";
            command.ExecuteNonQuery();
        }

        public static string? AuthenticateUser(string username, string password)
        {
            using var connection = new SqliteConnection("Data Source=utilisateurs.db");
            connection.Open();

            var command = connection.CreateCommand();
            command.CommandText = @"
                SELECT Role FROM utilisateurs
                WHERE Username = $username AND Password = $password;
            ";

            command.Parameters.AddWithValue("$username", username);
            command.Parameters.AddWithValue("$password", password);

            var result = command.ExecuteScalar();
            return result?.ToString();
        }

        public static void AjouterUtilisateur(string username, string password, string role)
        {
            using var connection = new SqliteConnection("Data Source=utilisateurs.db");
            connection.Open();

            var command = connection.CreateCommand();
            command.CommandText = @"
                INSERT INTO utilisateurs (Username, Password, Role)
                VALUES ($username, $password, $role);
            ";

            command.Parameters.AddWithValue("$username", username);
            command.Parameters.AddWithValue("$password", password);
            command.Parameters.AddWithValue("$role", role);

            command.ExecuteNonQuery();
        }

        public static List<Utilisateur> GetAllUsers()
        {
            var utilisateurs = new List<Utilisateur>();

            using var connection = new SqliteConnection("Data Source=utilisateurs.db");
            connection.Open();

            var command = connection.CreateCommand();
            command.CommandText = "SELECT Username, Password, Role FROM utilisateurs;";

            using var reader = command.ExecuteReader();
            while (reader.Read())
            {
                utilisateurs.Add(new Utilisateur
                {
                    Username = reader.GetString(0),
                    Password = reader.GetString(1),
                    Role = reader.GetString(2)
                });
            }

            return utilisateurs;
        }

        public static void SupprimerUtilisateur(string username)
        {
            using var connection = new SqliteConnection("Data Source=utilisateurs.db");
            connection.Open();

            var command = connection.CreateCommand();
            command.CommandText = @"
                DELETE FROM utilisateurs
                WHERE Username = $username;
            ";

            command.Parameters.AddWithValue("$username", username);
            command.ExecuteNonQuery();
        }

        public static void SupprimerUser(string username)
        {
            using var connection = new SqliteConnection("Data Source=utilisateurs.db");
            connection.Open();

            var command = connection.CreateCommand();
            command.CommandText = "DELETE FROM utilisateurs WHERE Username = $username";
            command.Parameters.AddWithValue("$username", username);
            command.ExecuteNonQuery();
        }

    }
}
