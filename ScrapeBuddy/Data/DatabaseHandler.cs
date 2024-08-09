using System;
using System.Data;
using Microsoft.Data.Sqlite;
using ScrapeBuddy.Logic;

namespace ScrapeBuddy.Data
{
    /// <summary>
    /// Handles database operations for ScrapeBuddy.
    /// </summary>
    public class DatabaseHandler
    {
        private readonly string _connectionString;
        public string websiteName;
        public string xPath;
        public string elementId;
        public string content;
        public string? username;
        public string? password;

        /// <summary>
        /// Initializes a new instance of the <see cref="DatabaseHandler"/> class.
        /// </summary>
        /// <param name="databaseFilePath">Path to the SQLite database file.</param>
        /// <param name="websiteName">The name of the website.</param>
        /// <param name="xPath">The XPath of the content element.</param>
        /// <param name="elementId">The ID of the content element.</param>
        /// <param name="content">The content to be stored.</param>
        /// <param name="username">The username for the user record.</param>
        /// <param name="password">The password for the user record.</param>
        public DatabaseHandler(string databaseFilePath, string websiteName, string xPath, string elementId, string content, string username, string password)
        {
            _connectionString = $"Data Source={databaseFilePath};";
            this.websiteName = websiteName;
            this.xPath = xPath;
            this.elementId = elementId;
            this.content = content;
            this.username = username;
            this.password = password;
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
        
                // Correct SQL command with appropriate placeholders
                string key = "key";
                Encryption enc = new Encryption(username!, password!, key);
                var result = enc.EncryptStringToBytes_Aes();

                var command = new SqliteCommand("INSERT INTO users (username, password, salt, IV, website_id) VALUES (@username, @password, @salt, @IV, @website_id)", connection);
                command.Parameters.AddWithValue("@username", username ?? (object)DBNull.Value);
                command.Parameters.AddWithValue("@password", password ?? (object)DBNull.Value);
                command.Parameters.AddWithValue("@salt", result.salt);
                command.Parameters.AddWithValue("@IV", result.IV);
                command.Parameters.AddWithValue("@website_id", websiteId);
        
                command.ExecuteNonQuery();
            } 
        }

        /// <summary>
        /// Creates a new website content record in the database.
        /// </summary>
        /// <returns>The ID of the newly inserted website content record.</returns>
        public int CreateWebsiteContent()
        {
            using (var connection = new SqliteConnection(_connectionString))
            {
                connection.Open();
                var command = new SqliteCommand("INSERT INTO website_content (website_name, x_path, element_id, content) VALUES (@websiteName, @xPath, @elementId, @content)", connection);
                command.Parameters.AddWithValue("@websiteName", websiteName);
                command.Parameters.AddWithValue("@xPath", xPath);
                command.Parameters.AddWithValue("@elementId", elementId);
                command.Parameters.AddWithValue("@content", content);
                command.ExecuteNonQuery();

                // Retrieve the last inserted ID
                var lastInsertIdCommand = new SqliteCommand("SELECT last_insert_rowid()", connection);
                int insertedId = Convert.ToInt32(lastInsertIdCommand.ExecuteScalar());
                return insertedId;
            }
        }

        /// <summary>
        /// Retrieves all records from a specified table.
        /// </summary>
        /// <param name="tableName">The name of the table to query.</param>
        /// <returns>A <see cref="DataTable"/> containing all records from the specified table.</returns>
        public DataTable GetAllRecordsFromTable(string tableName)
        {
            var resultTable = new DataTable();

            using (var connection = new SqliteConnection(_connectionString))
            {
                connection.Open();
                var command = new SqliteCommand($"SELECT * FROM {tableName}", connection);

                using (var reader = command.ExecuteReader())
                {
                    resultTable.Load(reader);
                }
            }

            return resultTable;
        }
    }
}
