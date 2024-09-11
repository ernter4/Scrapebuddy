using System;
using System.Linq;
using Avalonia.Controls;
using Avalonia.Interactivity;
using ScrapeBuddy.Data;
using Avalonia.Layout;
using System.Timers;
using Avalonia; 
using Avalonia.Threading;  
using Newtonsoft.Json.Linq;
using System.IO;
using Avalonia.Media;
using Avalonia.VisualTree;

namespace ScrapeBuddy.ViewModels
{
    public partial class NotificationViewModel : UserControl
    {
        public event EventHandler CancelRequested;
        private Timer _notificationRefreshTimer;  
        private bool _isControlVisible;  
        private SolidColorBrush notificationBackgroundColor;
        private SolidColorBrush notificationTextColor;

        public NotificationViewModel()
        {
            InitializeComponent();
            _isControlVisible = false; 
            SettingsViewModel.ThemeChanged += SetTheme;
            SetTheme(null, null);
        }

        /// <summary>
        /// Starts a timer to refresh notifications periodically.
        /// </summary>
        private void StartNotificationRefreshTimer()
        {
            _notificationRefreshTimer = new Timer(1000);  
            _notificationRefreshTimer.Elapsed += OnTimedRefresh;  
            _notificationRefreshTimer.AutoReset = true;  
            _notificationRefreshTimer.Start();  
        }

        /// <summary>
        /// Stops the notification refresh timer.
        /// </summary>
        private void StopNotificationRefreshTimer()
        {
            if (_notificationRefreshTimer != null)
            {
                _notificationRefreshTimer.Stop();
                _notificationRefreshTimer.Elapsed -= OnTimedRefresh;
                _notificationRefreshTimer.Dispose();
                _notificationRefreshTimer = null;
            }
        }

        /// <summary>
        /// Event triggered every few seconds to refresh the notifications.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">Event arguments containing the time of the event.</param>
        private void OnTimedRefresh(object sender, ElapsedEventArgs e)
        {
            if (_isControlVisible)
            {
                Dispatcher.UIThread.InvokeAsync(() =>
                {
                    RefreshNotifications();
                });
            }
        }

        /// <summary>
        /// Reloads the notifications, clearing the old ones and displaying the latest.
        /// </summary>
        public void RefreshNotifications()
        {
            NotificationList.Items.Clear(); 
            LoadNotifications(); 
        }

        /// <summary>
        /// Loads notifications from the database and displays them.
        /// </summary>
        public void LoadNotifications()
        {
            NotificationList.Items.Clear(); 
            var notifications = DatabaseHandler.GetNotifications();

            foreach (var notification in notifications)
            {
                // Create border and set properties
                var border = new Border
                {
                    BorderThickness = new Avalonia.Thickness(4),
                    CornerRadius = new Avalonia.CornerRadius(10),
                    Margin = new Avalonia.Thickness(5),
                    Padding = new Avalonia.Thickness(10),
                    Background = notificationBackgroundColor // Use theme-specific background color
                };

                // Apply a border color if Dismiss is false
                if (!notification.dismiss)
                {
                    border.BorderBrush = new SolidColorBrush(Color.Parse("#a7dee4"));
                    border.Margin = new Avalonia.Thickness(15);
                }
                else
                {
                    border.BorderBrush = Avalonia.Media.Brushes.Transparent;
                    border.Margin = new Avalonia.Thickness(25);
                }

                // Create grid for layout
                var grid = new Grid();
                grid.ColumnDefinitions.Add(new ColumnDefinition(1, GridUnitType.Star));
                grid.ColumnDefinitions.Add(new ColumnDefinition(GridLength.Auto));

                // Create stack panel for title and notification text
                var stackPanel = new StackPanel { Orientation = Orientation.Vertical };

                var titleBlock = new TextBlock
                {
                    Text = notification.title,
                    FontWeight = Avalonia.Media.FontWeight.Bold,
                    FontSize = 16,
                    Foreground = notificationTextColor // Use theme-specific text color
                };

                var notificationBlock = new TextBlock
                {
                    Text = notification.notification,
                    TextWrapping = Avalonia.Media.TextWrapping.Wrap,
                    MaxWidth = 500,
                    Foreground = notificationTextColor // Use theme-specific text color
                };

                stackPanel.Children.Add(titleBlock);
                stackPanel.Children.Add(notificationBlock);

                // Create delete button
                var deleteButton = new Button
                {
                    Content = "✖",
                    HorizontalAlignment = Avalonia.Layout.HorizontalAlignment.Right,
                    VerticalAlignment = Avalonia.Layout.VerticalAlignment.Top,
                    Background = Avalonia.Media.Brushes.Transparent,
                    BorderBrush = Avalonia.Media.Brushes.Transparent,
                    Foreground = Avalonia.Media.Brushes.Red,
                    Tag = notification.id 
                };
                deleteButton.Click += OnDeleteNotification;

                // Add elements to grid
                grid.Children.Add(stackPanel);
                grid.Children.Add(deleteButton);
                Grid.SetColumn(deleteButton, 1);

                // Set grid as the child of the border
                border.Child = grid;

                // Add border to the notification list
                NotificationList.Items.Add(border);
            }
        }


