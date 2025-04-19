using QuestPDF.Fluent;
using QuestPDF.Helpers;
using QuestPDF.Infrastructure;
using System.IO;
using System;

namespace KitBox_Project.Helpers
{
    public static class InvoiceGenerator
    {
        public static void GenerateSampleInvoice()
        {
            var document = Document.Create(container =>
            {
                container.Page(page =>
                {
                    page.Size(PageSizes.A4);
                    page.Margin(2, Unit.Centimetre);
                    page.DefaultTextStyle(x => x.FontSize(20));

                    page.Content().Column(col =>
                    {
                        col.Item().Text("Sample Invoice").Bold().FontSize(30);
                        col.Item().Text("Thank you for your order!");
                        
                        // Utilisation d'un Container pour ajouter du Padding autour du TextBlock
                        col.Item().Container().Padding(10).Column(c =>
                        {
                            c.Item().Text("Sample Invoice Description").FontSize(16);
                        });

                        // Création d'un tableau avec padding
                        col.Item().Table(table =>
                        {
                            table.ColumnsDefinition(columns =>
                            {
                                columns.RelativeColumn();
                                columns.ConstantColumn(100);
                            });

                            table.Header(header =>
                            {
                                header.Cell().Padding(5).Background("#eee").Text("Item").SemiBold();
                                header.Cell().Padding(5).Background("#eee").Text("Price").SemiBold();
                            });

                            for (int i = 1; i <= 3; i++)
                            {
                                table.Cell().Padding(5).Text($"Item {i}");
                                table.Cell().Padding(5).Text($"${i * 10}");
                            }
                        });
                    });
                });
            });

            // Création du chemin de sortie
            var outputPath = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.Desktop), "invoice.pdf");


            // Vérification que le répertoire existe
            string? directoryPath = Path.GetDirectoryName(outputPath);
            if (!string.IsNullOrEmpty(directoryPath))
            {
                Directory.CreateDirectory(directoryPath); // Crée le dossier si nécessaire
            }

            // Génération du PDF
            QuestPDF.Settings.License = LicenseType.Community;
            document.GeneratePdf(outputPath);
        }
    }
} 