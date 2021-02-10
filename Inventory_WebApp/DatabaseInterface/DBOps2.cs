using System;
using System.Data;
using System.Data.SQLite;
using System.IO;
using System.Xml;
using log4net;

namespace Inventory_WebApp.DatabaseInterface
{
    /// <summary>
    /// Class of SQLite DB manipulation functions written with reference from:
    /// http://zetcode.com/csharp/sqlite/
    /// https://docs.microsoft.com/en-us/dotnet/csharp/programming-guide/classes-and-structs/constructors
    /// https://support.microsoft.com/en-us/help/307548/how-to-read-xml-from-a-file-by-using-visual-c
    /// </summary>
    public class DBOps
    {

        /// <summary>
        /// Private variables Section
        /// 1. DatabaseSource: a path to the sqlite .db file used in the application.
        /// 2. _config: a handle to a config file (Not used at the moment)
        /// 3. _loggerpath: a path to a log file that we can write database exceptions and errors to. File must exist and have the right permissions or else this line will fail and crash the whole app.
        /// </summary>
        private string DataBaseSource = "Data Source=" + AppDomain.CurrentDomain.BaseDirectory + "sample_inventory.db";  //C:\\Users\\sanketm\\Documents\\ETS_Inventory\\sample_inventory.db";
        private XmlTextReader _config;
        private string _loggerpath = AppDomain.CurrentDomain.BaseDirectory +"./inventory_db_exceptions.log"; // READ from Config File new StreamWriter("C:\\Users\\sanketm\\Documents\\ETS_Inventory\\Log\\log.log");
        private static readonly log4net.ILog log = log4net.LogManager.GetLogger(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType);

        /// <summary>
        /// GetDBs(): A function to get all existing tables from the sqlite database.
        /// It ignores sqlite system tables.
        /// Returns: DataSet
        /// </summary>
        /// <returns></returns>
        public DataSet GetDBs()
        {
            DataSet dbs = new DataSet();
            try
            {
                using (var con = new SQLiteConnection(this.DataBaseSource))
                {
                    string sql = "SELECT name FROM sqlite_master WHERE type = 'table' AND name NOT LIKE 'sqlite_%'; ";
                    using (var command = new SQLiteCommand(sql, con))
                    {
                        SQLiteDataAdapter da = new SQLiteDataAdapter(command);
                        da.Fill(dbs);
                    }
                }
            }
            catch (Exception ex)
            {
                using (StreamWriter logger = new StreamWriter(this._loggerpath))
                {
                    logger.WriteAsync("DBOps.GetDBs: " + ex.Message);
                }

                /*throw*/;
            }
            return dbs;
        }

        /// <summary>
        /// CustomValidation: A helper function designed to clean/sanitize the sql command issued from the web-application.
        /// Removes SQL special characters which may change the query or be used to unintentionally harm the database.
        /// may break math like addition or modulo, so perform that stuff in the web-app logic, not in the SQL.
        /// </summary>
        /// <param name="command"></param>
        /// <returns></returns>
        string CustomValidation(string command)
        {
            string temp = command.ToLower();
            temp = temp.Replace("--", "");     //avoids commenting the rest of the query out
            temp = temp.Replace("+", "");
            temp = temp.Replace("||", "");
            temp = temp.Replace("concat", "");
            temp = temp.Replace("%", "");
            temp = temp.Replace("", "");
            return temp;
        }

        #region Constructor and Destructor
        public DBOps()
        {
            //InitLogger();
            //InitConfig();
            string path;
            XmlDocument conf = new XmlDocument();
            //conf.Load("./properties.xml");
            XmlNodeList nodeList = (conf.SelectNodes("properties"));

            foreach (XmlNode elem in nodeList)
            {
                path = elem.ChildNodes[0].InnerText;
                if (path != null || path != "")
                {
                    this.DataBaseSource = "data source=" + conf.GetElementsByTagName("datasource")[0].ChildNodes[0].Value;
                    this._loggerpath = conf.GetElementsByTagName("logfile")[0].ChildNodes[0].Value;
                }
            }


        }

