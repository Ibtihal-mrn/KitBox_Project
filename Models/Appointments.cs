public class Appointment
{
    public int Id { get; set; }  // Pour la base SQLite
    public string Day { get; set; }
    public string Hour { get; set; }
    public string Name { get; set; }
    public string PhoneNumber { get; set; }

    public override string ToString() => $"{Hour} - {Name} ({PhoneNumber})";
}
