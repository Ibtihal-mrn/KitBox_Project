
using Avalonia.Controls;
using KitBox_Project.ViewModel;

namespace KitBox_Project.View;

public partial class MainWindow : Window
{
    public MainWindow()
    {
        InitializeComponent();
        this.DataContext = new MainViewModel(); // Lier le ViewModel à la fenêtre principale
    }
}