using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text.Json;
using KitBox_Project.Models;

namespace KitBox_Project.Services
{
    public static class ConfirmedOrderService
    {
        private static readonly string FilePath = "confirmed_orders.json";
        private static int _orderCount = 0;

        public static List<ConfirmedOrder> LoadConfirmedOrders()
        {
            if (!File.Exists(FilePath)) return new List<ConfirmedOrder>();
            try
            {
                var json = File.ReadAllText(FilePath);
                var orders = JsonSerializer.Deserialize<List<ConfirmedOrder>>(json)
                             ?? new List<ConfirmedOrder>();

                // Met à jour _orderCount pour repartir de la valeur la plus haute
                if (orders.Count > 0)
                {
                    int max = orders
                        .Select(o => int.Parse(o.OrderId.Split(' ')[1]))
                        .Max();
                    _orderCount = max;
                }
                return orders;
            }
            catch
            {
                return new List<ConfirmedOrder>();
            }
        }

        public static void SaveConfirmedOrder(ConfirmedOrder order)
        {
            var orders = LoadConfirmedOrders();

            // Duplication défensive des articles pour éviter les références croisées
            var safeOrder = new ConfirmedOrder(order.OrderId)
            {
                Articles = order.Articles.Select(a => new Article
                {
                    Reference = a.Reference,
                    Color = a.Color,
                    Quantity = a.Quantity,
                    Code = a.Code
                }).ToList()
            };

            orders.Add(safeOrder);

            var json = JsonSerializer.Serialize(orders,
                       new JsonSerializerOptions { WriteIndented = true });
            File.WriteAllText(FilePath, json);
        }

        public static string GenerateOrderId()
        {
            _orderCount++;
            return $"Commande {_orderCount:D3}";
        }
    }
}
