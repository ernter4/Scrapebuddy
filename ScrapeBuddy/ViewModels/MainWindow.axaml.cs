using System;
using System.Timers;
using Avalonia.Controls;
using ScrapeBuddy.Data;
using ScrapeBuddy.Logic;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using Avalonia.Interactivity;

namespace ScrapeBuddy.ViewModels
{
    /// <summary>
    /// The main window of the application, responsible for managing the user interface
    /// and handling user interactions.
    /// </summary>
    public partial class MainWindow : Window
    {
        private Timer _timer;
        
        /// <summary>
        /// Gets or sets the view for adding a new website.
        /// </summary>
        private NewWebsite NewWebsiteView { get; set; }

        /// <summary>
        /// Gets or sets the view for displaying registered websites.
        /// </summary>
        private RegisteredWebsite RegisteredWebsiteView { get; set; }

        /// <summary>
        /// Gets or sets the list of websites stored by the user.
        /// </summary>
        public List<string?> StoredUserWebsites { get; set; }

        /// <summary>
        /// Initializes a new instance of the <see cref="MainWindow"/> class.
        /// </summary>
        public MainWindow()
        {
            InitializeComponent();
            SetupTimer();
            NewWebsiteView = new NewWebsite();
            RegisteredWebsiteView = new RegisteredWebsite();
            StoredUserWebsites = GetUsersWebsites(new DatabaseHandler());
            CreateNewBtn.Click += (sender, args) => RenderView(sender, args, NewWebsiteView);
            RegisteredWebsiteListBox.Tapped += (sender, args) => RenderView(sender, args, RegisteredWebsiteView);
            NewWebsiteView.CancelRequested += (sender, args) => OnViewCancelRequested(sender, args, NewWebsiteView);
            RegisteredWebsiteView.CancelRequested += (sender, args) => OnViewCancelRequested(sender, args, RegisteredWebsiteView);
        }

        /// <summary>
        /// Sets up a timer that triggers an event at regular intervals.
        /// </summary>
        private void SetupTimer()
        {
            // Create and configure the timer
            _timer = new Timer
            {
                Interval = 300000 // 5 minutes in milliseconds
            };
            _timer.Elapsed += OnTimedEvent;
            _timer.Start();
        }

        /// <summary>
        /// Handles the elapsed event of the timer. This is where the scraping process starts.
        /// </summary>
        /// <param name="source">The source of the event.</param>
        /// <param name="e">The event data.</param>
        private void OnTimedEvent(object source, ElapsedEventArgs e)
        {
            // Call your method here
            Console.WriteLine("Start scraping Now");
            Scrape.ScrapeItems();
        }

        /// <summary>
        /// Disposes the timer when the window is closed.
        /// </summary>
        /// <param name="e">The event data.</param>
        protected override void OnClosed(EventArgs e)
        {
            _timer?.Dispose();
            base.OnClosed(e);
        }

        /// <summary>
        /// Handles the event when a view requests to be canceled (removed from display).
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="args">The event data.</param>
        /// <param name="view">The view to be removed from the main content.</param>
        private void OnViewCancelRequested(object sender, EventArgs args, Control view)
        {
            MainContent.Children.Remove(view);
        }

        /// <summary>
        /// Retrieves the list of websites stored by the user from the database.
        /// </summary>
        /// <param name="db">The database handler used to query the database.</param>
        /// <returns>A list of website names stored by the user.</returns>
        private List<string?> GetUsersWebsites(DatabaseHandler db)
        {
            return db.GetAllWebsites().AsEnumerable().Select(row => row["website_name"].ToString()).ToList();
        }

        /// <summary>
        /// Renders a specified view in the main content area of the window.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="args">The event data.</param>
        /// <param name="view">The view to be rendered.</param>
        private void RenderView(object sender, RoutedEventArgs args, Control view)
        {
            if (!IsViewLoadedInMainContent(view))
            {
                MainContent.Children.Clear();
                MainContent.Children.Add(view);
            }
        }

        /// <summary>
        /// Checks if a specified view is already loaded in the main content area.
        /// </summary>
        /// <param name="view">The view to check.</param>
        /// <returns><c>true</c> if the view is loaded; otherwise, <c>false</c>.</returns>
        private bool IsViewLoadedInMainContent(Control view)
        {
            return MainContent.Children.Contains(view);
        }
    }
}
