using System;
using Avalonia;
using Avalonia.Controls;
using Avalonia.Interactivity;
using Avalonia.Markup.Xaml;

namespace ScrapeBuddy.ViewModels;

public partial class DeleteWebsite : Window
{
    public DeleteWebsite()
    {
        InitializeComponent();
    }

    private void OnConfirm(object? sender, RoutedEventArgs e)
    {
        Close(true);
    }

    private void OnCancel(object? sender, RoutedEventArgs e)
    {
        Close(false);
    }
}