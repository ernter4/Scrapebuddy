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

namespace ScrapeBuddy.ViewModels
{
    /// <summary>
    /// Main application window managing the UI and user interactions.
    /// </summary>
    public partial class MainWindow : Window
    {
        private Timer _timer;
        private int? _selectedWebsiteId;
        
        private AddWebsite NewWebsiteView { get; set; }
        private NotificationViewModel NotificationsView { get; set; }
        private SettingsViewModel SettingsView { get; set; }
        private RegisteredWebsite RegisteredWebsiteView { get; set; }

        public static List<WebsiteInfo> StoredUserWebsites { get; set; }
        private bool _isScraping = false;

        /// <summary>
        /// Initializes the window and its components.
        /// </summary>
        public MainWindow()
        {
            InitializeComponent();
            SetupTimer();
            NewWebsiteView = new AddWebsite();
            NotificationsView = new NotificationViewModel();
            RegisteredWebsiteView = new RegisteredWebsite();
            SettingsView = new SettingsViewModel();
            StoredUserWebsites = GetUsersWebsites(new DatabaseHandler());
            CreateNewBtn.Click += (sender, args) => RenderView(sender, args, NewWebsiteView); 
            NotificationsBtn.Click += (sender, args) => RenderView(sender, args, NotificationsView);
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
            UpdateList();
        }

        /// <summary>
        /// Configures and starts the timer for periodic events.
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
            Console.WriteLine($"Current Id: {selectedId}");
            
            var confDialog = new DeleteWebsite(websiteInfo.WebsiteName);
            bool result = await confDialog.ShowDialog<bool>(this);

            if (result)
            {
                DatabaseHandler.DeleteScrapeItem(websiteInfo.Id);
                UpdateList();
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
