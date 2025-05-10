public class Appointment
{
    public int Id { get; set; }  // Pour la base SQLite
    public required string Day { get; set; }
    public required string Hour { get; set; }
    public required string Name { get; set; }
    public string? PhoneNumber { get; set; }

    public override string ToString() => $"{Hour} - {Name} ({PhoneNumber})";
}
