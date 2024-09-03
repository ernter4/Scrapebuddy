using NUnit.Framework;
using ScrapeBuddy.Data;
using System;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using System.Collections.Generic;
using ScrapeBuddy.Logic;

namespace TestScrapeBuddy
{
    [TestFixture]
    public class DatabaseHandlerTests
    {
        private string _databasePath = @"Data/Database.db";
        private DatabaseHandler _dbHandler;

        [SetUp]
        public async Task SetUp()
        {
            Hashing hash = new Hashing("Hallo123!");
            await hash.GenerateHashAsync();
            hash.StoreMasterKey(); 
            var currentDirectory = AppDomain.CurrentDomain.BaseDirectory;
            Console.WriteLine($"Current working directory: {currentDirectory}");

            // Ensure the database file exists
            Assert.That(File.Exists(_databasePath), Is.True, "Database file does not exist.");

            // Optional: Initialize your database with some test data if needed
            // For example, you could call a method to insert test data.
        }
        
        [Test]
        public void TestCreateWebsiteContent()
        {
            _dbHandler = new DatabaseHandler("Website1", "https://test", null,"xPath1", 
                "elementId1", "content1", false, null, null);
            
            // Act
            _dbHandler.CreateWebsiteContent();
            
            // Assert
            var scrapeContent = DatabaseHandler.GetScrapeContent();
            var foundItem = scrapeContent.FirstOrDefault(item => item.name == "Website1");
            Assert.That(foundItem, Is.Not.Null, "Website content should be inserted.");
            Assert.That(foundItem?.name, Is.EqualTo("Website1"));
            Assert.That(foundItem?.url, Is.EqualTo("https://test"));
            Assert.That(foundItem?.xPath, Is.EqualTo("xPath1"));
            Assert.That(foundItem?.elementId, Is.EqualTo("elementId1"));
            Assert.That(foundItem?.login, Is.EqualTo(false));
            Assert.That(foundItem?.username, Is.Null);
            Assert.That(foundItem?.password, Is.Null);
        }
        
        [Test]
        public void TestCreateWebsiteWithLogin()
        {
            _dbHandler = new DatabaseHandler("Website2", "https://test2", null,"xPath2", 
                null, "content2", true, "password2", "password1");
            
            // Act
            _dbHandler.CreateWebsiteContent();
            
            // Assert
            var scrapeContent = DatabaseHandler.GetScrapeContent();
            var foundItem = scrapeContent.FirstOrDefault(item => item.name == "Website2");
            Assert.That(foundItem, Is.Not.Null, "Website content should be inserted.");
            Assert.That(foundItem?.name, Is.EqualTo("Website2"));
            Assert.That(foundItem?.url, Is.EqualTo("https://test2"));
            Assert.That(foundItem?.xPath, Is.EqualTo("xPath2"));
            Assert.That(foundItem?.elementId, Is.Null);
            Assert.That(foundItem?.login, Is.EqualTo(true));
            Assert.That(foundItem?.username, Is.EqualTo("password2"));
            Assert.That(foundItem?.password, Is.EqualTo("password1"));
        }

        [Test]
        public void TestGetScrapeContent()
        {
            var scrapeContent = DatabaseHandler.GetScrapeContent();
            Console.WriteLine(scrapeContent);
            foreach (var item in scrapeContent)
            {
                Console.WriteLine("Name: " + item.name);
                Console.WriteLine("URL: " + item.url);
                Console.WriteLine("Element ID: " + (item.elementId ?? "N/A"));
                Console.WriteLine("XPath: " + (item.xPath ?? "N/A"));
                Console.WriteLine("Requires Login: " + item.login);
                Console.WriteLine("Username: " + (item.username ?? "N/A"));
                Console.WriteLine("Password: " + (item.password ?? "N/A"));
                Console.WriteLine("---------------------------------------------------");
            }
        }
        
        [Test]
        public void TestDeleteScrapeItem()
        {
            // First, create some test data
            _dbHandler = new DatabaseHandler("TestWebsiteToDelete", "https://delete.test", null,"deleteXPath", 
                "deleteElementId", "deleteContent", false, "test", "test");
            _dbHandler.CreateWebsiteContent();

            // Retrieve the inserted item
            var scrapeContentBeforeDelete = DatabaseHandler.GetScrapeContent();
            var itemToDelete = scrapeContentBeforeDelete.FirstOrDefault(item => item.name == "TestWebsiteToDelete");
            Assert.That(itemToDelete, Is.Not.Null, "Website content should be inserted for deletion test.");
    
            int idToDelete = itemToDelete?.id ?? throw new InvalidOperationException("Item to delete not found.");

            // Act: Delete the item
            DatabaseHandler.DeleteScrapeItem(idToDelete);

            // Assert: Check that the item was deleted
            var scrapeContentAfterDelete = DatabaseHandler.GetScrapeContent();
            var deletedItem = scrapeContentAfterDelete.FirstOrDefault(item => item.id == idToDelete);
            Assert.That(deletedItem, Is.Null, "Website content should be deleted.");
        }

