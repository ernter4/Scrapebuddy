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
using Avalonia.Input;
using Avalonia.Media;
using Avalonia.Threading;


namespace ScrapeBuddy.ViewModels
{
    /// <summary>
    /// Main application window managing the UI and user interactions.
    /// </summary>
    public partial class MainWindow : Window
    {
        private Timer _timer;
        private Timer _notificationsTimer;
        private int? _selectedWebsiteId;
        
        private AddWebsite NewWebsiteView { get; set; }
        private NotificationViewModel NotificationsView { get; set; }
        private SettingsViewModel SettingsView { get; set; }
        private RegisteredWebsite RegisteredWebsiteView { get; set; }
        
        private MainPageViewModel MainPageView { get; set; }

        public static List<WebsiteInfo> StoredUserWebsites { get; set; }
        private bool _isScraping = false;

        /// <summary>
        /// Initializes the window and its components.
        /// </summary>
        public MainWindow()
        {
            InitializeComponent();
            SetupTimer();
            SetupNotificationsTimer();
            SettingsViewModel.ThemeChanged += SetTheme;
            SetTheme(null, null);
            NotificationsTextBlock.Text = DatabaseHandler.GetNotificationsUndismissed().ToString();
            
            NewWebsiteView = new AddWebsite();
            NotificationsView = new NotificationViewModel();
            RegisteredWebsiteView = new RegisteredWebsite();
            SettingsView = new SettingsViewModel();
            MainPageView = new MainPageViewModel();
            StoredUserWebsites = GetUsersWebsites(new DatabaseHandler());
            CreateNewBtn.Click += (sender, args) => RenderView(sender, args, NewWebsiteView); 
            NotificationsBtn.Click += (sender, args) =>
            {
                RenderView(sender, args, NotificationsView);
                NotificationsView.LoadNotifications();
            };
            SettingsBtn.Click += (sender, args) =>
            {
                SettingsView.LoadSettings();
                RenderView(sender, args, SettingsView);
            };
            SearchTextBox.TextChanged += UpdateWebsiteListBox;
            RegisteredWebsiteListBox.SelectionChanged += RegisteredWebsiteListBox_SelectionChanged;
            RegisteredWebsiteListBox.Tapped += (sender, args) => RenderView(sender, args, RegisteredWebsiteView);
            NewWebsiteView.CancelRequested += (sender, args) => OnViewCancelRequested(sender, args, NewWebsiteView);
            RegisteredWebsiteView.CancelRequested += (sender, args) => OnViewCancelRequested(sender, args, RegisteredWebsiteView);
            NotificationsView.CancelRequested += (sender, args) => OnViewCancelRequested(sender, args, NotificationsView);
            SettingsView.CancelRequested += (sender, args) => OnViewCancelRequested(sender, args, SettingsView);
            RenderView(sender: MainPageView, args: null, MainPageView);
        }

        /// <summary>
        /// Handles website selection changes in the list box.
        /// </summary>
        private void RegisteredWebsiteListBox_SelectionChanged(object? sender, SelectionChangedEventArgs e)
        {
            var selectedWebsite = RegisteredWebsiteListBox.SelectedItem as WebsiteInfo;
            if (selectedWebsite != null)
            {
                _selectedWebsiteId = selectedWebsite.Id;
                Console.WriteLine($"Selected website ID: {_selectedWebsiteId}");
                if (_selectedWebsiteId.HasValue)
                {
                    RegisteredWebsiteView.LoadWebsiteData(_selectedWebsiteId.Value);
                }
                
            }
        }

        /// <summary>
        /// Displays the specified view in the main content area.
        /// </summary>
        private void RenderView(object sender, RoutedEventArgs args, Control view)
        {
            
            if (view is RegisteredWebsite && _selectedWebsiteId.HasValue)
            {
                RegisteredWebsiteView.LoadWebsiteData(_selectedWebsiteId.Value);
            }
    
            if (!IsViewLoadedInMainContent(view))
            {
                MainContent.Children.Clear();
                MainContent.Children.Add(view);
            }
        }
        
        private void OnDeleteSettingsBtnClick(object sender, RoutedEventArgs e)
        {
            e.Handled = true; // Prevents the event from propagating to other controls.
        }

        /// <summary>
        /// Retrieves the user's websites from the database.
        /// </summary>
        public static List<WebsiteInfo> GetUsersWebsites(DatabaseHandler db)
        {
            return db.GetAllWebsites().AsEnumerable().Select(row => new WebsiteInfo
            {
                Id = Convert.ToInt32(row["id"]),
                WebsiteName = row["website_name"].ToString()
            }).ToList();
        }

        /// <summary>
        /// Removes a view from the main content area when canceled.
        /// </summary>
        private void OnViewCancelRequested(object sender, EventArgs args, Control view)
        {
            MainContent.Children.Remove(view);
            RenderView(sender, args: null, MainPageView);
            UpdateList();
        }

        /// <summary>
        /// Configures and starts the timer for scraping events.
        /// </summary>
        private void SetupTimer()
        {
            if (_timer != null) 
            {
                _timer.Stop();
                _timer.Elapsed -= OnTimedEvent;
            }

            _timer = new Timer
            {
                Interval = 300000 // 5 minutes
            };
    
            _timer.Elapsed += OnTimedEvent;
            _timer.Start();
        }
        
