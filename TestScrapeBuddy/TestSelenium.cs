using OpenQA.Selenium;
using ScrapeBuddy.Logic;
namespace TestScrapeBuddy;
using ScrapeBuddy.Error;
public class SeleniumTests
{
    private Selenium? _driver;

    [SetUp]
    public void Setup()
    {
 

    }

    [Test]
    public void XPath()
    {
        
        _driver = new Selenium("https://dualis.dhbw.de");
        Thread.Sleep(1000);
        String title = _driver!.GetElementByXPath("//*[@id=\"link000280\"]").GetAttribute("Title");
        _driver.Quit();
        Assert.That(title, Is.EqualTo("Kontakt")); 
    }
    [Test]
    public void Id()
    {
        _driver = new Selenium("https://dualis.dhbw.de");
        Thread.Sleep(1000);
        String title = _driver!.GetElementById("link000340").GetAttribute("Title");
        _driver.Quit();
        Assert.That(title, Is.EqualTo("Informationen")); 
        

    }
    [Test]
    public void Login()
    {
        _driver = new Selenium("https://stackoverflow.com/users/login");
        _driver.Login("scrapebuddy@gmail.com","Mu@EPxHKF$BZ%oiNyfcdX3");
        IWebElement element = _driver.GetElementByXPath("/html/body/div[3]/div[1]/div[1]/nav/ol/li[1]/ol/li[1]/a/div/span");
        Assert.That("Home",Is.EqualTo(element.Text));
        
        _driver.Quit();
    }

    [Test]
    public void Scrape()
    {
        _driver = new Selenium();
        IWebElement element = _driver.Scrape("https://stackoverflow.com/users/login", true,
            "/html/body/div[3]/div[2]/div[1]/div[1]/div/a", null,
            "scrapebuddy@gmail.com",
            "Mu@EPxHKF$BZ%oiNyfcdX3");
        Assert.That("Ask Question",Is.EqualTo(element.Text));
        _driver.Quit();
    }
    [Test]
    public void Exception()
    {
        _driver = new Selenium();
        try
        {
            _driver.Scrape("https://www.google.com", true, "test", "test", "test", "test");
            
        }
        catch(LoginException e)
        {
            if (e is MissingUsernamefieldException)
            {
                Assert.That("No Usernamefield found",Is.EqualTo(e.Message));
            }
        }
        _driver.Quit();
    }
}