        ~DBOps()
        {
            //this.logger.Close();
            //this._config.Close();
            this.DataBaseSource = null;
            this._loggerpath = null;
        }
        #endregion

        #region Inventory DB Interface
        public int InsertUpdateInventoryTable(string item,string itemCode, Int64 quantity,string lab, string category, string description,Int64 warnQuantity, Int64 alertQuantity)
        {
            int retval = 0;
            int insertQueryretval = 0;
            try
            {
                using (var con = new SQLiteConnection(this.DataBaseSource))
                {
                    con.Open();
                    var cmd = new SQLiteCommand(con);
                    cmd.CommandText = @"update inventory set 

                    itemCode = @itemcode,
                    quantity = @quantity,
                    lab = @lab,
                    description = @description,
                    model = @model,
                    category = @category,
                    alert_quantity = @alertquantity,
                    warning_quantity = @warnquantity
                    where
                        inventory.itemCode = @itemcode
                        and inventory.lab = @lab
                        and inventory.model = @model
                     ";

                    cmd.Parameters.AddWithValue("@itemcode", item);
                    cmd.Parameters.AddWithValue("@model", itemCode);
                    cmd.Parameters.AddWithValue("@quantity", quantity);
                    cmd.Parameters.AddWithValue("@lab", lab);
                    cmd.Parameters.AddWithValue("@category", category);
                    cmd.Parameters.AddWithValue("@description", description);
                    cmd.Parameters.AddWithValue("@alertquantity", alertQuantity);
                    cmd.Parameters.AddWithValue("@warnquantity", warnQuantity);
                    cmd.Prepare();

                    retval = cmd.ExecuteNonQuery();
                    if (retval > 0)
                    {
                        //this.logger.WriteAsync("row updated");
                    }
                    else
                    {
                        //this.logger.WriteAsync("No row updated");
                        //Need to insert instead
                        cmd.CommandText = "INSERT INTO inventory(itemcode, model, quantity,lab, category, description,alert_quantity,warning_quantity) VALUES(@itemcode, @model, @quantity, @lab, @category, @description, @alertquantity, @warningquantity)";
                        cmd.Parameters.AddWithValue("@itemcode", item);
                        cmd.Parameters.AddWithValue("@model", itemCode);
                        cmd.Parameters.AddWithValue("@quantity", quantity);
                        cmd.Parameters.AddWithValue("@lab", lab);
                        cmd.Parameters.AddWithValue("@category", category);
                        cmd.Parameters.AddWithValue("@description", description);
                        cmd.Parameters.AddWithValue("@alertquantity", alertQuantity);
                        cmd.Parameters.AddWithValue("@warnquantity", warnQuantity);
                        cmd.Prepare();

                        insertQueryretval = cmd.ExecuteNonQuery();

                    }
                }
            }
            catch (Exception ex)
            {
                using (StreamWriter logger = new StreamWriter(this._loggerpath))
                {
                    logger.WriteAsync("DBOps.InsertInventoryTable: " + ex.Message);
                }
                //throw;
            }
            return (retval == 0)?insertQueryretval:retval;
        }

