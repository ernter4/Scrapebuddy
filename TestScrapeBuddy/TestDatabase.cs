using System.Data;
using NUnit.Framework;
using ScrapeBuddy.Data;  // Make sure this matches the namespace of DatabaseHandler

[TestFixture]
public class DatabaseHandlerTests
{
    private string _databasePath;
    private DatabaseHandler _dbHandler;

    [SetUp]
    public void SetUp()
    {
        var currentDirectory = AppDomain.CurrentDomain.BaseDirectory;
        Console.WriteLine($"Current working directory: {currentDirectory}");
        _databasePath = @"Data/Database.db"; // Path to your existing database
        _dbHandler = new DatabaseHandler(_databasePath);

        // Ensure the database file exists
        Assert.That(System.IO.File.Exists(_databasePath), Is.True, "Database file does not exist.");
    }

    [Test]
    public void TestCreateUser()
    {
        // Act
        _dbHandler.CreateUser("test_user", "test_password", 1);

        // Assert
        var users = _dbHandler.GetAllRecordsFromTable("users");
        var foundRow = users.Select("username = 'test_user'").FirstOrDefault();
        Assert.That(foundRow, Is.Not.Null, "User should be inserted.");
        Assert.That(foundRow["username"], Is.EqualTo("test_user"));
        Assert.That(foundRow["password"], Is.EqualTo("test_password"));
        Assert.That(foundRow["website_id"], Is.EqualTo(1));
    }

    [Test]
    public void TestCreateWebsiteContent()
    {
        // Act
        _dbHandler.CreateWebsiteContent("test_site", "//div[@id='test']", "testId", "test content");

        // Assert
        var content = _dbHandler.GetAllRecordsFromTable("website_content");
        var foundRow = content.Select("website_name = 'test_site'").FirstOrDefault();
        Assert.That(foundRow, Is.Not.Null, "Website content should be inserted.");
        Assert.That(foundRow["website_name"], Is.EqualTo("test_site"));
        Assert.That(foundRow["x_path"], Is.EqualTo("//div[@id='test']"));
        Assert.That(foundRow["element_id"], Is.EqualTo("testId"));
        Assert.That(foundRow["content"], Is.EqualTo("test content"));
    }

    [Test]
    public void TestGetAllRecordsFromTable()
    {
        // Arrange
        _dbHandler.CreateUser("test_user_2", "test_password_2", 2);
        _dbHandler.CreateWebsiteContent("test_site_2", "//div[@id='test2']", "testId2", "test content 2");

        // Act
        var users = _dbHandler.GetAllRecordsFromTable("users");
        var content = _dbHandler.GetAllRecordsFromTable("website_content");

        // Assert
        Assert.That(users.Rows.Count, Is.GreaterThanOrEqualTo(1));
        Assert.That(content.Rows.Count, Is.GreaterThanOrEqualTo(2));
    }
    
   
}
