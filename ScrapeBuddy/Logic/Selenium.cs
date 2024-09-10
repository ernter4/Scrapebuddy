using System;
using System.Collections.ObjectModel;
using OpenQA.Selenium.Chrome;
using OpenQA.Selenium;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Avalonia.Controls.Notifications;
using ScrapeBuddy.Exceptions;
using ScrapeBuddy.Data;
using ScrapeBuddy.ViewModels;
using Splat;
using MissingFieldException = System.MissingFieldException;

namespace ScrapeBuddy.Logic
{
    /// <summary>
    /// Provides methods for interacting with web pages using Selenium WebDriver.
    /// </summary>
    public class Selenium
    {
        /// <summary>
        /// The WebDriver instance used for interacting with web pages.
        /// </summary>
        private IWebDriver _driver;

        private double _implicit_Wait = 2.5;
        /// <summary>
        /// Initializes a new instance of the <see cref="Selenium"/> class and navigates to the specified URL.
        /// </summary>
        /// <param name="url">The URL to navigate to upon initialization.</param>
        public Selenium(string url = "")
        {
            var options = new ChromeOptions();
            options.AddArgument("--disable-search-engine-choice-screen");
            options.AddArgument("--headless"); // Runs Chrome in headless mode, without a GUI.
            options.AddArgument("--no-sandbox");
            options.AddArgument("--disable-dev-shm-usage");
            
            var service = ChromeDriverService.CreateDefaultService();
            service.HideCommandPromptWindow = true; 
            
            TimeSpan commandTimeout = TimeSpan.FromSeconds(20);

            _driver = new ChromeDriver(service, options, commandTimeout);
            _driver.Manage().Timeouts().ImplicitWait = TimeSpan.FromSeconds(_implicit_Wait); 
            if (url != "")
            {
                _driver.Navigate().GoToUrl(url);
            }
        }

        /// <summary>
        /// Retrieves a web element by its XPath.
        /// </summary>
        /// <param name="xpath">The XPath of the web element to find.</param>
        /// <returns>The web element found by the specified XPath.</returns>
        public IWebElement GetElementByXPath(string xpath)
        {
            try
            {
                IWebElement element = _driver.FindElement(By.XPath(xpath));
                return element;
            }
            catch (Exception ex)
            {
                if (ex is NoSuchElementException)
                {
                    throw new MissingFieldException($"Missing Field by Xpath {xpath}.");
                }

                if (xpath is null)
                {
                    throw new SeleniumException("Xpath cannot be null");
                }

                throw new ScrapeFailedException($"Error while scraping for Xpath {xpath}occured");

            }
        }

        /// <summary>
        /// Retrieves a web element by its ID.
        /// </summary>
        /// <param name="id">The ID of the web element to find.</param>
        /// <returns>The web element found by the specified ID.</returns>
        public IWebElement GetElementById(string id)
        {
            try
            {
                IWebElement element = _driver.FindElement(By.Id(id));
                return element;
            }
            catch (Exception ex)
            {
                if (ex is NoSuchElementException)
                {
                    throw new MissingFieldException($"Missing Field by id {id}.");
                }

                if (id is null)
                {
                    throw new SeleniumException("ID cannot be null");
                }
            
                throw new ScrapeFailedException($"Error while scraping for id {id}occured");

            }

        }

        /// <summary>
        /// Performs a login action on the web page.
        /// </summary>
        /// <param name="username">The username to be used for login.</param>
        /// <param name="password">The password to be used for login.</param>
        public void Login(string username, string password)
        {
            IWebElement passwordfield;
            IWebElement usernamefield;
            List<IWebElement> usernamefields;
            List<IWebElement> passwordfields = Getpasswordfields();
            usernamefields = Getusernamefields();
            if (usernamefields.Any() == false)
            {
                throw new LoginException(new MissingUsernamefieldException("No Usernamefield found"));
            }
            usernamefield = usernamefields[0];// gets First Field where the username should be pasted in.
            usernamefield.SendKeys(username);

            if (passwordfields.Any() == false)
            {
                usernamefield.SendKeys(Keys.Return);
                passwordfields = Getpasswordfields();
            }
            
            if (passwordfields.Any() == false)
            {
                 throw new LoginException(new  MissingPasswordfieldException("No Passwordfield found"));
            }

            passwordfield = passwordfields[0];
            
            passwordfield.SendKeys(password);
            passwordfield.SendKeys(Keys.Return);
            
        }
        