        public int InsertInventoryTable(string item, string itemCode, Int64 quantity, string lab, string category, string description)
        {
            int retval = 0;
            int insertQueryretval = 0;
            try
            {
                using (var con = new SQLiteConnection(this.DataBaseSource))
                {
                    con.Open();
                    var cmd = new SQLiteCommand(con);
                    cmd.CommandText = @"update inventory set 

                    itemCode = @itemcode,
                    quantity = @quantity,
                    lab = @lab,
                    description = @description,
                    model = @model,
                    category = @category
                    where
                        inventory.itemCode = @itemcode
                        and inventory.lab = @lab
                        and inventory.model = @model
                     ";

                    cmd.Parameters.AddWithValue("@itemcode", item);
                    cmd.Parameters.AddWithValue("@model", itemCode);
                    cmd.Parameters.AddWithValue("@quantity", quantity);
                    cmd.Parameters.AddWithValue("@lab", lab);
                    cmd.Parameters.AddWithValue("@category", category);
                    cmd.Parameters.AddWithValue("@description", description);
                    cmd.Prepare();

                    retval = cmd.ExecuteNonQuery();
                    if (retval > 0)
                    {
                        //this.logger.WriteAsync("row updated");
                    }
                    else
                    {
                        //this.logger.WriteAsync("No row updated");
                        //Need to insert instead
                        cmd.CommandText = "INSERT INTO inventory(itemcode, model, quantity,lab, category, description) VALUES(@itemcode, @model, @quantity, @lab, @category, @description)";
                        cmd.Parameters.AddWithValue("@itemcode", item);
                        cmd.Parameters.AddWithValue("@model", itemCode);
                        cmd.Parameters.AddWithValue("@quantity", quantity);
                        cmd.Parameters.AddWithValue("@lab", lab);
                        cmd.Parameters.AddWithValue("@category", category);
                        cmd.Parameters.AddWithValue("@description", description);
                        cmd.Prepare();

                        insertQueryretval = cmd.ExecuteNonQuery();

                    }
                }
            }
            catch (Exception ex)
            {
                using (StreamWriter logger = new StreamWriter(this._loggerpath))
                {
                    logger.WriteAsync("DBOps.InsertInventoryTable: " + ex.Message);
                }
                //throw;
            }
            return (retval == 0) ? insertQueryretval : retval;
        }

        public int UpdateInventoryTable(string itemcode, Int64 quantity,string lab)
        {
            int retval = 0;
            try
            {
                using (var con = new SQLiteConnection(this.DataBaseSource))
                {
                    con.Open();
                    var cmd = new SQLiteCommand(con);
                    cmd.CommandText = "update inventory set quantity = @quantity where itemcode = @itemcode and lab = @lab";

                    cmd.Parameters.AddWithValue("@itemcode", itemcode);
                    cmd.Parameters.AddWithValue("@quantity", quantity);
                    cmd.Parameters.AddWithValue("@lab", lab);
                    cmd.Prepare();

                    retval = cmd.ExecuteNonQuery();
                    if (retval <= 0)
                    {
                        //this.logger.WriteAsync("no row updated");
                    }
                    else
                    {
                        //this.logger.WriteAsync(retval + " Rows updated");
                    }

                }
            }
            catch (Exception ex)
            {
                using (StreamWriter logger = new StreamWriter(this._loggerpath))
                {
                    logger.WriteAsync("DBOps.UpdateInventoryTable: " + ex.Message);
                }
                //this.logger.WriteAsync("Update Inventory Table: " + ex.Message);
                /*throw*/;
            }
            return retval;
        }


        public DataSet ReadInventoryTable()
        {
            DataSet ds = new DataSet();
            try
            {
                using (SQLiteConnection con = new SQLiteConnection(this.DataBaseSource))
                {
                    //string sql = "Select ItemCode,ItemName,Supplier,LastUpdatedOn from Items;";
                    string sql = "Select * from inventory";
                    using (SQLiteCommand command = new SQLiteCommand(sql, con))
                    {
                        SQLiteDataAdapter da = new SQLiteDataAdapter(command);
                        da.Fill(ds);
                    }
                }
            }
            catch (Exception ex)
            {
                using (StreamWriter logger = new StreamWriter(this._loggerpath))
                {
                    logger.WriteAsync("DBOps.ReadInventoryTable: " + ex.Message);
                }

                /*throw*/;
                //this.logger.WriteAsync(ex.Message);
            }
            return ds;
        }

