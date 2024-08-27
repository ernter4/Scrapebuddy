using System;
using System.Timers;
using Avalonia.Controls;
using ScrapeBuddy.Models;
using ScrapeBuddy.Logic;
namespace ScrapeBuddy.Views;

public partial class MainWindow : Window
{
    private Timer _timer;
    
    public MainWindow()
    {
        InitializeComponent();
        SetupTimer();
        DataContext = new Websites();
    }
    
    private void SetupTimer()
    {
        // Create and configure the time
        _timer = new Timer
        {
            Interval = 300000 // 5 minutes in milliseconds
        };
        _timer.Elapsed += OnTimedEvent;
        _timer.Start();
    }
    
    private void OnTimedEvent(object source, ElapsedEventArgs e)
    {
        // Call your method here
        Console.WriteLine("Start scraping Now");
        Scrape.ScrapeItems();
    }
    
    protected override void OnClosed(EventArgs e)
    {
        _timer?.Dispose();
        base.OnClosed(e);
    }
    
}