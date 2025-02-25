using Avalonia.Controls;

namespace KitBox_Project
{
    public partial class MainWindow : Window
    {
<<<<<<< HEAD
        InitializeComponent();
        var textBlock = new TextBlock
        {
           Text = "Bikass La Menace" ,
           HorizontalAlignment = Avalonia.Layout.HorizontalAlignment.Center,
           VerticalAlignment = Avalonia.Layout.VerticalAlignment.Center,
        };
        this.Content = textBlock;
    }

    private static Grid CreateGrid(){
        var grid = new Grid();
        grid.ColumnDefinitions.Add(new ColumnDefinition(100));
        grid.ColumnDefinitions.Add(new ColumnDefinition(100));
        grid.RowDefinitions.Add(new RowDefinition(100));
        grid.RowDefinitions.Add(new RowDefinition(100));
        return grid; 
=======
        public MainWindow()
        {
            InitializeComponent();
        }

        private void GoToSize(object sender, Avalonia.Interactivity.RoutedEventArgs e)
        {
            var sizeWindow = new Size();  // Crée la fenêtre Size
            Fonctions.NavigateToPage(this, sizeWindow);  // Utilise la méthode dans NavigationHelper
        }
>>>>>>> origin/Configuration_armoire
    }
}
