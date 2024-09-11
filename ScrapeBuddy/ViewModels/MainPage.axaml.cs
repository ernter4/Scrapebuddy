using System;
using System.Timers;
using Avalonia.Controls;
using ScrapeBuddy.Data;
using ScrapeBuddy.Logic;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Linq;
using Avalonia;
using Avalonia.Interactivity;
using Newtonsoft.Json.Linq;
using System.IO;
using Avalonia.Media;


namespace ScrapeBuddy.ViewModels
{
    /// <summary>
    /// Main Page Window with short Tutorial
    /// </summary>
    public partial class MainPageViewModel : UserControl
    {
        public MainPageViewModel()
        {
            InitializeComponent();
            SettingsViewModel.ThemeChanged += SetTheme;
            SetTheme(null, null);
        }

        /// <summary>
        /// Sets the application theme based on the "DarkMode" setting.
        /// </summary>
        /// <param name="sender">The event sender (can be null).</param>
        /// <param name="eventArgs">Event arguments (can be null).</param>
        private void SetTheme(object? sender, EventArgs eventArgs)
        {
            JObject settingsJson = JObject.Parse(File.ReadAllText(@"Data/Settings.json"));
            bool isDarkMode = settingsJson["DarkMode"].ToObject<bool>();
            if (isDarkMode)
            {
                MainGrid.Background = (SolidColorBrush)Application.Current.Resources["BackgroundDark"];
                Title.Foreground = new SolidColorBrush(Color.FromRgb(239, 239, 239));
                TextBlock1.Foreground = new SolidColorBrush(Color.FromRgb(239, 239, 239));
                TextBlock2.Foreground = new SolidColorBrush(Color.FromRgb(239, 239, 239));
                TextBlock3.Foreground = new SolidColorBrush(Color.FromRgb(239, 239, 239));
                TextBlock4.Foreground = new SolidColorBrush(Color.FromRgb(239, 239, 239));
                TextBlock5.Foreground = new SolidColorBrush(Color.FromRgb(239, 239, 239));
                TextBlock6.Foreground = new SolidColorBrush(Color.FromRgb(239, 239, 239));
                TextBlock7.Foreground = new SolidColorBrush(Color.FromRgb(239, 239, 239));
                TextBlock8.Foreground = new SolidColorBrush(Color.FromRgb(239, 239, 239));
                TextBlock9.Foreground = new SolidColorBrush(Color.FromRgb(239, 239, 239));
                TextBlock10.Foreground = new SolidColorBrush(Color.FromRgb(239, 239, 239));
            }
            else
            {
                MainGrid.Background = (SolidColorBrush)Application.Current.Resources["BackgroundLight"];
                Title.Foreground = new SolidColorBrush(Color.FromRgb(40, 40, 40));
                TextBlock1.Foreground = new SolidColorBrush(Color.FromRgb(40, 40, 40));
                TextBlock2.Foreground = new SolidColorBrush(Color.FromRgb(40, 40, 40));
                TextBlock3.Foreground = new SolidColorBrush(Color.FromRgb(40, 40, 40));
                TextBlock4.Foreground = new SolidColorBrush(Color.FromRgb(40, 40, 40));
                TextBlock5.Foreground = new SolidColorBrush(Color.FromRgb(40, 40, 40));
                TextBlock6.Foreground = new SolidColorBrush(Color.FromRgb(40, 40, 40));
                TextBlock7.Foreground = new SolidColorBrush(Color.FromRgb(40, 40, 40));
                TextBlock8.Foreground = new SolidColorBrush(Color.FromRgb(40, 40, 40));
                TextBlock9.Foreground = new SolidColorBrush(Color.FromRgb(40, 40, 40));
                TextBlock10.Foreground = new SolidColorBrush(Color.FromRgb(40, 40, 40));
               
                
            }
        }
    }
   
}

