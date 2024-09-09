using ScrapeBuddy.Logic;

namespace TestScrapeBuddy;

public class TestEmail
{
    [Test]
    public void email()
    {
        Email emailclient = new Email();
        emailclient.Send("scrapebuddy@gmail.com", "Thats the test mails title", "Scrape buddy", "Thats the test mails boddy ");
    }
}