        public DataSet GetInventoryColumns()
        {
            DataSet dbs = new DataSet();
            try
            {
                using (var con = new SQLiteConnection(this.DataBaseSource))
                {
                    string sql = "PRAGMA table_info(inventory)";
                    using (var command = new SQLiteCommand(sql, con))
                    {
                        SQLiteDataAdapter da = new SQLiteDataAdapter(command);
                        da.Fill(dbs);
                    }
                }
            }
            catch (Exception ex)
            {
                using (StreamWriter logger = new StreamWriter(this._loggerpath))
                {
                    logger.WriteAsync("DBOps.GetInventoryColumns: " + ex.Message);
                }

                /*throw*/;
            }
            return dbs;
        }

        public int DeleteInventoryTable(string itemcode, string lab)
        {
            int retval = 0;
            try
            {
                using (var con = new SQLiteConnection(this.DataBaseSource))
                {
                    con.Open();
                    var cmd = new SQLiteCommand(con);
                    cmd.CommandText = "delete from inventory where itemcode = @itemcode and lab = @lab";

                    cmd.Parameters.AddWithValue("@itemcode", itemcode);
                    cmd.Parameters.AddWithValue("@lab", lab);
                    cmd.Prepare();

                    retval = cmd.ExecuteNonQuery();
                    if (retval <= 0)
                    {
                        //this.logger.WriteAsync("no row updated");
                    }
                    else
                    {
                        //this.logger.WriteAsync(retval + " Rows updated");
                    }

                }
            }
            catch (Exception ex)
            {
                using (StreamWriter logger = new StreamWriter(this._loggerpath))
                {
                    logger.WriteAsync("DBOps.DeleteInventoryTable: " + ex.Message);
                }

                /*throw*/;
            }
            return retval;
        }
        #endregion

        #region Items DB Interface
        public int InsertItemsTable(String key, Int64 value)
        {
            int retval = 0;
            try
            {
                using (var con = new SQLiteConnection(this.DataBaseSource))
                {
                    con.Open();
                    var cmd = new SQLiteCommand(con);
                    cmd.CommandText = "INSERT INTO Items(itemcode, quantity) VALUES(@itemcode, @quantity)";

                    cmd.Parameters.AddWithValue("@itemcode", key);
                    cmd.Parameters.AddWithValue("@quantity", value);
                    cmd.Prepare();

                    retval = cmd.ExecuteNonQuery();
                    if (retval != 0)
                    {
                        //this.logger.WriteAsync("row inserted");
                    }
                    else
                    {
                        //this.logger.WriteAsync("No row inserted");
                    }
                }
            }
            catch (Exception ex)
            {
                using (StreamWriter logger = new StreamWriter(this._loggerpath))
                {
                    logger.WriteAsync("DBOps.InsertItemsTable: " + ex.Message);
                }

                /*throw*/;
            }
            return retval;
        }

        public int UpdateItemsTable(String key, Int64 value)
        {
            int retval = 0;
            try
            {
                using (var con = new SQLiteConnection(this.DataBaseSource))
                {
                    con.Open();
                    var cmd = new SQLiteCommand(con);
                    cmd.CommandText = "update Items set quantity = @quantity where itemcode = @itemcode";

                    cmd.Parameters.AddWithValue("@itemcode", key);
                    cmd.Parameters.AddWithValue("@quantity", value);
                    cmd.Prepare();

                    retval = cmd.ExecuteNonQuery();
                    if (retval <= 0)
                    {
                        //this.logger.WriteAsync("no row updated");
                    }
                    else
                    {
                        //this.logger.WriteAsync(retval + " Rows updated");
                    }

                }
            }
            catch (Exception ex)
            {
                using (StreamWriter logger = new StreamWriter(this._loggerpath))
                {
                    logger.WriteAsync("DBOps.UpdateItemsTable: " + ex.Message);
                }

                /*throw*/;
            }
            return retval;
        }


