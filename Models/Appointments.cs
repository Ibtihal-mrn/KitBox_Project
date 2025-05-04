public class Appointment
{
    public int Id { get; set; }  = 0 ; // Pour la base SQLite
    public string Day { get; set; } = string.Empty;
    public string Hour { get; set; } = string.Empty;
    public string Name { get; set; } = string.Empty;
    public string PhoneNumber { get; set; } = string.Empty;

    public override string ToString() => $"{Hour} - {Name} ({PhoneNumber})";
}
