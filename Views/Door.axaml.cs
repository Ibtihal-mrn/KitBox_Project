using Avalonia;  
using Avalonia.Controls;
using Avalonia.Media;
using Avalonia.Media.Imaging;
using Avalonia.Layout;

namespace KitBox_Project
{
    public partial class Door : Window
    {
        public Door()
        {
            InitializeComponent();
            this.Content = CreateGrid();
        }



        //Next = Choice
        private void GoToChoice(object? sender, Avalonia.Interactivity.RoutedEventArgs e)
        {
            var choiceWindow = new Choice();  // Crée une nouvelle instance de MainWindow
            Fonctions.NavigateToPage(this, choiceWindow);  // Utilise la méthode dans NavigationHelper
        }

        private void GoToMainWindow(object sender, Avalonia.Interactivity.RoutedEventArgs e)
        {
            var mainWindow = new MainWindow();  // Crée une nouvelle instance de MainWindow
            Fonctions.NavigateToPage(this, mainWindow);  // Utilise la méthode dans NavigationHelper
        }

        //Back = Color
        private void GoToColor(object sender, Avalonia.Interactivity.RoutedEventArgs e)
        {
            var colorWindow = new Color();  // Crée une nouvelle instance de MainWindow
            Fonctions.NavigateToPage(this, colorWindow);  // Utilise la méthode dans NavigationHelper
        }


        private Grid CreateGrid()
        {
            var grid = new Grid
            {
                Name = "grid1",
                ShowGridLines = true,
                VerticalAlignment = Avalonia.Layout.VerticalAlignment.Top,
                HorizontalAlignment = Avalonia.Layout.HorizontalAlignment.Center
            };

            // Make columns take full width
            grid.ColumnDefinitions.Add(new ColumnDefinition(new GridLength(350))); // First column
            grid.ColumnDefinitions.Add(new ColumnDefinition(new GridLength(250))); // Second column
            grid.ColumnDefinitions.Add(new ColumnDefinition(new GridLength(400))); // Third column
            // Make rows take full height
            grid.RowDefinitions.Add(new RowDefinition(new GridLength(200))); // First row
            grid.RowDefinitions.Add(new RowDefinition(new GridLength(200))); // Second row
            grid.RowDefinitions.Add(new RowDefinition(new GridLength(300))); // Third row
            grid.RowDefinitions.Add(new RowDefinition(new GridLength(150))); // Third row


            var image = new Image
            {
                Source = new Bitmap("D:/BAC 3/Software engineering 2/Projet_Kitbox/KitBox_Project/Ressources/GlassDoor.png"), // Replace with the actual path to your image
                Stretch = Avalonia.Media.Stretch.UniformToFill // This will make the image fill the space proportionally
            };

            // Add the Image to the first column (left side)
            grid.Children.Add(image);
            Grid.SetColumn(image, 0); // Place the image in the first column (left part)
            Grid.SetRow(image, 1); // Place the image in the second row
            Grid.SetColumnSpan(image, 2); // Make the image span across the first and second columns
            Grid.SetRowSpan(image, 2); // Make the image span across the rows

            var button = new Button
            {
                 Content = "Glass door", // Text displayed on the button
                VerticalAlignment = VerticalAlignment.Center, // Center the button vertically
                HorizontalAlignment = HorizontalAlignment.Center, // Center the button horizontally
                Background = new SolidColorBrush(Colors.Blue), // Button background color
                BorderBrush = new SolidColorBrush(Colors.Black), // Border color
                BorderThickness = new Avalonia.Thickness(2), // Border thickness
                CornerRadius = new CornerRadius(10), // Rounded corners
                Padding = new Avalonia.Thickness(20), // Padding inside the button
                FontSize = 18, // Font size
                FontWeight = FontWeight.Bold, // Bold text
                Foreground = new SolidColorBrush(Colors.White), // Text color
            };

            // Hover effect - change background color when the mouse is over the button
            button.PointerEntered += (sender, args) =>
            {
                button.Background = new SolidColorBrush(Colors.DarkOrange); // Darken background on hover
            };

            // Reset the background color when the mouse leaves the button
            button.PointerExited += (sender, args) =>
            {
                button.Background = new SolidColorBrush(Colors.Blue); // Reset background color
            };

            // Bind the Button click event to the GoToChoice method
            button.Click += GoToChoice;

            // Add the button below the image (in the third row)
            grid.Children.Add(button);
            Grid.SetColumn(button, 0); // Place the button in the first column
            Grid.SetRow(button, 3); // Place the button in the third row
            Grid.SetColumnSpan(button, 2); // Make the button span across the first and second columns

            // Center the button horizontally across the two columns
            button.HorizontalAlignment = HorizontalAlignment.Center;
            button.VerticalAlignment = VerticalAlignment.Center;
            
            return grid;
        }       
        
    }
}