        public DataSet ReadItemsTable()
        {
            DataSet ds = new DataSet();
            try
            {
                using (SQLiteConnection con = new SQLiteConnection(this.DataBaseSource))
                {
                    //string sql = "Select ItemCode,ItemName,Supplier,LastUpdatedOn from Items;";
                    string sql = "Select * from Items";
                    using (SQLiteCommand command = new SQLiteCommand(sql, con))
                    {
                        SQLiteDataAdapter da = new SQLiteDataAdapter(command);
                        da.Fill(ds);
                    }
                }
            }
            catch (Exception ex)
            {
                //using (StreamWriter logger = new StreamWriter(this._loggerpath))
                //{
                //    logger.WriteAsync("DBOps.ReadItemsTable: " + ex.Message);
                //}

                /*throw*/;
            }
            return ds;
        }

        public DataSet GetItems()
        {
            DataSet ds = new DataSet();
            try
            {
                using (SQLiteConnection con = new SQLiteConnection(this.DataBaseSource))
                {
                    string sql = "Select distinct itemcode from inventory";
                    using (SQLiteCommand command = new SQLiteCommand(sql, con))
                    {
                        SQLiteDataAdapter da = new SQLiteDataAdapter(command);
                        da.Fill(ds);
                    }
                }
            }
            catch (Exception ex)
            {
                using (StreamWriter logger = new StreamWriter(this._loggerpath))
                {
                    logger.WriteAsync("DBOps.GetItems: " + ex.Message);
                }

                /*throw*/;
            }
            return ds;
        }
        #endregion

        #region Supplier DB Interface
        public int InsertSupplierTable(String key, String value, Int64 contactno)
        {
            int retval = 0;
            try
            {
                using (var con = new SQLiteConnection(this.DataBaseSource))
                {
                    con.Open();
                    var cmd = new SQLiteCommand(con);
                    cmd.CommandText = "INSERT INTO Supplier(SupCode, SupName,SupContactNo1) VALUES(@supcode, @supname,@supcontactno)";

                    cmd.Parameters.AddWithValue("@supcode", key);
                    cmd.Parameters.AddWithValue("@supname", value);
                    cmd.Parameters.AddWithValue("@supcontactno", contactno);
                    cmd.Prepare();

                    retval = cmd.ExecuteNonQuery();
                    if (retval != 0)
                    {
                        //this.logger.WriteAsync("row inserted");
                    }
                    else
                    {
                        //this.logger.WriteAsync("No row inserted");
                    }
                }
            }
            catch (Exception ex)
            {
                using (StreamWriter logger = new StreamWriter(this._loggerpath))
                {
                    logger.WriteAsync("DBOps.InsertSupplierTable: " + ex.Message);
                }

                /*throw*/;
            }
            return retval;
        }

        public int UpdateSupplierTable(String key, Int64 value)
        {
            int retval = 0;
            try
            {
                using (var con = new SQLiteConnection(this.DataBaseSource))
                {
                    con.Open();
                    var cmd = new SQLiteCommand(con);
                    cmd.CommandText = "update Supplier set quantity = @quantity where itemcode = @itemcode";

                    cmd.Parameters.AddWithValue("@itemcode", key);
                    cmd.Parameters.AddWithValue("@quantity", value);
                    cmd.Prepare();

                    retval = cmd.ExecuteNonQuery();
                    if (retval <= 0)
                    {
                        //this.logger.WriteAsync("no row updated");
                    }
                    else
                    {
                        //this.logger.WriteAsync(retval + " Rows updated");
                    }

                }
            }
            catch (Exception ex)
            {
                using (StreamWriter logger = new StreamWriter(this._loggerpath))
                {
                    logger.WriteAsync("DBOps.UpdateSupplierTable: " + ex.Message);
                }

                /*throw*/;
            }
            return retval;
        }


