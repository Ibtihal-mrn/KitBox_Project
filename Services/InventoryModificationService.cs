using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text.Json;
using KitBox_Project.Models;
using KitBox_Project.Services;

public static class InventoryModificationService
{
    private static readonly string Dir = Path.Combine("data", "inventory");
    private static readonly string CurrentFile = Path.Combine(Dir, "inventory_current.json");

    static InventoryModificationService()
    {
        Directory.CreateDirectory(Dir);
        if (!File.Exists(CurrentFile))
            File.WriteAllText(CurrentFile, "[]");
    }

    public static List<InventoryAdjustment> LoadCurrent() =>
        JsonSerializer.Deserialize<List<InventoryAdjustment>>(File.ReadAllText(CurrentFile))
        ?? new List<InventoryAdjustment>();

    public static void SaveCurrent(List<InventoryAdjustment> adjustments)
    {
        var json = JsonSerializer.Serialize(adjustments, new JsonSerializerOptions { WriteIndented = true });
        File.WriteAllText(CurrentFile, json);
    }

    public static void SnapshotCurrent()
    {
        var timestamp = DateTime.Now.ToString("yyyyMMdd'T'HHmmss");
        var archive = Path.Combine(Dir, $"inventory_{timestamp}.json");

        if (File.Exists(archive))
        {
            var guid = Guid.NewGuid().ToString().Substring(0, 8);
            archive = Path.Combine(Dir, $"inventory_{timestamp}_{guid}.json");
        }

        File.Copy(CurrentFile, archive, false);
        File.WriteAllText(CurrentFile, "[]");
    }

    /// <summary>
    /// Lit tous les fichiers snapshot + current et renvoie l’ensemble des ajustements.
    /// </summary>
    public static List<InventoryAdjustment> LoadAll()
    {
        var all = new List<InventoryAdjustment>();
        foreach (var file in Directory.GetFiles(Dir, "*.json"))
        {
            Console.WriteLine($"[DEBUG] Chargement JSON inventaire : {file}");
            var json = File.ReadAllText(file);
            var list = JsonSerializer.Deserialize<List<InventoryAdjustment>>(json)
                    ?? new List<InventoryAdjustment>();
            Console.WriteLine($"[DEBUG]   ↳ {list.Count} ajustement(s) dans ce fichier");
            all.AddRange(list);
        }
        Console.WriteLine($"[DEBUG] Total ajustements inventaire chargés : {all.Count}");
        return all;
    }
}
