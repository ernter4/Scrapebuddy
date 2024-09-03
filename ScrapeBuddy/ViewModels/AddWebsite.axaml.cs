using System;
using System.Threading.Tasks;
using Avalonia.Controls;
using Avalonia.Interactivity;
using Avalonia.Media;
using Avalonia.Styling;
using OpenQA.Selenium;
using ScrapeBuddy.Data;
using ScrapeBuddy.Logic;

namespace ScrapeBuddy.ViewModels;

/// <summary>
/// Represents the user control for managing a new website entry, including functionality for saving user inputs and switching between XPath and Id modes.
/// </summary>
public partial class AddWebsite : UserControl
{
    public AddWebsite()
    {
        InitializeComponent();
        DataContext = this;
        NeedsCredentials = false;
        SaveBtn.IsEnabled = IsInputSavable();
        WebsiteNameTextBox.TextChanged += OnTextboxInputChanged;
        WebsiteLinkTextBox.TextChanged += OnTextboxInputChanged;
        PathToElementTextBox.TextChanged += OnTextboxInputChanged;
        UsernameTextBox.TextChanged += OnTextboxInputChanged;
        PasswordTextBox.TextChanged += OnTextboxInputChanged;
        SetButtonState(XPathButton, false);
        SetButtonState(IdButton, true);
    }

    private string _websiteName;
    private string _websiteLink;
    private string _username;
    private string _password; 
    public string? WebsiteName
    {
        get { return _websiteName; }
        set { _websiteName = value; }
    }
    public string? WebsiteLink
    {
        get { return _websiteLink;}
        set { _websiteLink = value; }
    }
    public string? XPath { get; set; }
    public string? Id { get; set; }
    public string? Username
    {
        get { return _username;}
        set { _username = value; }
    } 
    public string? Password
    {
        get { return _password;}
        set { _password = value; }
    }
    public bool IsId { get; set; }
    public bool Login = false;
    public bool Scraping = false;
    public string Content;
    public bool NeedsCredentials { get; set; }
    private Style PseudoDeactivated { get; set; }

    public event EventHandler CancelRequested;

    /// <summary>
    /// Clears all input fields.
    /// </summary>
    private void ClearAllFields()
    {
        WebsiteName = null;
        WebsiteNameTextBox.Text = null;
        WebsiteLink = null;
        WebsiteLinkTextBox.Text = null;
        XPath = null;
        Id = null;
        PathToElementTextBox.Text = null;
        Username = null;
        UsernameTextBox.Text = null;
        Password = null;
        PasswordTextBox.Text = null;
        Scraping = false;
        Login = false;
    }

    /// <summary>
    /// Writes user inputs to the database and clears fields.
    /// </summary>
    /// <param name="sender">The source of the event.</param>
    /// <param name="args">The event data.</param>
    private void WriteUserInputsToDatabase(object sender, RoutedEventArgs args)
    {
        Login = _password != null && _username != null;
        DatabaseHandler website = new DatabaseHandler(WebsiteName, WebsiteLink, XPath, Id, Content, Login, Username, Password);
        website.CreateWebsiteContent();
        CancelRequested.Invoke(this, EventArgs.Empty);
        ClearAllFields();
    }
    
    /// <summary>
    /// Initiates the scraping process and handles the results.
    /// </summary>
    /// <param name="sender">The source of the event.</param>
    /// <param name="args">The event data.</param>
    private async void Scrape(object sender, RoutedEventArgs args)
    {
        var loadingDialog = new ScrapeDialog();
        var selenium = new Selenium();
        Login = _password != null && _username != null;

        try
        {
            loadingDialog.Show();
            IWebElement element = await Task.Run(() => selenium.Scrape(WebsiteLink, Login, XPath, Id, Username, Password));
            loadingDialog.UpdateMessage($"Scraping for {WebsiteName} succeeded: {element.Text}");
            Scraping = true;
            Content = element.Text;
            SaveBtn.IsEnabled = IsInputSavable();
        }
        catch (Exception ex)
        {
            DatabaseHandler notification = new DatabaseHandler($"Scraping for {WebsiteName} failed: ", ex.Message);
            notification.CreateNotification();

            loadingDialog.UpdateMessage($"Scraping for {WebsiteName} failed: {ex.Message}");
        }
        finally
        {
            selenium.Quit();
            Console.WriteLine("Scraping complete");
            loadingDialog.ShowOkButton();
        }
    }

    /// <summary>
    /// Cancels the view and raises the CancelRequested event.
    /// </summary>
    /// <param name="sender">The source of the event.</param>
    /// <param name="args">The event data.</param>
    private void OnCancelView(object sender, RoutedEventArgs args)
    {
        CancelRequested.Invoke(this, EventArgs.Empty);
        ClearAllFields();
    }

