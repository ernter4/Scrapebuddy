using System.Data;
using Microsoft.Data.Sqlite;

namespace ScrapeBuddy.Data
{
    /// <summary>
    /// Handles database operations for ScrapeBuddy.
    /// </summary>
    public class DatabaseHandler
    {
        private readonly string _connectionString;

        /// <summary>
        /// Initializes a new instance of the <see cref="DatabaseHandler"/> class.
        /// </summary>
        /// <param name="databaseFilePath">Path to the SQLite database file.</param>
        public DatabaseHandler(string databaseFilePath)
        {
            _connectionString = $"Data Source={databaseFilePath};";
        }

        /// <summary>
        /// Creates a new user record in the database.
        /// </summary>
        /// <param name="username">The username of the new user.</param>
        /// <param name="password">The password of the new user.</param>
        /// <param name="websiteId">The ID of the associated website.</param>
        public void CreateUser(string username, string password, int websiteId)
        {
            using (var connection = new SqliteConnection(_connectionString))
            {
                connection.Open();
                var command = new SqliteCommand("INSERT INTO users (username, password, website_id) VALUES (@username, @password, @website_id)", connection);
                command.Parameters.AddWithValue("@username", username);
                command.Parameters.AddWithValue("@password", password);
                command.Parameters.AddWithValue("@website_id", websiteId);
                command.ExecuteNonQuery();
            }
        }

        /// <summary>
        /// Creates a new website content record in the database.
        /// </summary>
        /// <param name="websiteName">The name of the website.</param>
        /// <param name="xPath">The XPath of the content element.</param>
        /// <param name="elementId">The ID of the content element.</param>
        /// <param name="content">The content to be stored.</param>
        public void CreateWebsiteContent(string websiteName, string xPath, string elementId, string content)
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
