using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using Microsoft.Data.Sqlite;
using ScrapeBuddy.Logic;

namespace ScrapeBuddy.Data
{
    /// <summary>
    /// Handles database operations for ScrapeBuddy.
    /// </summary>
    public class DatabaseHandler
    {
        private static readonly string _connectionString = @"Data Source=Data/Database.db;";
        public string websiteName;
        public string url;
        public string? xPath;
        public string? elementId;
        public string content;
        public bool login;
        public string? username;
        public string? password;
        public string notification;
        public string title;
        public bool dismiss;
        public List<string>? clicks;

        /// <summary>
        /// Initializes a new instance of the <see cref="DatabaseHandler"/> class (used to create scrape object).
        /// </summary>
        /// <param name="databaseFilePath">Path to the SQLite database file.</param>
        /// <param name="websiteName">The name of the website.</param>
        /// <param name="url">The url of the website.</param>
        /// <param name="xPath">The XPath of the content element.</param>
        /// <param name="elementId">The ID of the content element.</param>
        /// <param name="content">The content to be stored.</param>
        /// <param name="login">Indicator if login exists.</param>
        /// <param name="username">The username for the user record.</param>
        /// <param name="password">The password for the user record.</param>
        public DatabaseHandler(string websiteName, string url, List<string>? clicks, string? xPath, string? elementId, string content, bool login, string? username, string? password)
        {
            this.websiteName = websiteName;
            this.url = url;
            this.clicks = clicks;
            this.xPath = xPath;
            this.elementId = elementId;
            this.content = content;
            this.login = login;
            this.username = username;
            this.password = password;
        }
        
        /// <summary>
        /// Initializes a new instance of the <see cref="DatabaseHandler"/> class (used to create notification object).
        /// </summary>
        /// <param name="title"Title of the notification.</param>
        /// <param name="notification">Value of the notification.</param>
        public DatabaseHandler(string title, string notification) {
            this.title = title;
            this.notification = notification;
            this.dismiss = false; 
        }
        
        public DatabaseHandler() {}
        
        /// <summary>
        /// Ceate entry for notification table.
        /// </summary>
        public void CreateNotification(){
            using (var connection = new SqliteConnection(_connectionString))
            {
                connection.Open();
                
                var command = new SqliteCommand("INSERT INTO notification (title, notification, dismiss) VALUES (@title, @notification, @dismiss)", connection);
                command.Parameters.AddWithValue("@title", title);
                command.Parameters.AddWithValue("@notification", notification);
                command.Parameters.AddWithValue("@dismiss", dismiss);
                command.ExecuteNonQuery();
            }
        }

        /// <summary>
        /// Creates a new user record in the database.
        /// </summary>
        /// <param name="websiteId">The ID of the associated website.</param>
        public void CreateUser(int websiteId)
        {
            using (var connection = new SqliteConnection(_connectionString))
            {
                connection.Open();

                if (username == null || password == null)
                {
                    throw new InvalidOperationException("Username or password cannot be null.");
                }

                // Encrypt the username and password
                Encryption enc = new Encryption(username, password);
                var result = enc.EncryptStringToBytes_Aes();

                var command = new SqliteCommand("INSERT INTO users (username, password, salt, IV, website_id) VALUES (@username, @password, @salt, @IV, @website_id)", connection);
                command.Parameters.AddWithValue("@username", result.encUsername);
                command.Parameters.AddWithValue("@password", result.encPassword);
                command.Parameters.AddWithValue("@salt", result.salt);
                command.Parameters.AddWithValue("@IV", result.IV);
                command.Parameters.AddWithValue("@website_id", websiteId);

                command.ExecuteNonQuery();
            }
        }

