using System;
using System.Linq;
using Avalonia.Controls;
using Avalonia.Interactivity;
using ScrapeBuddy.Data;
using Avalonia.Layout;
using System.Timers;
using Avalonia; 
using Avalonia.Threading;  

namespace ScrapeBuddy.ViewModels
{
    public partial class NotificationViewModel : UserControl
    {
        public event EventHandler CancelRequested;
        private Timer _notificationRefreshTimer;  
        private bool _isControlVisible;  

        public NotificationViewModel()
        {
            InitializeComponent();
            _isControlVisible = false; 
        }

        /// <summary>
        /// Starts a timer to refresh notifications periodically.
        /// </summary>
        private void StartNotificationRefreshTimer()
        {
            _notificationRefreshTimer = new Timer(5000);  
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
            Console.WriteLine("RefreshNotifications");
            NotificationList.Items.Clear(); 
            LoadNotifications(); 
        }

        /// <summary>
        /// Loads notifications from the database and displays them.
        /// </summary>
        public void LoadNotifications()
        {
            var notifications = DatabaseHandler.GetNotifications();

            foreach (var notification in notifications)
            {
                var border = new Border
                {
                    BorderThickness = new Avalonia.Thickness(2),
                    CornerRadius = new Avalonia.CornerRadius(10),
                    Margin = new Avalonia.Thickness(5),
                    Padding = new Avalonia.Thickness(10),
                    Background = Avalonia.Media.Brushes.LightGray
                };

                var grid = new Grid();
                grid.ColumnDefinitions.Add(new ColumnDefinition(1, GridUnitType.Star));
                grid.ColumnDefinitions.Add(new ColumnDefinition(GridLength.Auto));

                var stackPanel = new StackPanel { Orientation = Orientation.Vertical };

                var titleBlock = new TextBlock
                {
                    Text = notification.title,
                    FontWeight = Avalonia.Media.FontWeight.Bold,
                    FontSize = 16
                };

                var notificationBlock = new TextBlock
                {
                    Text = notification.notification,
                    TextWrapping = Avalonia.Media.TextWrapping.Wrap,
                    MaxWidth = 500
                };

                stackPanel.Children.Add(titleBlock);
                stackPanel.Children.Add(notificationBlock);

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

                grid.Children.Add(stackPanel);
                grid.Children.Add(deleteButton);
                Grid.SetColumn(deleteButton, 1);

                border.Child = grid;

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
    }
}








