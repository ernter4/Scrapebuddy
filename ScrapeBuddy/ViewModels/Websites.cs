using System.Collections.Generic;
using ScrapeBuddy.Data;
using System.Collections.ObjectModel;
using System.Data;
using System.Linq;
using CommunityToolkit.Mvvm.ComponentModel;

namespace ScrapeBuddy.Models;


public class Websites: ObservableObject
{
    
    private DataTable table;
    private List<string> sites;

    public List<string> Sites { get; set; }
    public DataTable Table
    {
        get => table;
        set { table = value; }
    }

    public Websites()
    {
        Table = GetWebsites();
        Sites = ConvertToList(Table);
    }

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

    public List<string> ConvertToList(DataTable t)
    {
        return t.AsEnumerable()
            .Select(row => row["website_name"].ToString())
            .ToList();
    }
    
    public DataTable GetWebsites()
    {
        DatabaseHandler db = new DatabaseHandler();
        DataTable results = db.GetAllWebsites();
        return results;
    }
}   