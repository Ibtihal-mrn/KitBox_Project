public class InventoryAdjustment
{
    public string Code { get; set; } = "";
    public string Color { get; set; } = "";
    public int Height { get; set; }
    /// <summary>
    /// Delta à ajouter (peut être négatif)
    /// </summary>
    public int Delta { get; set; }
}