        /// <summary>
        /// Configures and starts the timer for notification events.
        /// </summary>
        private void SetupNotificationsTimer()
        {
            if (_notificationsTimer != null) 
            {
                _notificationsTimer.Stop();
                _notificationsTimer.Elapsed -= OnNotificationsTimerEvent;
            }

            _notificationsTimer = new Timer
            {
                Interval = 1000 // 1 second
            };

            _notificationsTimer.Elapsed += OnNotificationsTimerEvent;
            _notificationsTimer.Start();
        }
        
        /// <summary>
        /// Updates the Notification symbol.
        /// </summary>
        private void OnNotificationsTimerEvent(object sender, ElapsedEventArgs e)
        {
            Dispatcher.UIThread.InvokeAsync(() => 
            {
                int undismissedCount = DatabaseHandler.GetNotificationsUndismissed();
                if (undismissedCount > 0)
                {
                    NotificationsTextBlock.Text = undismissedCount.ToString();
                    NotificationBadge.IsVisible = true;
                }
                else
                {
                    NotificationBadge.IsVisible = false;
                }
            });
        }


        /// <summary>
        /// Starts the scraping process when the timer elapses.
        /// </summary>
        private void OnTimedEvent(object source, ElapsedEventArgs e)
        {
            if (_isScraping) 
            {
                Console.WriteLine("Scraping is already in progress, skipping...");
                return; // Prevent overlapping scrapes
            }

            _isScraping = true;
    
            try
            {
                Console.WriteLine("Start scraping now");
                Scrape.ScrapeItems(); // Your scraping logic
            }
            finally
            {
                _isScraping = false; // Ensure this gets reset even if scraping fails
            }
        }


        /// <summary>
        /// Disposes of the timer when the window closes.
        /// </summary>
        protected override void OnClosed(EventArgs e)
        {
            _timer?.Dispose();
            base.OnClosed(e);
        }

        /// <summary>
        /// Checks if a view is already displayed in the main content area.
        /// </summary>
        private bool IsViewLoadedInMainContent(Control view)
        {
            return MainContent.Children.Contains(view);
        }

        
        
        /// <summary>
        /// Updates the contents of the RegisteredWebsiteListBox based on the text entered in the SearchTextBox.
        /// </summary>
        /// <param name="sender">The source of the event, typically the SearchTextBox.</param>
        /// <param name="args">The event data associated with the input change, if any.</param>
        public void UpdateWebsiteListBox(object? sender, EventArgs? args)
        {
            RegisteredWebsiteListBox.ItemsSource = string.IsNullOrEmpty(SearchTextBox.Text)
                ? StoredUserWebsites
                : StoredUserWebsites.Where(site => site.WebsiteName.Contains(SearchTextBox.Text, StringComparison.OrdinalIgnoreCase))
                    .ToList();
        }

        /// <summary>
        /// Refreshes the list of stored websites from the database and updates the RegisteredWebsiteListBox.
        /// </summary>
        public void UpdateList()
        {
            StoredUserWebsites = GetUsersWebsites(new DatabaseHandler());
            UpdateWebsiteListBox(null, null);
        }
        
        /// <summary>
        /// opens confirmation dialog for deleting registered Website
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private async void RenderDeleteWebsiteView(object? sender, RoutedEventArgs e)
        {
            var item = sender as MenuItem;
            var websiteInfo = item.Tag as WebsiteInfo;
            int selectedId = websiteInfo.Id;
            
            var confirmationDialog = new YesNoDialog();
            confirmationDialog.UpdateConfirmationMessage($"Do you really want to delete {websiteInfo.WebsiteName}?");
            bool result = await confirmationDialog.ShowDialog<bool>(this);

            if (result)
            {
                DatabaseHandler.DeleteScrapeItem(websiteInfo.Id);
                UpdateList();
            }
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
                RootControl.Foreground = Brushes.White;
                Sidebar.Background = (SolidColorBrush)Application.Current.Resources["BackgroundSidebarDark"];
                CreateNewBtn.Classes.Remove("btn_dark");
                CreateNewBtn.Classes.Add("btn_white");
                SettingsBtn.Classes.Remove("btn_dark");
                SettingsBtn.Classes.Add("btn_white");
                NotificationsBtn.Classes.Remove("btn_dark");
                NotificationsBtn.Classes.Add("btn_white");
            }
            else
            {
                RootControl.Foreground = Brushes.Black;
                Sidebar.Background = (SolidColorBrush)Application.Current.Resources["BackgroundSidebarLight"];
                CreateNewBtn.Classes.Remove("btn_white");
                CreateNewBtn.Classes.Add("btn_dark");
                SettingsBtn.Classes.Remove("btn_white");
                SettingsBtn.Classes.Add("btn_dark");
                NotificationsBtn.Classes.Remove("btn_white");
                NotificationsBtn.Classes.Add("btn_dark");
            }
        }
    }

    /// <summary>   
    /// Represents a website's basic information.
    /// </summary>
    public class WebsiteInfo
    {
        public int Id { get; set; }
        public string? WebsiteName { get; set; }
    }
}
