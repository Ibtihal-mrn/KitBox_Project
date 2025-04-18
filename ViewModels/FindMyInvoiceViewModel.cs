using ReactiveUI;
using System.Windows.Input;
using KitBox_Project.Helpers; // Remplace par le bon namespace de InvoiceGenerator si nécessaire

namespace KitBox_Project.ViewModels // Ou KitBox_Project.ViewModels, selon ta structure réelle
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
            InvoiceGenerator.GenerateSampleInvoice();
        }
    }
}
