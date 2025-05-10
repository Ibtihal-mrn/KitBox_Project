using System;
using System.Collections.Generic;
using System.Linq;
using KitBox_Project.Models;
using MySql.Data.MySqlClient;


namespace KitBox_Project.Data
{
    public class DataAccess
    {
        // On suppose que StaticArticleDatabase.AllArticles est déjà rempli avant tout appel.

        public List<Article> GetArticles()
        {
            var articles = StaticArticleDatabase.AllArticles.ToList();
            try
            {
                Console.WriteLine("[DEBUG] Début de GetArticles");
                foreach (var a in articles.Where(a => a.Reference?.IndexOf("panel horizontal", StringComparison.OrdinalIgnoreCase) >= 0))
                {
                    Console.WriteLine($"[DEBUG] Article chargé : Ref={a.Reference}, L={a.Length}, P={a.Depth}, C={a.Color}, Stock={a.NumberOfPiecesAvailable}");
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"❌ Erreur dans GetArticles : {ex.Message}");
            }
            return articles;
        }

        public List<Article> GetLengthOfPanelHorizontal()
        {
            Console.WriteLine("[DEBUG] Début de GetLengthOfPanelHorizontal");
            var all = StaticArticleDatabase.AllArticles;
            Console.WriteLine($"[DEBUG] Total articles en base : {all.Count}");

            // Étape 1 : panel horizontal
            var step1 = all.Where(p => p.Reference?.IndexOf("panel horizontal", StringComparison.OrdinalIgnoreCase) >= 0).ToList();
            Console.WriteLine($"[DEBUG] panel horizontal trouvés : {step1.Count}");

            // Étape 2 : stock > 0
            var step2 = step1.Where(p => p.NumberOfPiecesAvailable > 0).ToList();
            Console.WriteLine($"[DEBUG] Avec stock > 0 : {step2.Count}");

            // Étape 3 : couleur
            var step3 = step2.Where(p => p.Color?.Equals(AppState.SelectedColor, StringComparison.OrdinalIgnoreCase) ?? false).ToList();
            Console.WriteLine($"[DEBUG] Avec couleur {AppState.SelectedColor} : {step3.Count}");

            // Étape 4 : crossbar front, même longueur
            var step4 = step3.Where(p => all.Any(c =>
                c.Reference?.IndexOf("crossbar front", StringComparison.OrdinalIgnoreCase) >= 0 &&
                c.Length == p.Length &&
                c.NumberOfPiecesAvailable > 0)).ToList();
            Console.WriteLine($"[DEBUG] Avec 'crossbar front' même longueur : {step4.Count}");

            // Étape 5 : crossbar back, même longueur
            var step5 = step4.Where(p => all.Any(c =>
                c.Reference?.IndexOf("crossbar back", StringComparison.OrdinalIgnoreCase) >= 0 &&
                c.Length == p.Length &&
                c.NumberOfPiecesAvailable > 0)).ToList();
            Console.WriteLine($"[DEBUG] Avec 'crossbar back' même longueur : {step5.Count}");

            var result = step5.Select(p => new Article { Length = p.Length }).ToList();
            Console.WriteLine($"[DEBUG] Résultat final : {result.Count}");
            if (result.Count == 0)
                Console.WriteLine("La colonne 'Length' est considérée comme nulle pour cette requête (aucun résultat).");
            return result;
        }

        public List<Article> GetDepthOfPanelHorizontal(int length)
        {
            Console.WriteLine($"[DEBUG] Début de GetDepthOfPanelHorizontal pour Length = {length}");
            var all = StaticArticleDatabase.AllArticles;

            var result = all
                .Where(p => p.Reference?.IndexOf("panel horizontal", StringComparison.OrdinalIgnoreCase) >= 0)
                .Where(p => p.Length == length)
                .Where(p => p.NumberOfPiecesAvailable > 0)
                .Where(p => p.Color == AppState.SelectedColor)
                .Where(p => all.Any(c => 
                    c.Reference?.IndexOf("crossbar left", StringComparison.OrdinalIgnoreCase) >= 0 &&
                    c.Depth == p.Depth &&
                    c.NumberOfPiecesAvailable > 1))
                .Select(p => new Article { Depth = p.Depth })
                .ToList();

            Console.WriteLine($"[DEBUG] Résultat GetDepthOfPanelHorizontal : {result.Count}");
            return result;
        }

