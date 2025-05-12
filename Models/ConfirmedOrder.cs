using System.Collections.Generic;
using System.Linq;

namespace KitBox_Project.Models
{
    public class ConfirmedOrder
    {
        public string OrderId { get; set; } // Identifiant unique de la commande (par exemple, "Commande 001")
        public List<Article> Articles { get; set; } // Liste des articles de la commande

        // Constructeur
        public ConfirmedOrder(string orderId)
        {
            OrderId = orderId;
            Articles = new List<Article>();
        }

        // Propriété calculée : somme des quantités de tous les articles de la commande
        public int TotalQuantity => Articles?.Sum(a => a.Quantity) ?? 0;
    }
}
