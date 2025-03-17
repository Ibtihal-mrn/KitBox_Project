namespace KitBox_Project.Models
{
    public class Article
    {
        public string? Code{ get; set; } // Clé primaire (PK)
        public string? Reference { get; set; }
        public string? Color{ get; set; }
        public string? Dimensions { get; set; } // Stocke les dimensions sous forme de chaîne (ex: "100x50x30")
        public int Length { get; set; }
        public int Height { get; set; }
        public int Depth { get; set; }
        public decimal PriceSupplierUno { get; set; }
        public int DelaySupplierUno { get; set; }
        public decimal PriceSupplierDos { get; set; }
        public int DelaySupplierDos { get; set; }
        public decimal SellingPrice { get; set; }
        public int NumberOfPiecesAvailable { get; set; }
    }
}