        public bool IsStockLow(int length, int depth)
        {
            Console.WriteLine("[DEBUG] Début de IsStockLow");
            var all = StaticArticleDatabase.AllArticles;

            bool low = all
                .Where(a =>
                    (a.Reference?.IndexOf("panel back", StringComparison.OrdinalIgnoreCase) >= 0 &&
                     a.Length == length && a.Color == AppState.SelectedColor)
                    ||
                    ((a.Reference?.IndexOf("panel left", StringComparison.OrdinalIgnoreCase) >= 0 ||
                      a.Reference?.IndexOf("panel right", StringComparison.OrdinalIgnoreCase) >= 0) &&
                     (a.Dimensions?.Contains($"{depth}x{length}", StringComparison.OrdinalIgnoreCase) ?? false) &&
                     a.Color == AppState.SelectedColor)
                )
                .Any(a => a.NumberOfPiecesAvailable <= 5);

            Console.WriteLine($"[DEBUG] Stock faible : {low}");
            return low;
        }

        public List<Article> GetHeightOfPanel(int length, int depth, string color)
        {
            Console.WriteLine("[DEBUG] Début de GetHeightOfPanel");
            var all = StaticArticleDatabase.AllArticles;
            Console.WriteLine($"[DEBUG] Total articles en base : {all.Count}");

            // Vérifions si les panneaux existent
            var panelsBack = all.Where(p => p.Reference?.StartsWith("panel back", StringComparison.OrdinalIgnoreCase) ?? false).ToList();
            var panelsSide = all.Where(p => p.Reference?.StartsWith("panel left or right", StringComparison.OrdinalIgnoreCase) ?? false).ToList();
            var battens = all.Where(p => p.Reference?.StartsWith("vertical batten", StringComparison.OrdinalIgnoreCase) ?? false).ToList();
            
            Console.WriteLine($"[DEBUG] Total PAR panels: {panelsBack.Count}");
            Console.WriteLine($"[DEBUG] Total PAG panels: {panelsSide.Count}");
            Console.WriteLine($"[DEBUG] Total TAS battens: {battens.Count}");
            Console.WriteLine($"[DEBUG] Paramètres: length={length}, depth={depth}, color={color}");
            
            // Afficher quelques exemples d'articles pour vérifier la structure
            if (panelsBack.Any())
            {
                var sample = panelsBack.First();
                Console.WriteLine($"[DEBUG] Exemple PAR: Ref={sample.Reference}, L={sample.Length}, H={sample.Height}, D={sample.Depth}, C={sample.Color}");
            }
            if (panelsSide.Any())
            {
                var sample = panelsSide.First();
                Console.WriteLine($"[DEBUG] Exemple PAG: Ref={sample.Reference}, L={sample.Length}, H={sample.Height}, D={sample.Depth}, C={sample.Color}");
            }
            if (battens.Any())
            {
                var sample = battens.First();
                Console.WriteLine($"[DEBUG] Exemple TAS: Ref={sample.Reference}, L={sample.Length}, H={sample.Height}, D={sample.Depth}, C={sample.Color}");
            }

            // Étape 1 : panels (back, left, or right) avec color et dimensions cohérentes
            var backPanels = all
                .Where(p => 
                    (p.Reference?.StartsWith("panel back", StringComparison.OrdinalIgnoreCase) ?? false) &&
                    p.Color?.Equals(color, StringComparison.OrdinalIgnoreCase) == true &&
                    p.Length == length && 
                    p.NumberOfPiecesAvailable > 0)
                .ToList();
            
            var sidePanels = all
                .Where(p => 
                    (p.Reference?.StartsWith("panel left", StringComparison.OrdinalIgnoreCase) ?? false) &&
                    p.Color?.Equals(color, StringComparison.OrdinalIgnoreCase) == true &&
                    p.Depth == depth &&
                    p.NumberOfPiecesAvailable > 0)
                .ToList();
            
            var step1 = backPanels.Concat(sidePanels).ToList();
            
            Console.WriteLine($"[DEBUG] Back panels trouvés: {backPanels.Count}");
            Console.WriteLine($"[DEBUG] Side panels trouvés: {sidePanels.Count}");
            Console.WriteLine($"[DEBUG] Step1 (panel back/left/right) : {step1.Count}");

            // Afficher les hauteurs disponibles
            var heights = step1.Select(p => p.Height).Distinct().ToList();
            Console.WriteLine($"[DEBUG] Hauteurs disponibles: {string.Join(", ", heights)}");

            // Étape 2 : vertical battens correspondants en hauteur et stock suffisant (≥ 2)
            var step2 = new List<Article>();
            foreach (var panel in step1)
            {
                var matchingBattens = all.Count(c =>
                    c.Reference?.StartsWith("vertical batten", StringComparison.OrdinalIgnoreCase) ?? false &&
                    c.Height == panel.Height &&
                    c.NumberOfPiecesAvailable > 0);
                    
                Console.WriteLine($"[DEBUG] Panel hauteur {panel.Height} - battens correspondants: {matchingBattens}");
                
                if (matchingBattens >= 2)
                {
                    step2.Add(panel);
                }
            }
            
            Console.WriteLine($"[DEBUG] Step2 (2+ vertical battens) : {step2.Count}");

            // Résultat final : hauteurs disponibles distinctes
            var result = step2
                .Select(p => new Article { Height = p.Height })
                .GroupBy(p => p.Height)
                .Select(g => g.First())
                .ToList();
                
            Console.WriteLine($"[DEBUG] Résultat final GetHeightOfPanel : {result.Count}");
            if (result.Count > 0)
            {
                Console.WriteLine($"[DEBUG] Hauteurs retenues: {string.Join(", ", result.Select(r => r.Height))}");
            }

            return result;
        }


