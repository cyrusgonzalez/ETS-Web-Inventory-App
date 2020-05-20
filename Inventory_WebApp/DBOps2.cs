using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Data;
using System.Data.SQLite;
using System.Xml;
using System.IO;
using System.Security;

namespace Inventory_WebApp
{
    /// <summary>
    /// Class of SQLite DB manipulation functions written with reference from:
    /// http://zetcode.com/csharp/sqlite/
    /// https://docs.microsoft.com/en-us/dotnet/csharp/programming-guide/classes-and-structs/constructors
    /// https://support.microsoft.com/en-us/help/307548/how-to-read-xml-from-a-file-by-using-visual-c
    /// </summary>
    public class DBOps
    {
  
        private string DataBaseSource = "Data Source=C:\\Users\\sanketm\\Documents\\ETS_Inventory\\sample_inventory.db";
        private XmlTextReader config;
        private StreamWriter logger;
        
        void InitLogger()
        {
            string logFile = "C:\\Users\\sanketm\\Documents\\ETS_Inventory\\log_" + DateTime.Now.ToFileTimeUtc().ToString() + ".txt";

            try
            {
                // Open the file
                this.logger = new StreamWriter(logFile);
                logger.WriteLine("Logger initialized on " + DateTime.Now.ToString());
            }
            catch (Exception ex)
            {
                if
                (
                    ex is UnauthorizedAccessException
                    || ex is ArgumentNullException
                    || ex is PathTooLongException
                    || ex is DirectoryNotFoundException
                    || ex is NotSupportedException
                    || ex is ArgumentException
                    || ex is SecurityException
                    || ex is IOException
                )
                {
                    throw new Exception("Failed to create log file: " + ex.Message);
                }
                else
                {
                    // Unexpected failure
                    throw;
                }
            }
        }

        void InitConfig()
        {
            config = new XmlTextReader("C:\\Users\\sanketm\\source\\repos\\Inventory_WebApp\\Inventory_WebApp\\properties.xml");
            while (config.Read())
            {
                switch (config.NodeType)
                {
                    case XmlNodeType.Element: // The node is an element.
                        Console.Write("<" + config.Name);
                        Console.WriteLine(">");
                        break;
                    case XmlNodeType.Text: //Display the text in each element.
                        Console.WriteLine(config.Value);
                        break;
                    case XmlNodeType.EndElement: //Display the end of the element.
                        Console.Write("</" + config.Name);
                        Console.WriteLine(">");
                        break;
                }
            }
            //Console.ReadLine();   
        }
        public DBOps()
        {
            InitConfig();
            InitLogger();
            string path;
            XmlDocument conf = new XmlDocument();
            conf.Load("C:\\Users\\sanketm\\source\\repos\\Inventory_WebApp\\Inventory_WebApp\\properties.xml");
            XmlNodeList nodeList = (conf.SelectNodes("properties"));

            foreach (XmlNode elem in nodeList)
            {
                path = elem.ChildNodes[0].InnerText;
                //path = elem.Attributes[0].Value;    //here attributes refer to the  
                //string path2 = elem.("datasource").Value;
                if (path != null)
                {
                    this.DataBaseSource = "data source=" + conf.GetElementsByTagName("datasource")[0].ChildNodes[0].Value;
                }
            }
        }

        public void CheckVersion()
        {
            string version;
            try
            {

                string sqlcmd = "SELECT SQLITE_VERSION();";

                using (var con = new SQLiteConnection(DataBaseSource))
                {
                    con.Open();
                    using (var cmd = new SQLiteCommand(sqlcmd, con))
                    {
                        string ver = cmd.ExecuteScalar().ToString();
                        Console.WriteLine(ver);
                        version = ver;
                    }
                }
            }
            catch (Exception ex)
            {
                //txtErr.Text = ex.ToString();
                throw ex;

            }
        }

