using OpenQA.Selenium.Chrome;
using OpenQA.Selenium;

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

        /// <summary>
        /// Initializes a new instance of the <see cref="Selenium"/> class and navigates to the specified URL.
        /// </summary>
        /// <param name="url">The URL to navigate to upon initialization.</param>
        public Selenium(string url)
        {
            var options = new ChromeOptions();
            options.AddArgument("--disable-search-engine-choice-screen");
            options.AddArgument("--headless"); // Runs Chrome in headless mode, without a GUI.
            options.AddArgument("--no-sandbox");
            options.AddArgument("--disable-dev-shm-usage");
            _driver = new ChromeDriver(options);

            _driver.Navigate().GoToUrl(url);
        }

        /// <summary>
        /// Retrieves a web element by its XPath.
        /// </summary>
        /// <param name="xpath">The XPath of the web element to find.</param>
        /// <returns>The web element found by the specified XPath.</returns>
        public IWebElement GetElementByXPath(string xpath)
        {
            return _driver.FindElement(By.XPath(xpath));
        }

        /// <summary>
        /// Retrieves a web element by its ID.
        /// </summary>
        /// <param name="id">The ID of the web element to find.</param>
        /// <returns>The web element found by the specified ID.</returns>
        public IWebElement GetElementById(string id)
        {
            return _driver.FindElement(By.Id(id));
        }

        /// <summary>
        /// Performs a login action on the web page.
        /// </summary>
        /// <param name="username">The username to be used for login.</param>
        /// <param name="password">The password to be used for login.</param>
        /// <param name="multiple">Indicates whether multiple steps are required for login (e.g., 1-step login).</param>
        public void login(string username, string password, bool multiple)
        {
            IWebElement usernamefield = _driver.FindElement(By.CssSelector("[autocomplete='username']"));
            IWebElement passwordfield;

            usernamefield.SendKeys(username);
            if (multiple)
            {
                usernamefield.SendKeys(Keys.Return); // Proceeds to the next step if multiple steps are required.
            }

            passwordfield = _driver.FindElement(By.CssSelector("[autocomplete='current-password']"));
            passwordfield.SendKeys(password);
            passwordfield.SendKeys(Keys.Return); // Submits the login form.
        }

        /// <summary>
        /// Closes the WebDriver and quits the browser session.
        /// </summary>
        public void Quit()
        {
            _driver.Quit();
        }
    }
}