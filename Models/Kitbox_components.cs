namespace AvaloniaApplication1.Models
{
    public class Kitbox_components
    {
        public int Id { get; set; } // Primary Key, auto-increment
        public string? Reference { get; set; }
        public string? Code { get; set; }
        public string? Dimensions { get; set; }
        public float LengthCm { get; set; }
        public float WidthCm { get; set; }
        public float DepthCm { get; set; }
        public float PriceSupplier1 { get; set; }
        public int DelaySupplier1 { get; set; }
        public float PriceSupplier2 { get; set; }
        public int DelaySupplier2 { get; set; }
        public float SellingPrice { get; set; }
        public int NumberOfPiecesAvailable { get; set; }
        public string? Color { get; set; }
    }
}
