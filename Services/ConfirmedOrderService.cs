using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text.Json;
using System.Threading.Tasks;
using KitBox_Project.Models;

namespace KitBox_Project.Services
{
    public static class ConfirmedOrderService
    {
        private static readonly string OrdersDir = Path.Combine("data", "orders");
        private static readonly string CurrentFile = Path.Combine(OrdersDir, "orders_current.json");
        private static int _orderCount;

        static ConfirmedOrderService()
        {
            Directory.CreateDirectory(OrdersDir);
            if (!File.Exists(CurrentFile))
                File.WriteAllText(CurrentFile, "[]");

            var all = LoadAllOrders();
            _orderCount = all
                .Select(o => {
                    var parts = o.OrderId.Split(' ');
                    return parts.Length > 1 && int.TryParse(parts[1], out var n) ? n : 0;
                })
                .DefaultIfEmpty(0)
                .Max();
        }

        public static List<ConfirmedOrder> LoadCurrentOrders()
        {
            var json = File.ReadAllText(CurrentFile);
            return JsonSerializer.Deserialize<List<ConfirmedOrder>>(json) ?? new List<ConfirmedOrder>();
        }

        public static List<ConfirmedOrder> LoadAllOrders()
        {
            var all = new List<ConfirmedOrder>();
            var snaps = Directory.GetFiles(OrdersDir, "orders_*.json")
                                .Where(f => !f.EndsWith("orders_current.json"));
            foreach (var file in snaps)
            {
                try
                {
                    var list = JsonSerializer.Deserialize<List<ConfirmedOrder>>(File.ReadAllText(file))
                               ?? new List<ConfirmedOrder>();
                    all.AddRange(list);
                }
                catch { }
            }
            all.AddRange(LoadCurrentOrders());
            return all.GroupBy(o => o.OrderId).Select(g => g.First()).ToList();
        }

        public static void SnapshotCurrentOrders()
        {
            var ts = DateTime.Now.ToString("yyyyMMdd'T'HHmmss");
            var archive = Path.Combine(OrdersDir, $"orders_{ts}.json");
            if (File.Exists(archive))
                archive = Path.Combine(OrdersDir, $"orders_{ts}_{Guid.NewGuid():N8}.json");
            File.Copy(CurrentFile, archive, false);
            File.WriteAllText(CurrentFile, "[]");
        }

        public static string GenerateOrderId()
        {
            _orderCount++;
            return $"Order {_orderCount:D3}";
        }

        public static void SaveConfirmedOrder(ConfirmedOrder order)
        {
            var list = LoadCurrentOrders();
            list.Add(order);
            File.WriteAllText(CurrentFile, JsonSerializer.Serialize(list, new JsonSerializerOptions { WriteIndented = true }));
            Console.WriteLine($"✅ Commande {order.OrderId} sauvegardée");
        }

                public static async Task ApplyAllOrdersToStockAsync()
        {
            var all = LoadAllOrders();
            await StockService.ResetStockToOriginalAsync();
            foreach (var o in all)
                StockService.UpdateStock(o.Articles);
            Console.WriteLine($"[Stock] {all.Count} commandes appliquées (async)");
        }

        public static void ApplyAllOrdersToStock()
        {
            ApplyOrdersOnly();
        }

        public static void ApplyOrdersOnly()
        {
            var all = LoadAllOrders();
            foreach (var o in all)
                StockService.UpdateStock(o.Articles);
        }


        public static ConfirmedOrder? GetLastConfirmedOrder()
        {
            return LoadCurrentOrders().LastOrDefault();
        }
    }
}