        [Test]
        public void TestCreateNotification()
        {
            // Arrange
            string notificationTitle = "Test Notification";
            string notificationContent = "This is a test notification.";
            _dbHandler = new DatabaseHandler(notificationTitle, notificationContent);

            // Act
            _dbHandler.CreateNotification();

            // Assert
            var notifications = DatabaseHandler.GetNotifications();
            var foundNotification = notifications.FirstOrDefault(n => n.title == notificationTitle && n.notification == notificationContent);
            Assert.That(foundNotification, Is.Not.Null, "Notification should be inserted.");
            Assert.That(foundNotification?.title, Is.EqualTo(notificationTitle));
            Assert.That(foundNotification?.notification, Is.EqualTo(notificationContent));
            Assert.That(foundNotification?.dismiss, Is.False);
        }

        [Test]
        public void TestDeleteAllNotitifications()
        {
            // Act: Delete all notifications
            DatabaseHandler.DeleteAllNotitifications();

            // Assert: Ensure all notifications are deleted
            var notifications = DatabaseHandler.GetNotifications();
            Assert.That(notifications.Count, Is.EqualTo(0), "All notifications should be deleted.");
        }

        [Test]
        public void TestDeleteNotitification()
        {
            // Arrange: Create a test notification
            string notificationTitle = "Notification to Delete";
            string notificationContent = "This notification will be deleted.";
            _dbHandler = new DatabaseHandler(notificationTitle, notificationContent);
            _dbHandler.CreateNotification();

            // Retrieve the inserted notification
            var notifications = DatabaseHandler.GetNotifications();
            var notificationToDelete = notifications.FirstOrDefault(n => n.title == notificationTitle);
            Assert.That(notificationToDelete, Is.Not.Null, "Notification should be inserted for deletion test.");

            int idToDelete = notificationToDelete?.id ?? throw new InvalidOperationException("Notification to delete not found.");

            // Act: Delete the notification
            DatabaseHandler.DeleteNotitification(idToDelete);

            // Assert: Ensure the notification is deleted
            notifications = DatabaseHandler.GetNotifications();
            var deletedNotification = notifications.FirstOrDefault(n => n.id == idToDelete);
            Assert.That(deletedNotification, Is.Null, "Notification should be deleted.");
        }

        [Test]
        public void TestDismissAllNotitifications()
        {
            // Arrange: Ensure there are notifications to dismiss
            _dbHandler = new DatabaseHandler("DismissTest1", "Dismiss this notification.");
            _dbHandler.CreateNotification();
            _dbHandler = new DatabaseHandler("DismissTest2", "Dismiss this notification too.");
            _dbHandler.CreateNotification();

            // Act: Dismiss all notifications
            DatabaseHandler.DismissAllNotitifications();

            // Assert: Ensure all notifications are dismissed
            var notifications = DatabaseHandler.GetNotifications();
            Assert.That(notifications.All(n => n.dismiss), Is.True, "All notifications should be dismissed.");
        }

        [Test]
        public void TestGetNotifications()
        {
            // Arrange: Ensure there are some notifications in the database
            string notificationTitle = "Test Get Notifications";
            string notificationContent = "This is a test notification for retrieval.";
            _dbHandler = new DatabaseHandler(notificationTitle, notificationContent);
            _dbHandler.CreateNotification();

            // Act: Retrieve the notifications
            var notifications = DatabaseHandler.GetNotifications();

            // Assert: Ensure the notifications are retrieved correctly
            Assert.That(notifications.Count, Is.GreaterThan(0), "Notifications should be retrieved.");
            var foundNotification = notifications.FirstOrDefault(n => n.title == notificationTitle);
            Assert.That(foundNotification, Is.Not.Null, "Inserted notification should be retrieved.");
            Assert.That(foundNotification?.title, Is.EqualTo(notificationTitle));
            Assert.That(foundNotification?.notification, Is.EqualTo(notificationContent));
        }
    }
}