        /// <summary>
        /// Creates a new website content record in the database.
        /// </summary>
        public void CreateWebsiteContent()
        {
            using (var connection = new SqliteConnection(_connectionString))
            {
                connection.Open();
                
                
                
                string? serializedClicks = clicks != null ? string.Join(",", clicks) : null;
                
                if (clicks?.Count == 0)
                {
                    serializedClicks = null;
                }
                
                var command = new SqliteCommand("INSERT INTO website_content (website_name, url, clicks, x_path, element_id, content, login) VALUES (@websiteName, @url, @clicks, @xPath, @elementId, @content, @login)", connection);
                command.Parameters.AddWithValue("@websiteName", websiteName);
                command.Parameters.AddWithValue("@url", url);
                command.Parameters.AddWithValue("@clicks", serializedClicks ?? (object)DBNull.Value);
                command.Parameters.AddWithValue("@xPath", xPath ?? (object)DBNull.Value);
                command.Parameters.AddWithValue("@elementId", elementId ?? (object)DBNull.Value);
                command.Parameters.AddWithValue("@content", content);
                command.Parameters.AddWithValue("@login", login ? 1 : 0);
        
                command.ExecuteNonQuery();

                // Retrieve the last inserted ID
                var lastInsertIdCommand = new SqliteCommand("SELECT last_insert_rowid()", connection);
                int insertedId = Convert.ToInt32(lastInsertIdCommand.ExecuteScalar());
        
                if (login)
                {
                    CreateUser(insertedId);
                }
            }
        }
        
        /// <summary>
        /// Get all undismissed Notifications
        /// </summary>
        static public int GetNotificationsUndismissed()
        {
            using (var connection = new SqliteConnection(_connectionString))
            {
                connection.Open();

                string query = @"
                SELECT 
                    COUNT(*)
                FROM 
                    notification
                WHERE
                    dismiss = 0";

                using (var command = new SqliteCommand(query, connection))
                {
                    using (var reader = command.ExecuteReader())
                    {
                        while (reader.Read())
                        {
                            return (reader.GetInt32(0));
                        }
                    }
                }
            }

            return 0;
        }

        
        /// <summary>
        /// Delete all Notifications 
        /// </summary>
        static public void DeleteAllNotitifications(){
            using (var connection = new SqliteConnection(_connectionString))
            {
                connection.Open();

                using (var command = new SqliteCommand("DELETE FROM notification", connection))
                {
                    command.ExecuteNonQuery();
                }
            }
        }
        
        /// <summary>
        /// Delete a specific notification (based on id).
        /// </summary>
        static public void DeleteNotitification(int id){
            using (var connection = new SqliteConnection(_connectionString))
            {
                connection.Open();

                using (var command = new SqliteCommand("DELETE FROM notification WHERE id = @notification_id", connection))
                {
                    command.Parameters.AddWithValue("@notification_id", id);
                    command.ExecuteNonQuery();
                }
            }
        }
        
        /// <summary>
        /// Dismiss all notifications
        /// </summary> 
        static public void DismissAllNotitifications(){
            using (var connection = new SqliteConnection(_connectionString))
            {
                connection.Open();
                
                using (var command = new SqliteCommand("UPDATE notification SET dismiss = 1", connection))
                {
                    command.ExecuteNonQuery();
                } 
            }
        }
        
