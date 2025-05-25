using Avalonia.Controls;

namespace KitBox_Project.Views
{
    public partial class Inventory : UserControl
    {
        public Inventory()
        {
            InitializeComponent();
            DataContext = new ViewModels.InventoryViewModel();
        }
    }
}