        public List<Article> GetAvailableDoors(int height, int length)
        {
            Console.WriteLine($"[DEBUG] Début de GetAvailableDoors pour H={height}, L={length}");
            var result = StaticArticleDatabase.AllArticles
                .Where(d => d.Reference?.IndexOf("door", StringComparison.OrdinalIgnoreCase) >= 0)
                .Where(d => d.NumberOfPiecesAvailable > 0)
                .Where(d => d.Height == height && d.Length == length)
                .Select(d => new Article {
                    Reference = d.Reference,
                    Code = d.Code,
                    Color = d.Color,
                    Height = d.Height,
                    Length = d.Length,
                    Depth = d.Depth
                })
                .ToList();

            Console.WriteLine($"[DEBUG] Portes trouvées : {result.Count}");
            return result;
        }

        public List<Article> GetAngleIron(int height)
        {
            Console.WriteLine("[DEBUG] Début de GetAngleIron");
            var result = StaticArticleDatabase.AllArticles
                .Where(a => a.Reference?.IndexOf("angle iron", StringComparison.OrdinalIgnoreCase) >= 0)
                .Where(a => a.Height == height)
                .ToList();

            Console.WriteLine($"[DEBUG] Angle irons trouvés : {result.Count}");
            return result;
        }

        public List<LowStockItem> GetLowStockItems(int length, int depth)
        {
            Console.WriteLine("[DEBUG] Début de GetLowStockItems");
            var result = StaticArticleDatabase.AllArticles
                .Where(a =>
                    a.Reference?.IndexOf("panel left", StringComparison.OrdinalIgnoreCase) >= 0 &&
                    (a.Dimensions?.Contains($"{depth}x{length}", StringComparison.OrdinalIgnoreCase) ?? false) &&
                    a.Color == AppState.SelectedColor &&
                    (a.NumberOfPiecesAvailable <= 5 || a.NumberOfPiecesAvailable == 0))
                .Select(a => new LowStockItem {
                    Reference = a.Reference,
                    AvailableQuantity = a.NumberOfPiecesAvailable,
                    Length = a.Length,
                    Depth = a.Depth,
                    Height = a.Height
                })
                .ToList();

            Console.WriteLine($"[DEBUG] LowStockItems : {result.Count}");
            return result;
        }

        public Dictionary<string, int> GetAngleIronStockByHeight(int height)
        {
            Console.WriteLine("[DEBUG] Début de GetAngleIronStockByHeight");
            var dict = StaticArticleDatabase.AllArticles
                .Where(a => a.Reference?.IndexOf("angle iron", StringComparison.OrdinalIgnoreCase) >= 0)
                .Where(a => a.Height == height)
                .GroupBy(a => a.Color)
                .Where(g => g.Key != null)
                .ToDictionary(g => g.Key!, g => g.Sum(a => a.NumberOfPiecesAvailable));

            foreach (var kv in dict)
                Console.WriteLine($"[DEBUG] Couleur : {kv.Key}, Quantité : {kv.Value}");
            return dict;
        }
        
        private int? TryExtractHeightFromDimension(string? dimensions)
        {
            if (string.IsNullOrEmpty(dimensions))
                return null;

            var parts = dimensions.Split('x');
            if (parts.Length == 2 && int.TryParse(parts[0], out int height))
                return height;

            return null;
        }
        

        public class LowStockItem
        {
            public string? Reference { get; set; }
            public int AvailableQuantity { get; set; }
            public int Length { get; set; }
            public int Depth { get; set; }
            public int Height { get; set; }
        }
        
    }
}