    /// <summary>
    /// Clears all input fields when invoked.
    /// </summary>
    /// <param name="sender">The source of the event.</param>
    /// <param name="args">The event data.</param>
    private void ClearFields(object sender, RoutedEventArgs args)
    {
        ClearAllFields();
    }

    /// <summary>
    /// Toggles the visibility of the credentials fields.
    /// </summary>
    /// <param name="sender">The source of the event.</param>
    /// <param name="args">The event data.</param>
    private void ShowCredentialsView(object sender, RoutedEventArgs args)
    {
        NeedsCredentials = !NeedsCredentials;
        if (NeedsCredentials)
        {
            CredentialsBtn.Content = "Add Credentials"; 
        }
        else
        {
            CredentialsBtn.Content = "Remove Credentials";
        }
        Password = null;
        Username = null;
        Console.WriteLine($"{NeedsCredentials}");
        UsernameTextBox.IsVisible = NeedsCredentials;
        PasswordTextBox.IsVisible = NeedsCredentials;
        UsernameTextBox.Text = null;
        PasswordTextBox.Text = null;
    }

    /// <summary>
    /// Updates the corresponding variables when text box input changes and checks if the input is savable.
    /// </summary>
    /// <param name="sender">The source of the event.</param>
    /// <param name="args">The event data.</param>
    private void OnTextboxInputChanged(object sender, RoutedEventArgs args)
    {
        if (sender is TextBox textBox)
        {
            Console.WriteLine(textBox.Name);
            switch (textBox.Name)
            {   
                case "WebsiteNameTextBox":
                    WebsiteName = textBox.Text;
                    Console.WriteLine($"{WebsiteName}");
                    SaveBtn.IsEnabled = IsInputSavable();
                    break;

                case "WebsiteLinkTextBox":
                    WebsiteLink = textBox.Text;
                    Console.WriteLine($"{WebsiteLink}");
                    SaveBtn.IsEnabled = IsInputSavable();
                    break;

                case "PathToElementTextBox":
                    XPath = IsId ? null : textBox.Text;
                    Id = IsId ? textBox.Text : null;
                    SaveBtn.IsEnabled = IsInputSavable();
                    break;

                case "PasswordTextBox":
                       Password = textBox.Text;
                       break;
                   
                case "UsernameTextBox": 
                       Username = textBox.Text;
                       break;

                default:
                    Console.WriteLine("Unknown Textbox");
                    break;
            }
        }
    }

    /// <summary>
    /// Checks if user input is valid and savable.
    /// </summary>
    /// <returns>True if savable; otherwise, false.</returns>
    public bool IsInputSavable()
    {
        return (!string.IsNullOrWhiteSpace(_websiteName) && Scraping && !string.IsNullOrWhiteSpace(_websiteLink) && (!string.IsNullOrWhiteSpace(Id) || !string.IsNullOrWhiteSpace(XPath)));
    }

    /// <summary>
    /// Handles the click event for the XPath button and updates the state.
    /// </summary>
    /// <param name="sender">The source of the event.</param>
    /// <param name="e">The event data.</param>
    private void OnXPathClicked(object sender, RoutedEventArgs e)
    {
        SetButtonState(XPathButton, true);
        SetButtonState(IdButton, false);
        PathToElementTextBox.Watermark = "XPath";
        XPath = PathToElementTextBox.Text;
        Id = null;
        IsId = false;
    }

    /// <summary>
    /// Handles the click event for the ID button and updates the state.
    /// </summary>
    /// <param name="sender">The source of the event.</param>
    /// <param name="e">The event data.</param>
    private void OnIDClicked(object sender, RoutedEventArgs e)
    {
        SetButtonState(XPathButton, false);
        SetButtonState(IdButton, true);
        PathToElementTextBox.Watermark = "Id";
        XPath = null;
        Id = PathToElementTextBox.Text;
        IsId = true;
    }

    /// <summary>
    /// Updates the visual state of a button based on its active status.
    /// </summary>
    /// <param name="button">The button to update.</param>
    /// <param name="isActive">Whether the button is active or not.</param>
    private void SetButtonState(Button button, bool isActive)
    {
        if (isActive)
        {
            button.Background = Brushes.DodgerBlue;
            button.Foreground = Brushes.White;
            button.BorderBrush = Brushes.DodgerBlue;
        }
        else
        {
            button.Background = new SolidColorBrush(Color.Parse("#f5f7fa"));
            button.Foreground = Brushes.Black;
            button.BorderBrush = Brushes.Black;
        }
    }
}