        /// <summary>
        /// Update content for website when it has changed (Scrape.ScrapeItems looks for changes)
        /// </summary>
        /// <param name="id">The id for therecord.</param>
        /// <param name="content">The new content.</param>
        static public void UpdateContent(int id, string content)
        {
            try
            {
                using (var connection = new SqliteConnection(_connectionString))
                {
                    connection.Open();
                    using (var command = new SqliteCommand("UPDATE website_content SET content = @content WHERE id = @id", connection))
                    {
                        command.Parameters.AddWithValue("@content", content);
                        command.Parameters.AddWithValue("@id", id);
                        int rowsAffected = command.ExecuteNonQuery();
                    }
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error: {ex.Message}");
            }
        }
        
        // <summary>
        /// Updates  the user record in the database.
        /// </summary>
        /// <param name="websiteId">The ID of the associated website.</param>
        public void UpdateUser(int websiteId)
        {
            using (var connection = new SqliteConnection(_connectionString))
            {
                connection.Open();

                if (string.IsNullOrEmpty(username) || string.IsNullOrEmpty(password))
                {
                    throw new InvalidOperationException("Username or password cannot be null or empty.");
                }
                
                Encryption enc = new Encryption(username, password);
                var result = enc.EncryptStringToBytes_Aes();
                
                var command = new SqliteCommand(
                    "UPDATE users SET username = @username, password = @password, salt = @salt, IV = @IV WHERE website_id = @website_id", 
                    connection
                );
                
                command.Parameters.AddWithValue("@username", result.encUsername);
                command.Parameters.AddWithValue("@password", result.encPassword);
                command.Parameters.AddWithValue("@salt", result.salt);
                command.Parameters.AddWithValue("@IV", result.IV);
                command.Parameters.AddWithValue("@website_id", websiteId);

                int rowsAffected = command.ExecuteNonQuery();
                
                if (rowsAffected > 0)
                {
                    Console.WriteLine("User updated successfully.");
                }
                else
                {
                    Console.WriteLine("No user found with the provided website_id.");
                }
            }
        }
        
        
        // <summary>
        /// Update Website.
        /// </summary>
        /// <param name="websiteId">The ID of the associated website.</param>
        public void UpdateWebsite(int id)
        {
            try
            {
                using (var connection = new SqliteConnection(_connectionString))
                {
                    connection.Open();
                    
                    string? serializedClicks = clicks != null ? string.Join(",", clicks) : null;
                
                    if (clicks?.Count == 0)
                    {
                        serializedClicks = null;
                    }
                    
                    using (var command = new SqliteCommand("UPDATE website_content SET website_name = @websiteName, url = @url, clicks = @clicks, x_path = @xPath, element_id = @elementId, content = @content, login = @login WHERE id = @id", connection))
                    {
                        command.Parameters.AddWithValue("@id", id);
                        command.Parameters.AddWithValue("@websiteName", websiteName ?? (object)DBNull.Value);
                        command.Parameters.AddWithValue("@url", url ?? (object)DBNull.Value);
                        command.Parameters.AddWithValue("@clicks", serializedClicks ?? (object)DBNull.Value);
                        command.Parameters.AddWithValue("@xPath", xPath ?? (object)DBNull.Value);
                        command.Parameters.AddWithValue("@elementId", elementId ?? (object)DBNull.Value);
                        command.Parameters.AddWithValue("@content", content ?? (object)DBNull.Value);
                        command.Parameters.AddWithValue("@login", login ? 1 : 0); 

                        int rowsAffected = command.ExecuteNonQuery();
                    }
                    
                    if (login)
                    {
                        using (var checkUserCommand = new SqliteCommand("SELECT COUNT(1) FROM users WHERE website_id = @id", connection))
                        {
                            checkUserCommand.Parameters.AddWithValue("@id", id);
                            
                            int usersExists = Convert.ToInt32(checkUserCommand.ExecuteScalar());

                            if (usersExists > 0)
                            {
                                UpdateUser(id);
                            }
                            else
                            {
                                CreateUser(id);
                            }
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error: {ex.Message}");
            }
        }
        
        /// <summary>
        /// Deletes the website content and associated user records for a specified website ID.
        /// </summary>
        /// <param name="websiteId">The ID of the website to delete from the database.</param>
        static public void DeleteScrapeItem(int websiteId)
        {
            using (var connection = new SqliteConnection(_connectionString))
            {
                connection.Open();
                
                using (var command = new SqliteCommand("DELETE FROM website_content WHERE id = @website_id", connection))
                {
                    command.Parameters.AddWithValue("@website_id", websiteId);
                    command.ExecuteNonQuery();
                }
                
                using (var command = new SqliteCommand("DELETE FROM users WHERE website_id = @website_id", connection))
                {
                    command.Parameters.AddWithValue("@website_id", websiteId);
                    command.ExecuteNonQuery();
                } 
            }
        }

        /// <summary>
        /// Deletes All Tables if the User forgot the passphrase.
        /// </summary>
        static public void DeleteAllTables()
        {
            using (var connection = new SqliteConnection(_connectionString))
            {
                connection.Open();
                
                using (var command = new SqliteCommand("DELETE FROM website_content", connection))
                {
                    command.ExecuteNonQuery();
                }
                
                using (var command = new SqliteCommand("DELETE FROM users", connection))
                {
                    command.ExecuteNonQuery();
                } 
            }
        }

        /// <summary>
        /// Retrieves the content to be scraped from the database.
        /// </summary>
        /// <returns>
        /// A list of <see cref="ScrapeContent"/> objects that contain information necessary for web scraping.
        /// </returns>
        static public List<ScrapeContent> GetScrapeContent()
        {
            var scrapeContents = new List<ScrapeContent>();

            using (var connection = new SqliteConnection(_connectionString))
            {
                connection.Open();

                string query = @"
                SELECT 
                    wc.id,
                    wc.website_name,
                    wc.url, 
                    wc.clicks,
                    wc.x_path, 
                    wc.element_id,
                    wc.content,
                    wc.login, 
                    u.salt,
                    u.IV,
                    u.username, 
                    u.password
                FROM 
                    website_content wc
                LEFT JOIN 
                    users u ON wc.id = u.website_id";

                using (var command = new SqliteCommand(query, connection))
                {
                    using (var reader = command.ExecuteReader())
                    {
                        while (reader.Read())
                        {
                            string? decUsername = null;
                            string? decPassword = null;

                            if (!reader.IsDBNull(10) && !reader.IsDBNull(11))
                            {
                                byte[] salt = reader.GetFieldValue<byte[]>(8);
                                byte[] IV = reader.GetFieldValue<byte[]>(9);
                                Decryption dec = new Decryption(
                                    reader.GetFieldValue<byte[]>(10), 
                                    reader.GetFieldValue<byte[]>(11), 
                                    salt, 
                                    IV
                                );
                                var resultDec = dec.DecryptStringFromBytes_Aes();
                                decUsername = resultDec.username;
                                decPassword = resultDec.password;
                            }

                            var scrapeContent = new ScrapeContent
                            {
                                id = reader.GetInt32(0),
                                name = reader.GetString(1),
                                url = reader.GetString(2),
                                clicks = reader.IsDBNull(3) 
                                    ? null 
                                    : reader.GetString(3).Split(',').ToList(),
                                xPath = reader.IsDBNull(4) 
                                    ? null 
                                    : reader.GetString(4),  // Fixed index
                                elementId = reader.IsDBNull(5) 
                                    ? null 
                                    : reader.GetString(5),  // Fixed index
                                content = reader.GetString(6),
                                login = reader.GetBoolean(7),
                                username = decUsername,
                                password = decPassword
                            };

                            scrapeContents.Add(scrapeContent);
                        }
                    }
                }
            }
            return scrapeContents;
        }

        
        /// <summary>
        /// Retrieves the content of all notifications.
        /// </summary>
        /// <returns>
        /// A list of <see cref="NotificationContent"/> objects that contain information necessary notifications. 
        /// </returns>
        static public List<NotificationContent> GetNotifications()
        {
            var notifications = new List<NotificationContent>();
            using (var connection = new SqliteConnection(_connectionString))
            {
                connection.Open();

                string query = @"
                SELECT 
                    id,
                    title,
                    notification,
                    dismiss
                FROM 
                    notification
                ORDER BY 
                    id DESC";

                using (var command = new SqliteCommand(query, connection))
                {
                    using (var reader = command.ExecuteReader())
                    {
                        while (reader.Read())
                        {
                            var notificationContent = new NotificationContent
                            {
                                id = reader.GetInt32(0),
                                title = reader.GetString(1),
                                notification = reader.GetString(2),
                                dismiss = reader.GetBoolean(3)
                            };

                            notifications.Add(notificationContent);
                        }
                    }
                }
            }

            return notifications;
        }
        
        /// <summary>
        /// Gets all registered websites of a user
        /// </summary>
        /// <returns>A <see cref="DataTable"/> containing one column with all websitenames + ids</returns>
        public DataTable GetAllWebsites()
        {
            var resultTable = new DataTable();

            using (var connection = new SqliteConnection(_connectionString))
            {
                connection.Open();
                var command = new SqliteCommand("SELECT id, website_name FROM website_content ORDER BY website_name COLLATE NOCASE ASC", connection);


                using (var reader = command.ExecuteReader())
                {
                    resultTable.Load(reader);
                }
            }

            return resultTable;
        }
        
        /// <summary>
        /// Decrypts all user data from the database.
        /// </summary>
        public List<DecryptedUser> DecryptAllUserData()
        {
            var decryptedUsers = new List<DecryptedUser>();

            using (var connection = new SqliteConnection(_connectionString))
            {
                connection.Open();

                string query = @"
                SELECT 
                    id, 
                    username, 
                    password, 
                    salt, 
                    IV 
                FROM 
                    users";

                using (var command = new SqliteCommand(query, connection))
                {
                    using (var reader = command.ExecuteReader())
                    {
                        while (reader.Read())
                        {
                            byte[] salt = reader.GetFieldValue<byte[]>(3);
                            byte[] IV = reader.GetFieldValue<byte[]>(4);
                            byte[] encryptedUsername = reader.GetFieldValue<byte[]>(1);
                            byte[] encryptedPassword = reader.GetFieldValue<byte[]>(2);

                            // Decrypt data
                            Decryption decryption = new Decryption(encryptedUsername, encryptedPassword, salt, IV);
                            var result = decryption.DecryptStringFromBytes_Aes();

                            decryptedUsers.Add(new DecryptedUser
                            {
                                Id = reader.GetInt32(0),
                                Username = result.username,
                                Password = result.password
                            });
                        }
                    }
                }
            }

            return decryptedUsers;
        }

        /// <summary>
        /// Re-encrypts and updates user data in the database with a new passphrase.
        /// </summary>
        public void ReEncryptUserData(List<DecryptedUser> decryptedUsers)
        {
            using (var connection = new SqliteConnection(_connectionString))
            {
                connection.Open();

                foreach (var user in decryptedUsers)
                {
                    Encryption encryption = new Encryption(user.Username, user.Password);
                    var result = encryption.EncryptStringToBytes_Aes();

                    var command = new SqliteCommand("UPDATE users SET username = @username, password = @password, salt = @salt, IV = @IV WHERE id = @id", connection);
                    command.Parameters.AddWithValue("@username", result.encUsername);
                    command.Parameters.AddWithValue("@password", result.encPassword);
                    command.Parameters.AddWithValue("@salt", result.salt);
                    command.Parameters.AddWithValue("@IV", result.IV);
                    command.Parameters.AddWithValue("@id", user.Id);

                    command.ExecuteNonQuery();
                }
            }
        }
    }
    
    /// <summary>
    /// Represents the content required for scraping a website.
    /// </summary>
    public class ScrapeContent
    {
        public int id { get; set; }
        public string name { get; set; }
        public string url { get; set; }
        
        public List<string>? clicks { get; set; }
        public string? elementId { get; set; }
        public string? xPath { get; set; }
        public string content { get; set; }
        public bool login { get; set; }
        public string? username { get; set; }
        public string? password { get; set; }
    }
    
    /// <summary>
    /// Represents the content of notifications.
    /// </summary>
    public class NotificationContent
    {
        public int id { get; set; }
        public string title { get; set; }
        public string notification { get; set; }
        public bool dismiss { get; set; }
    }
    
    /// <summary>
    /// Represents user data used for change passphrase.
    /// </summary>
    public class DecryptedUser
    {
        public int Id { get; set; }
        public string Username { get; set; }
        public string Password { get; set; }
    }
}