        /// <summary>
        /// If necessary logs into a website, and returns the webelement at specified identifier
        /// </summary>
        /// <param name="url">The url of desired element</param> 
        /// <param name="login">Specifies if login is necessary</param>
        /// <param name="elementstoclick"> A list of xpaths of Elements wich should be clicked</param>
        /// <param name="xpath">The xpath of desired Webelement</param>
        /// <param name="id"> The id of desired Webelement</param>
        /// <param name="username">The username to be used for login</param>
        /// <param name="password">The password to be used for login</param>
        /// <returns>desired Webelement</returns>
        public IWebElement Scrape(string url, bool login, List<string>? elementstoclick, string? xpath, string? id, string? username, string? password)
        {
            try
            {
                Console.WriteLine("Url: " + url + " ElementstoClick: " + elementstoclick + " Xpath: " + xpath +
                                  " Id: " + id + " Username: " + username + " Password: " + password);

                _driver.Navigate().GoToUrl(url);

                if (login)
                {

                    Login(username, password);


                }

                Click(elementstoclick);

                IWebElement element;
                if (xpath is null)
                {
                    element = GetElementById(id);
                }
                else
                {
                    element = GetElementByXPath(xpath);
                }

                return element;

            }
            catch (Exception ex)
            {
                if (url is null)
                {
                    throw new ScrapeFailedException("The URL Cannot be Null");
                }

                if (ex is WebDriverArgumentException)
                {
                    throw new ScrapeFailedException($"Scrape failed for url {url}: URL is invalid", ex);
                }
                throw new ScrapeFailedException($"Scrape failed for url {url}",ex);
            }
        }

        
        /// <summary>
        /// /// Scrapes Website to get usernamefields for login.
        /// </summary>
        /// <returns>List of usernamefields in wich the password can be inserted</returns>
        private List<IWebElement> Getusernamefields()
        {
            List<IWebElement> transfer;
            List<IWebElement>? usernamefields = [];
            List<string> usernamefieldnames = ["usrname", "username", "email", "emailadress"];
            
            foreach (var username in usernamefieldnames)
            {
                transfer = _driver.FindElements(By.Name(username)).ToList();
                _driver.Manage().Timeouts().ImplicitWait = TimeSpan.FromSeconds(0);
                foreach (var usernamefield in transfer)
                {
                    if (usernamefield.TagName == "input")// Only return input fields
                    {
                        usernamefields.Insert(0,usernamefield);
                    }
                        
                }
                
                
            }
            _driver.Manage().Timeouts().ImplicitWait = TimeSpan.FromSeconds(_implicit_Wait);
            return usernamefields;
        }
        /// <summary>
        /// Scrapes Website to get passwordfields for login.
        /// </summary>
        /// <returns>List of passwordfields in wich the password can be inserted</returns>
        private List<IWebElement> Getpasswordfields()
        {
            _driver.Manage().Timeouts().ImplicitWait = TimeSpan.FromSeconds(0);
             List<string> passwordfieldnames = ["pssword", "password", "passwrd","pass"];
             ReadOnlyCollection<IWebElement> transfer;   
             List<IWebElement> passwordfields = [];
             foreach (var name in passwordfieldnames)
             {
                 transfer = _driver.FindElements(By.Name(name));
                 foreach (var passwordfield in transfer)
                 {
                     if (passwordfield.TagName == "input") // Only return input fields
                     {
                         passwordfields.Insert(0, passwordfield);
                     }
                 }
             }
            _driver.Manage().Timeouts().ImplicitWait = TimeSpan.FromSeconds(_implicit_Wait);
             return passwordfields;
        }
        /// <summary>
        /// clicks through elements specified by a list of xpath
        /// </summary>
        /// <param name="elementexpaths"></param>
        public void Click(List<string>? elementexpaths)
        {
            try
            {
                IWebElement element; // the element wich has to be clicked
                if (elementexpaths is null)
                {
                    return;
                }

                foreach (string xpath in elementexpaths)
                {
                    element = GetElementByXPath(xpath);
                    element.Click();
                }
            }
            catch (Exception ex)
            {
                throw new ClickactionfailedException(ex);
            }
        }

        /// <summary>
        /// Closes the WebDriver and quits the browser session.
        /// </summary>
        public void Quit()
        {
            _driver.Close();
            _driver.Quit();
        }
    }

    /// <summary>
    /// Class which connects scraping logic with the database.
    /// </summary>
    public class Scrape
    {
        
        /// <summary>
        /// Scrapes items from the list retrieved from the database using Selenium. Create notifications and also update the database.
        /// </summary>
        /// <returns>
        /// A completed <see cref="Task"/> after all items have been processed.
        /// </returns>
        public static async void ScrapeItems()
        {
            var scrapeContent = DatabaseHandler.GetScrapeContent();
            foreach (var item in scrapeContent)
            {
                var selenium = new Selenium();
                try
                {
                    IWebElement element = selenium.Scrape(item.url, item.login, item.clicks, item.xPath, item.elementId, item.username, item.password);
                    if (element.Text == item.content)
                    {
                        Console.WriteLine($"Content is still the same: {element.Text} = {item.content}");
                    } else {
                        Console.WriteLine($"Content changed: {element.Text} != {item.content}");
                        DatabaseHandler notification = new DatabaseHandler($"{item.name} had changes", $"Content changed from {item.content} to {element.Text}");
                        notification.CreateNotification();
                        DatabaseHandler.UpdateContent(item.id, element.Text);
                        SettingsManager _settingsManager = new SettingsManager();
                        SettingsModel _settings = _settingsManager.LoadSettings();
                        if (_settings.EmailNotificationsEnabled)
                        {
                            Decryption dec = new Decryption(_settings.Email, _settings.Name, _settings.Salt, _settings.IV);
                            var resultDec = dec.DecryptStringFromBytes_Aes();
                            string _email = resultDec.username;
                            string _name = resultDec.password;
                            Email emailclient = new Email();
                            emailclient.Send(_email, $"{item.name} had changes", _name,$"Content changed from {item.content} to {element.Text}");

                        }
                    }
                    
                }
                catch (Exception ex)
                {
                    DatabaseHandler notification = new DatabaseHandler($"Scraping for {item.name} failed: ", ex.Message);
                    notification.CreateNotification();
                    
                }
                finally
                {
                    try
                    {
                        selenium.Quit();
                    }
                    catch (Exception ex)
                    {
                        Console.WriteLine($"Exception during Quit for {item.name}: {ex.Message}");
                    }
                }
            }
            
            Console.WriteLine("Scraping complete");
        }    
    }
}