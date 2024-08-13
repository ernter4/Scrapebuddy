using System.Collections.Generic;
using ScrapeBuddy.Data;
using System.Data;
using System.Linq;
using CommunityToolkit.Mvvm.ComponentModel;

namespace ScrapeBuddy.Models
{
    /// <summary>
    /// List all Websites for the Main Page, also offers some logic to interact with them
    /// Inherits from <see cref="ObservableObject"/> to support property change notifications.
    /// </summary>
    public class Websites : ObservableObject
    {
        /// <summary>
        /// The <see cref="DataTable"/> containing website data.
        /// </summary>
        private DataTable table;

        /// <summary>
        /// A list of website names extracted from the <see cref="table"/>.
        /// </summary>
        private List<string> sites;

        /// <summary>
        /// Gets or sets the list of website names.
        /// </summary>
        public List<string?> Sites { get; set; }

        /// <summary>
        /// Gets or sets the <see cref="DataTable"/> containing website data.
        /// </summary>
        public DataTable Table
        {
            get => table;
            set { table = value; }
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="Websites"/> class.
        /// Loads website data from the database and converts it to a list of website names.
        /// </summary>
        public Websites()
        {
            Table = GetWebsites();
            Sites = ConvertToList(Table);
        }

        /// <summary>
        /// Populates the <see cref="Table"/> with test data and updates the <see cref="Sites"/> list accordingly.
        /// </summary>
        public void test()
        {
            Table = new DataTable();
            DataColumn col = new DataColumn();
            Table.Columns.Add("Columnname_Website", typeof(string));

            for (int i = 0; i < 50; i++)
            {
                var row = Table.NewRow();
                row["Columnname_Website"] = $"Eintrag {i}";
                Table.Rows.Add(row);
            }

            Sites = Table.AsEnumerable()
                .Select(row => row["Columnname_Website"].ToString())
                .ToList();   
        }

        /// <summary>
        /// Converts the given <see cref="DataTable"/> to a list of website names.
        /// </summary>
        /// <param name="t">The <see cref="DataTable"/> containing website data.</param>
        /// <returns>A list of website names extracted from the <see cref="DataTable"/>.</returns>
        public List<string?> ConvertToList(DataTable t)
        {
            return t.AsEnumerable()
                .Select(row => row["website_name"].ToString())
                .ToList();
        }

        /// <summary>
        /// Retrieves all website data from the database.
        /// </summary>
        /// <returns>A <see cref="DataTable"/> containing all website data from the database.</returns>
        public DataTable GetWebsites()
        {
            DatabaseHandler db = new DatabaseHandler();
            DataTable results = db.GetAllWebsites();
            return results;
        }
    }
}