        public void CreateTable()
        {
        
            using (var con = new SQLiteConnection(DataBaseSource))
            {
                con.Open();

                using (var cmd = new SQLiteCommand(con))
                {
                    cmd.CommandText = "DROP TABLE IF EXISTS cars";
                    cmd.ExecuteNonQuery();

                    cmd.CommandText = @"CREATE TABLE cars(id INTEGER PRIMARY KEY,
                    name TEXT, price INT)";
                    cmd.ExecuteNonQuery();

                    cmd.CommandText = "INSERT INTO cars(name, price) VALUES('Audi',52642)";
                    cmd.ExecuteNonQuery();

                    cmd.CommandText = "INSERT INTO cars(name, price) VALUES('Mercedes',57127)";
                    cmd.ExecuteNonQuery();

                    cmd.CommandText = "INSERT INTO cars(name, price) VALUES('Skoda',9000)";
                    cmd.ExecuteNonQuery();

                    cmd.CommandText = "INSERT INTO cars(name, price) VALUES('Volvo',29000)";
                    cmd.ExecuteNonQuery();

                    cmd.CommandText = "INSERT INTO cars(name, price) VALUES('Bentley',350000)";
                    cmd.ExecuteNonQuery();

                    cmd.CommandText = "INSERT INTO cars(name, price) VALUES('Citroen',21000)";
                    cmd.ExecuteNonQuery();

                    cmd.CommandText = "INSERT INTO cars(name, price) VALUES('Hummer',41400)";
                    cmd.ExecuteNonQuery();

                    cmd.CommandText = "INSERT INTO cars(name, price) VALUES('Volkswagen',21600)";
                    cmd.ExecuteNonQuery();
                }
                Console.WriteLine("Table cars created");
            }
        }

        public void InsertTable()
        {
            using(var con = new SQLiteConnection(this.DataBaseSource))
            {
                con.Open();
                var cmd = new SQLiteCommand(con);
                cmd.CommandText = "INSERT INTO cars(name, price) VALUES(@name, @price)";

                cmd.Parameters.AddWithValue("@name", "BMW");
                cmd.Parameters.AddWithValue("@price", 36600);
                cmd.Prepare();

                cmd.ExecuteNonQuery();

                Console.WriteLine("row inserted");
            }            
        }

        public void UpdateTable(String name, Int64 price)
        {
            using (var con = new SQLiteConnection(this.DataBaseSource))
            {
                con.Open();
                var cmd = new SQLiteCommand(con);
                cmd.CommandText = "Update cars set price = @price where name = @name";

                cmd.Parameters.AddWithValue("@name", name);
                cmd.Parameters.AddWithValue("@price", price);
                cmd.Prepare();

                cmd.ExecuteNonQuery();

                Console.WriteLine("row inserted");
            }
        }

        public DataTable ReadItems()
        {
            DataTable dt = new DataTable();
            try
            {
                using (var con = new SQLiteConnection(this.DataBaseSource))
                {
                    con.Open();
                    string sql = "SELECT * FROM cars LIMIT 5";
                    using (SQLiteCommand mycommand = new SQLiteCommand(con))
                    {
                        mycommand.CommandText = sql;
                        using (SQLiteDataReader reader = mycommand.ExecuteReader())
                        {
                            dt.Load(reader);
                            while (reader.Read())
                            {
                                logger.WriteLine($"{reader.GetInt32(0)} {reader.GetString(1)} {reader.GetInt32(2)}");
                            }
                            reader.Close();
                        }
                    }
                    con.Close();
                }
            }
            catch( Exception e)
            {
                logger.WriteLine("ReadItems Method: " + e.Message);
            }
            return dt;
        }

        public DataSet ReadTable()
        {
            DataSet ds = new DataSet();
            try
            {
                using(SQLiteConnection con = new SQLiteConnection(this.DataBaseSource))
                {
                    //string sql = "Select ItemCode,ItemName,Supplier,LastUpdatedOn from Items;";
                    string sql = "Select * from Cars";
                    using (SQLiteCommand command = new SQLiteCommand(sql,con))
                     {
                        SQLiteDataAdapter da = new SQLiteDataAdapter(command);
                        da.Fill(ds);
                    }
                }
            }
            catch( Exception ex)
            {
                logger.WriteLine(ex.Message);
            }
            return ds;
        }

        public DataSet getDBs()
        {
            DataSet dbs = new DataSet();
            try
            {
                using(var con = new SQLiteConnection(this.DataBaseSource))
                {
                    string sql = "SELECT name FROM sqlite_master WHERE type = 'table' AND name NOT LIKE 'sqlite_%'; ";
                    using(var command = new SQLiteCommand(sql, con))
                    {
                        SQLiteDataAdapter da = new SQLiteDataAdapter(command);
                        da.Fill(dbs);
                    }
                }
            }
            catch(Exception ex)
            {
                logger.WriteLine(ex.Message);
                throw ex;
            }
            return dbs;
        }
    }
    
}