        public DataSet ReadSupplierTable()
        {
            DataSet ds = new DataSet();
            try
            {
                using (SQLiteConnection con = new SQLiteConnection(this.DataBaseSource))
                {
                    //string sql = "Select ItemCode,ItemName,Supplier,LastUpdatedOn from Supplier;";
                    string sql = "Select * from Supplier";
                    using (SQLiteCommand command = new SQLiteCommand(sql, con))
                    {
                        SQLiteDataAdapter da = new SQLiteDataAdapter(command);
                        da.Fill(ds);
                    }
                }
            }
            catch (Exception ex)
            {
                using (StreamWriter logger = new StreamWriter(this._loggerpath))
                {
                    logger.WriteAsync("DBOps.ReadSupplierTable: " + ex.Message);
                }

                /*throw*/;
            }
            return ds;
        }
        #endregion

        #region Labs DB Interface

        public DataSet ReadLabsTable()
        {
            DataSet ds = new DataSet();
            try
            {
                using (SQLiteConnection con = new SQLiteConnection(this.DataBaseSource))
                {
                    //string sql = "Select ItemCode,ItemName,Supplier,LastUpdatedOn from Supplier;";
                    string sql = "Select * from Labs";
                    using (SQLiteCommand command = new SQLiteCommand(sql, con))
                    {
                        SQLiteDataAdapter da = new SQLiteDataAdapter(command);
                        da.Fill(ds);
                    }
                }
            }
            catch (Exception ex)
            {
                using (StreamWriter logger = new StreamWriter(this._loggerpath))
                {
                    logger.WriteAsync("DBOps.ReadLabsTable: " + ex.Message);
                }

                /*throw*/;
            }
            return ds;
        }

