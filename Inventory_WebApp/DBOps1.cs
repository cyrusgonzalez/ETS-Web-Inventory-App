using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Data.SQLite;
using System.Data;

namespace Inventory_WebApp
{
    /// <summary>
    /// TODO: Shift DB ops to this class when they work, for modularity and Single Responsibility and all that
    /// </summary>
    public class Database_Operations
    {
        /// <summary>
        /// Query argument Validation function 
        /// Attack character Ref: https://www.codementor.io/@satyaarya/prevent-sql-injection-attacks-in-net-ocfxkhnyf
        /// </summary>
        /// <param name="command"></param>
        /// <returns></returns>
        String CustomValidation(String command)
        {
            String temp = command.ToLower();
            temp.Replace("--", "");     //avoids commenting the rest of the query out
            temp.Replace("+", "");
            temp.Replace("||", "");
            temp.Replace("concat", "");
            temp.Replace("%", "");
            temp.Replace("", "");
            return temp;
        }

        /// <summary>
        /// Function to Create a default table given its name
        /// </summary>
        /// <param name="sql_conn">the sqlite connection object to the database</param>
        /// <param name="tablename">the name of the table</param>
        /// Ref: https://www.codeguru.com/csharp/.net/net_data/using-sqlite-in-a-c-application.html
        void CreateTable(SQLiteConnection sql_conn, String tablename)
        {
            SQLiteCommand sqlite_cmd;
            string Createsql = "CREATE TABLE " + CustomValidation(tablename) + "(Col1 VARCHAR(20), Col2 INT)";
            sqlite_cmd = sql_conn.CreateCommand();
            sqlite_cmd.CommandText = Createsql;
            sqlite_cmd.ExecuteNonQuery();
        }

        void ReadTable(SQLiteConnection sql_conn, String tablename)
        {
            SQLiteDataReader sqlite_datareader;
            SQLiteCommand sqlite_cmd;
            DataTable dt = new DataTable("Items");

            sqlite_cmd = sql_conn.CreateCommand();
            sqlite_cmd.CommandText = "SELECT * FROM " + tablename;

            sqlite_datareader = sqlite_cmd.ExecuteReader();
            while (sqlite_datareader.Read())
            {
                String myreader = sqlite_datareader.GetString(0);
                Console.WriteLine(myreader);
                //dt.Rows.Add(myreader.ToArray<DataRow>());
            }
            sql_conn.Close();
        }

    }
}