        /// <summary>
        /// Dismisses all notifications from the database.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="args">Event arguments containing any additional event data.</param>
        private void DismissAll(object sender, RoutedEventArgs args)
        {
            DatabaseHandler.DismissAllNotitifications(); 
            LoadNotifications();
        }

        /// <summary>
        /// Deletes a specific notification by ID.
        /// </summary>
        /// <param name="id">The ID of the notification to delete.</param>
        private void DeleteNotification(int id)
        {
            DatabaseHandler.DeleteNotitification(id);
            var itemToRemove = NotificationList.Items.Cast<Border>()
                .FirstOrDefault(item => (int)((Button)((Grid)item.Child).Children[1]).Tag == id);
            if (itemToRemove != null)
            {
                NotificationList.Items.Remove(itemToRemove);
            }
        }

        /// <summary>
        /// Handles the click event of the delete button and deletes the corresponding notification.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">Event arguments containing any additional event data.</param>
        private void OnDeleteNotification(object sender, RoutedEventArgs e)
        {
            if (sender is Button button && button.Tag is int id)
            {
                DeleteNotification(id);
            }
        }
        
        // <summary>
        /// Handles the click event for the "Delete All" action. 
        /// This method removes all notifications from the database and clears them from the user interface.
        /// </summary>
        /// <param name="sender">The source of the event, typically the control that triggered the event.</param>
        /// <param name="e">Provides data for the event, such as event-specific information.</param>
        private async void OnDeleteAll(object sender, RoutedEventArgs e)
        {
            var parentWindow = this.GetVisualRoot() as Window;
            var confirmationDialog = new YesNoDialog();
            confirmationDialog.UpdateConfirmationMessage("Do you really want to remove all Notifications?");
            bool result = await confirmationDialog.ShowDialog<bool>(parentWindow);
            if (result)
            {
                DatabaseHandler.DeleteAllNotitifications();
                NotificationList.Items.Clear();
            }
        }
        
        /// <summary>
        /// Cancels the view and raises the CancelRequested event.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="args">Event arguments containing any additional event data.</param>
        private void OnCancelView(object sender, RoutedEventArgs args)
        {
            CancelRequested?.Invoke(this, EventArgs.Empty);
            _isControlVisible = false;  
            StopNotificationRefreshTimer();
        }

        /// <summary>
        /// Called when the control is added to a visual tree (made visible).
        /// </summary>
        /// <param name="e">Event arguments containing information about the visual tree attachment.</param>
        protected override void OnAttachedToVisualTree(VisualTreeAttachmentEventArgs e)
        {
            base.OnAttachedToVisualTree(e);
            _isControlVisible = true; 
            StartNotificationRefreshTimer();  
            LoadNotifications();  
        }

        /// <summary>
        /// Called when the control is removed from the visual tree (hidden).
        /// </summary>
        /// <param name="e">Event arguments containing information about the visual tree detachment.</param>
        protected override void OnDetachedFromVisualTree(VisualTreeAttachmentEventArgs e)
        {
            base.OnDetachedFromVisualTree(e);
            _isControlVisible = false; 
            StopNotificationRefreshTimer();  
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
                Console.WriteLine("Now its Darkmode");
                MainGrid.Background = (SolidColorBrush)Application.Current.Resources["BackgroundDark"];
                HomeBtn.Classes.Clear();
                HomeBtn.Classes.Add("btn_white");
                Dismiss.Classes.Clear();
                Dismiss.Classes.Add("btn_white");
                DeleteAll.Classes.Clear();
                DeleteAll.Classes.Add("btn_white");
                Title.Foreground = new SolidColorBrush(Color.FromRgb(239, 239, 239));
                notificationBackgroundColor = new SolidColorBrush(Color.FromRgb(84, 98, 108)); // Dark gray
                notificationTextColor = new SolidColorBrush(Color.FromRgb(239, 239, 239)); // Light gray
            }
            else
            {
                Console.WriteLine("Now its Lightmode");
                MainGrid.Background = (SolidColorBrush)Application.Current.Resources["BackgroundLight"];
                HomeBtn.Classes.Clear();
                HomeBtn.Classes.Add("btn_dark");
                DeleteAll.Classes.Clear();
                DeleteAll.Classes.Add("btn_dark");
                Dismiss.Classes.Clear();
                Dismiss.Classes.Add("btn_dark");
                Title.Foreground = new SolidColorBrush(Color.FromRgb(40, 40, 40));
                notificationBackgroundColor = new SolidColorBrush(Color.FromRgb(216, 213, 213)); 
                notificationTextColor = new SolidColorBrush(Color.FromRgb(40, 40, 40)); 
            }
        }
    }
}








