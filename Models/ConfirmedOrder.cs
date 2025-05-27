using System.Collections.Generic;
using System.Linq;

namespace KitBox_Project.Models
{
    public class ConfirmedOrder
    {
        public string OrderId { get; set; } 
        public List<Article> Articles { get; set; } 


        public ConfirmedOrder(string orderId)
        {
            OrderId = orderId;
            Articles = new List<Article>();
        }


        public int TotalQuantity => Articles?.Sum(a => a.Quantity) ?? 0;
        

        public decimal TotalPrice => Articles?.Sum(a => a.TotalPrice) ?? 0m;

    }
}
