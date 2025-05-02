using Microsoft.Data.Sqlite;
using System.Collections.Generic;

public static class DatabaseCalendar
{
    private static string connectionString = "Data Source=appointments.db";

    public static void InitializeDatabase()
    {
        using var connection = new SqliteConnection(connectionString);
        connection.Open();

        var command = connection.CreateCommand();
        command.CommandText = @"
            CREATE TABLE IF NOT EXISTS Appointments (
                Id INTEGER PRIMARY KEY AUTOINCREMENT,
                Day TEXT,
                Hour TEXT,
                Name TEXT,
                PhoneNumber TEXT
            );";
        command.ExecuteNonQuery();
    }

    public static void AddAppointment(Appointment appointment)
    {
        using var connection = new SqliteConnection(connectionString);
        connection.Open();

        var command = connection.CreateCommand();
        command.CommandText = @"
            INSERT INTO Appointments (Day, Hour, Name, PhoneNumber)
            VALUES ($day, $hour, $name, $phone)";
        command.Parameters.AddWithValue("$day", appointment.Day);
        command.Parameters.AddWithValue("$hour", appointment.Hour);
        command.Parameters.AddWithValue("$name", appointment.Name);
        command.Parameters.AddWithValue("$phone", appointment.PhoneNumber);
        command.ExecuteNonQuery();
    }

    public static List<Appointment> GetAppointmentsByDay(string day)
    {
        var result = new List<Appointment>();
        using var connection = new SqliteConnection(connectionString);
        connection.Open();

        var command = connection.CreateCommand();
        command.CommandText = "SELECT * FROM Appointments WHERE Day = $day";
        command.Parameters.AddWithValue("$day", day);

        using var reader = command.ExecuteReader();
        while (reader.Read())
        {
            result.Add(new Appointment
            {
                Id = reader.GetInt32(0),
                Day = reader.GetString(1),
                Hour = reader.GetString(2),
                Name = reader.GetString(3),
                PhoneNumber = reader.GetString(4)
            });
        }

        return result;
    }
}
