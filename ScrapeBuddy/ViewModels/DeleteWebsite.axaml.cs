using System;
using Avalonia;
using Avalonia.Controls;
using Avalonia.Interactivity;
using Avalonia.Markup.Xaml;
using Tmds.DBus.Protocol;

namespace ScrapeBuddy.ViewModels
{
    /// <summary>
    /// Represents a window for confirming the deletion of a website.
    /// </summary>
    public partial class DeleteWebsite : Window
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="DeleteWebsite"/> class.
        /// </summary>
        /// <param name="websitename">The name of the website to be deleted.</param>
        public DeleteWebsite(string websitename)
        {
            InitializeComponent();
            Message.Text = $"Do you really want to delete {websitename}?";
        }

        /// <summary>
        /// Handles the confirm action when the user clicks the confirm button.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">The event data.</param>
        private void OnConfirm(object? sender, RoutedEventArgs e)
        {
            Close(true);
        }

        /// <summary>
        /// Handles the cancel action when the user clicks the cancel button.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">The event data.</param>
        private void OnCancel(object? sender, RoutedEventArgs e)
        {
            Close(false);
        }
    }
}