        public int InsertLabsTable(string labname, string building, string roomno)
        {
            int retval = 0;
            try
            {
                using (var conn = new SQLiteConnection(this.DataBaseSource))
                {
                    conn.Open();
                    string sqlstring = "Insert into labs(name,building,roomno) values (@lab,@bldg,@roomno)";
                    using (var command = new SQLiteCommand(sqlstring, conn))
                    {
                        command.Parameters.AddWithValue("@lab", labname);
                        command.Parameters.AddWithValue("@bldg", building);
                        command.Parameters.AddWithValue("@roomno", roomno);

                        command.Prepare();

                        retval = command.ExecuteNonQuery();
                        if (retval != 0)
                        {
                            //this.logger.WriteAsync("row inserted");
                        }
                        else
                        {
                            //this.logger.WriteAsync("No row inserted");
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                using (StreamWriter logger = new StreamWriter(this._loggerpath))
                {
                    logger.WriteAsync("DBOps.InsertLabsTable: " + ex.Message);
                }

                /*throw*/;
            }
            return retval;
        }

        public int UpdateLabsTable(string labname, string building, string roomno)
        {
            int retval = 0;
            try
            {
                using (var conn = new SQLiteConnection(this.DataBaseSource))
                {
                    conn.Open();
                    string sqlstring = "update labs set building = @bldg,roomno = @roomno where name = @lab ";
                    using (var command = new SQLiteCommand(sqlstring, conn))
                    {
                        command.Parameters.AddWithValue("@lab", labname);
                        command.Parameters.AddWithValue("@bldg", building);
                        command.Parameters.AddWithValue("@roomno", roomno);

                        command.Prepare();

                        retval = command.ExecuteNonQuery();
                        if (retval != 0)
                        {
                            //this.logger.WriteAsync("row updated");
                        }
                        else
                        {
                            //this.logger.WriteAsync("No row updated");
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                using (StreamWriter logger = new StreamWriter(this._loggerpath))
                {
                    logger.WriteAsync("DBOps.UpdateLabsTable: " + ex.Message);
                }

                /*throw*/;
            }
            return retval;
        }

        public DataSet GetLabColumns()
        {
            DataSet dbs = new DataSet();
            try
            {
                using (var con = new SQLiteConnection(this.DataBaseSource))
                {
                    string sql = "PRAGMA table_info(labs)";
                    using (var command = new SQLiteCommand(sql, con))
                    {
                        SQLiteDataAdapter da = new SQLiteDataAdapter(command);
                        da.Fill(dbs);
                    }
                }
            }
            catch (Exception ex)
            {
                //using (StreamWriter logger = new StreamWriter(this._loggerpath))
                //{
                //    logger.WriteAsync("DBOps.GetLabColumns: " + ex.Message);
                //}

                /*throw*/;
            }
            return dbs;
        }

        public DataSet GetLabs()
        {
            DataSet ds = new DataSet();
            try
            {
                using (SQLiteConnection con = new SQLiteConnection(this.DataBaseSource))
                {
                    //string sql = "Select ItemCode,ItemName,Supplier,LastUpdatedOn from Supplier;";
                    string sql = "Select distinct name from Labs";
                    using (SQLiteCommand command = new SQLiteCommand(sql, con))
                    {
                        SQLiteDataAdapter da = new SQLiteDataAdapter(command);
                        da.Fill(ds);
                    }
                }
            }
            catch(Exception ex)
            {
                using (StreamWriter logger = new StreamWriter(this._loggerpath))
                {
                    logger.WriteAsync("DBOps.GetLabs: " + ex.Message);
                }

                /*throw*/;
            }
            return ds;
        }
        #endregion

        #region Category DB Interface
        public DataSet GetCategories()
        {
            DataSet ds = new DataSet();
            try
            {
                using (SQLiteConnection con = new SQLiteConnection(this.DataBaseSource))
                {
                    //string sql = "Select ItemCode,ItemName,Supplier,LastUpdatedOn from Supplier;";
                    string sql = "Select distinct category from Inventory";
                    using (SQLiteCommand command = new SQLiteCommand(sql, con))
                    {
                        SQLiteDataAdapter da = new SQLiteDataAdapter(command);
                        da.Fill(ds);
                    }
                }
            }
            catch (Exception ex)
            {
                using (StreamWriter logger = new StreamWriter(this._loggerpath))
                {
                    logger.WriteAsync("DBOps.GetCategories: " + ex.Message);
                }

                /*throw*/;
            }
            return ds;
        }
        #endregion
        

        //TODO: DEVELOP THE BELOW FUNCTIONS - RIGHT NOW  WE'RE DOING THIS MANUALLY
        //public void InitLogger()
        //{
        //    string logFile = "C:\\Users\\sanketm\\Documents\\ETS_Inventory\\Log\\log_" + DateTime.Now.ToFileTimeUtc().ToString() + ".txt";

        //    try
        //    {
        //        // Open the file
        //        this.logger = new StreamWriter(logFile);
        //        this.logger.WriteAsync("Logger initialized on " + DateTime.Now.ToString());
        //    }
        //    catch (Exception ex)
        //    {
        //        if
        //        (
        //            ex is UnauthorizedAccessException
        //            || ex is ArgumentNullException
        //            || ex is PathTooLongException
        //            || ex is DirectoryNotFoundException
        //            || ex is NotSupportedException
        //            || ex is ArgumentException
        //            || ex is SecurityException
        //            || ex is IOException
        //        )
        //        {
        //            throw new Exception("Failed to create log file: " + ex.Message);
        //        }
        //        else
        //        {
        //            // Unexpected failure
        //            throw;
        //        }
        //    }
        //}

        //public void InitConfig()
        //{
        //    this._config = new XmlTextReader("C:\\Users\\sanketm\\source\\repos\\Inventory_WebApp\\Inventory_WebApp\\properties.xml");
        //    while (_config.Read())
        //    {
        //        switch (_config.NodeType)
        //        {
        //            case XmlNodeType.Element: // The node is an element.
        //                this.logger.Write("<" + _config.Name);
        //                this.logger.WriteAsync(">");
        //                break;
        //            case XmlNodeType.Text: //Display the text in each element.
        //                this.logger.WriteAsync(_config.Value);
        //                break;
        //            case XmlNodeType.EndElement: //Display the end of the element.
        //                this.logger.Write("</" + _config.Name);
        //                this.logger.WriteAsync(">");
        //                break;
        //        }
        //    }
        //    //Console.ReadLine();   
        //}

        //TODO: REMOVE DEPRECATED AND UNUSED CODE FUNCTIONS
        //public void CheckVersion()
        //{
        //    string version;
        //    try
        //    {

        //        string sqlcmd = "SELECT SQLITE_VERSION();";

        //        using (var con = new SQLiteConnection(DataBaseSource))
        //        {
        //            con.Open();
        //            using (var cmd = new SQLiteCommand(sqlcmd, con))
        //            {
        //                string ver = cmd.ExecuteScalar().ToString();
        //                //this.logger.WriteAsync("SQLite Version : "+ ver);
        //                version = ver;
        //            }
        //        }
        //    }
        //    catch (Exception ex)
        //    {
        //        throw ex;
        //        //this.logger.WriteAsync(ex.Message);
        //    }
        //}

        //public DataTable ReadItems()
        //{
        //    DataTable dt = new DataTable();
        //    try
        //    {
        //        using (var con = new SQLiteConnection(this.DataBaseSource))
        //        {
        //            con.Open();
        //            string sql = "SELECT * FROM cars LIMIT 5";
        //            using (SQLiteCommand mycommand = new SQLiteCommand(con))
        //            {
        //                mycommand.CommandText = sql;
        //                using (SQLiteDataReader reader = mycommand.ExecuteReader())
        //                {
        //                    dt.Load(reader);
        //                    while (reader.Read())
        //                    {
        //                        this.logger.WriteAsync($"{reader.GetInt32(0)} {reader.GetString(1)} {reader.GetInt32(2)}");
        //                    }
        //                    reader.Close();
        //                }
        //            }
        //            con.Close();
        //        }
        //    }
        //    catch( Exception e)
        //    {
        //          throw ex;
        //        this.logger.WriteAsync("ReadItems Method: " + e.Message);
        //    }
        //    return dt;
        //}

        //public void CreateTable()
        //{

        //    using (var con = new SQLiteConnection(DataBaseSource))
        //    {
        //        con.Open();

        //        using (var cmd = new SQLiteCommand(con))
        //        {
        //            cmd.CommandText = "DROP TABLE IF EXISTS cars";
        //            cmd.ExecuteNonQuery();

        //            cmd.CommandText = @"CREATE TABLE cars(id INTEGER PRIMARY KEY,
        //            name TEXT, price INT)";
        //            cmd.ExecuteNonQuery();

        //            cmd.CommandText = "INSERT INTO cars(name, price) VALUES('Audi',52642)";
        //            cmd.ExecuteNonQuery();

        //            cmd.CommandText = "INSERT INTO cars(name, price) VALUES('Mercedes',57127)";
        //            cmd.ExecuteNonQuery();

        //            cmd.CommandText = "INSERT INTO cars(name, price) VALUES('Skoda',9000)";
        //            cmd.ExecuteNonQuery();

        //            cmd.CommandText = "INSERT INTO cars(name, price) VALUES('Volvo',29000)";
        //            cmd.ExecuteNonQuery();

        //            cmd.CommandText = "INSERT INTO cars(name, price) VALUES('Bentley',350000)";
        //            cmd.ExecuteNonQuery();

        //            cmd.CommandText = "INSERT INTO cars(name, price) VALUES('Citroen',21000)";
        //            cmd.ExecuteNonQuery();

        //            cmd.CommandText = "INSERT INTO cars(name, price) VALUES('Hummer',41400)";
        //            cmd.ExecuteNonQuery();

        //            cmd.CommandText = "INSERT INTO cars(name, price) VALUES('Volkswagen',21600)";
        //            cmd.ExecuteNonQuery();
        //        }
        //        //this.logger.WriteAsync("Table cars created");
        //    }
        //}
    }


}