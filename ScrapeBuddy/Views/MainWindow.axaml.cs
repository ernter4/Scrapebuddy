using Avalonia.Controls;
using ScrapeBuddy.Models;
namespace ScrapeBuddy.Views;

public partial class MainWindow : Window
{
    
    public MainWindow()
    {
        InitializeComponent();
        DataContext = new Websites();
    }
    
}