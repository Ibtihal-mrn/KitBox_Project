using Microsoft.Data.Sqlite;
using System.Collections.Generic;

namespace KitBox_Project.Services
{
    public static class DatabaseCalendar
    {
        public class Appointment
        {
            public string Day { get; set; } = "";
            public string Hour { get; set; } = "";
            public string Name_of_customer { get; set; } = "";
            public string Phone_number { get; set; } = "";
            public string? Comment { get; set; }
        }

        static DatabaseCalendar()
        {
            using var connection = new SqliteConnection("Data Source=Appointments.db");
            connection.Open();
            var cmd = connection.CreateCommand();
            cmd.CommandText = @"
                CREATE TABLE IF NOT EXISTS Appointments (
                    Id INTEGER PRIMARY KEY AUTOINCREMENT,
                    Day TEXT NOT NULL,
                    Hour TEXT NOT NULL,
                    Name_of_customer TEXT NOT NULL,
                    Phone_number TEXT NOT NULL,
                    Comment TEXT
                );
            ";
            cmd.ExecuteNonQuery();
        }

        public static void AddAppointment(string day, string hour, string name, string phone, string? comment = "")
        {
            if (IsSlotTaken(day, hour))
                throw new System.Exception("Créneau déjà pris.");

            using var conn = new SqliteConnection("Data Source=Appointments.db");
            conn.Open();
            var cmd = conn.CreateCommand();
            cmd.CommandText = @"
                INSERT INTO Appointments (Day, Hour, Name_of_customer, Phone_number, Comment)
                VALUES ($day, $hour, $name, $phone, $comment);
            ";
            cmd.Parameters.AddWithValue("$day", day);
            cmd.Parameters.AddWithValue("$hour", hour);
            cmd.Parameters.AddWithValue("$name", name);
            cmd.Parameters.AddWithValue("$phone", phone);
            cmd.Parameters.AddWithValue("$comment", comment ?? "");
            cmd.ExecuteNonQuery();
        }

        public static bool IsSlotTaken(string day, string hour)
        {
            using var conn = new SqliteConnection("Data Source=Appointments.db");
            conn.Open();
            var cmd = conn.CreateCommand();
            cmd.CommandText = @"
                SELECT COUNT(*) FROM Appointments WHERE Day = $day AND Hour = $hour;
            ";
            cmd.Parameters.AddWithValue("$day", day);
            cmd.Parameters.AddWithValue("$hour", hour);
            return (long)cmd.ExecuteScalar() > 0;
        }

        public static List<Appointment> GetAllUsers()
        {
            var list = new List<Appointment>();
            using var conn = new SqliteConnection("Data Source=Appointments.db");
            conn.Open();
            var cmd = conn.CreateCommand();
            cmd.CommandText = "SELECT Day, Hour, Name_of_customer, Phone_number, Comment FROM Appointments;";
            using var reader = cmd.ExecuteReader();
            while (reader.Read())
            {
                list.Add(new Appointment
                {
                    Day = reader.GetString(0),
                    Hour = reader.GetString(1),
                    Name_of_customer = reader.GetString(2),
                    Phone_number = reader.GetString(3),
                    Comment = reader.IsDBNull(4) ? null : reader.GetString(4)
                });
            }
            return list;
        }

        public static void DeleteAppointment(string day, string hour, string name)
        {
            using var conn = new SqliteConnection("Data Source=Appointments.db");
            conn.Open();
            var cmd = conn.CreateCommand();
            cmd.CommandText = @"
                DELETE FROM Appointments 
                WHERE Name_of_customer = $name AND Hour = $hour AND Day = $day;
            ";
            cmd.Parameters.AddWithValue("$name", name);
            cmd.Parameters.AddWithValue("$hour", hour);
            cmd.Parameters.AddWithValue("$day", day);
            cmd.ExecuteNonQuery();
        }

        public static Appointment? GetAppointment(string day, string hour)
        {
            using var connection = new SqliteConnection("Data Source=Appointments.db");
            connection.Open();

            var command = connection.CreateCommand();
            command.CommandText = @"
                SELECT * FROM Appointments 
                WHERE Day = $day AND Hour = $hour
            ";
            command.Parameters.AddWithValue("$day", day);
            command.Parameters.AddWithValue("$hour", hour);

            using var reader = command.ExecuteReader();
            if (reader.Read())
            {
                return new Appointment
                {
                    Day = reader.GetString(0),
                    Hour = reader.GetString(1),
                    Name_of_customer = reader.GetString(2),
                    Phone_number = reader.GetString(3),
                    Comment = reader.IsDBNull(4) ? null : reader.GetString(4)
                };
            }
            return null;
        }

        public static void UpdateComment(string day, string hour, string comment)
        {
            using var connection = new SqliteConnection("Data Source=Appointments.db");
            connection.Open();

            var command = connection.CreateCommand();
            command.CommandText = @"
                UPDATE Appointments 
                SET Comment = $comment 
                WHERE Day = $day AND Hour = $hour;
            ";
            command.Parameters.AddWithValue("$comment", comment);
            command.Parameters.AddWithValue("$day", day);
            command.Parameters.AddWithValue("$hour", hour);
            command.ExecuteNonQuery();
        }

    }
}
