namespace Backend.Models
{
    public class Fournisseur
    {
        public int PK_Numero_fournisseur { get; set; }
        public required string Nom { get; set; }
        public required string Adresse { get; set; }
        public required string Code_postal { get; set; }
        public required string Ville { get; set; }
        public required string Tel { get; set; }
    }
}