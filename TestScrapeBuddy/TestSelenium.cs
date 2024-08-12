using ScrapeBuddy.Logic;
namespace TestScrapeBuddy;

public class SeleniumTests
{
    private Selenium? _driver;

    [SetUp]
    public void Setup()
    {
        _driver = new Selenium("https://dualis.dhbw.de");
        Thread.Sleep(1000);

    }

    [Test]
    public void XPath()
    {
        

        String title = _driver!.GetElementByXPath("//*[@id=\"link000280\"]").GetAttribute("Title");
        _driver.Quit();
        Assert.That(title, Is.EqualTo("Kontakt")); 
    }
    [Test]
    public void Id()
    {

        String title = _driver!.GetElementById("link000340").GetAttribute("Title");
        _driver.Quit();
        Assert.That(title, Is.EqualTo("Informationen")); 
        

    }
    [Test]
    public void Login()
    {
        _driver = new Selenium("https://accounts.google.com");
        _driver.login("scrapebuddy@gmail.com","qSM%gKdPzhx$8r4AJg69i#",true);
        _driver.Quit();
        
        
    }
}


