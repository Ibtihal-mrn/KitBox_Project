using ReactiveUI;
using System.Windows.Input;
using KitBox_Project.Helpers;
using System;
//test

namespace KitBox_Project.ViewModels
{
    public class FindMyInvoiceViewModel : ReactiveObject
    {
        public ICommand GeneratorInvoiceCommand { get; }

        public FindMyInvoiceViewModel()
        {
            GeneratorInvoiceCommand = ReactiveCommand.Create(GenerateInvoice);
        }

        private void GenerateInvoice()
        {
            try
            {
                InvoiceGenerator.GenerateSampleInvoice();
                Console.WriteLine("✅ Invoice generated successfully.");
            }
            catch (Exception ex)
            {
                Console.WriteLine("❌ Error generating invoice: " + ex.Message);
            }
        }

    }
}
