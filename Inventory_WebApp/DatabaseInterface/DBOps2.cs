using System;
using System.Data;
using System.Data.SQLite;
using System.IO;
using System.Web.UI.WebControls;
using System.Xml;
using log4net;
using System.Linq;

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
        /// Returns: DataSet containing one column of table names
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
        /// <summary>
        /// These aren't in use right now, and can be safely ignored.
        /// Use them if you want to do something everytime a DB Operation is required.
        /// Be sure to clean up any open file handlers and close files after the DB operation is complete in the ~DBOps destructor function
        /// </summary>

        public DBOps()
        {
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
        /// <summary>
        /// InsertUpdateInventoryTable(): A dual purpose combined DB method to update a row if it exists, else insert it into the Inventory Table of the DB
        /// </summary>
        /// <param name="item"></param>
        /// <param name="itemCode"></param>
        /// <param name="quantity"></param>
        /// <param name="lab"></param>
        /// <param name="category"></param>
        /// <param name="description"></param>
        /// <param name="warnQuantity"></param>
        /// <param name="alertQuantity"></param>
        /// <returns>
        /// retval/insertQueryVal :- an integer letting us know how many rows have been inserted/updated. Should always be 1. If two then there are duplicates in the Inventory table which must be deleted.
        /// </returns>
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
                        and inventory.category = @category
                        and description = @description
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
                        log.Info("Rows Inserted: " + retval.ToString() + " row(s) updated from method DBOps.InsertUpdateInventoryTable");
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

                        log.Info("Database updated " + retval.ToString() + " row updated from method DBOps.InsertUpdateInventoryTable");
                    }
                }
            }
            catch (Exception ex)
            {
                log.Error("Database Error in DBOps.InsertUpdateInventoryTable:  ", ex);
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
                log.Error("Database Error in DBOps.ReadInventoryTable:  ", ex);
            }
            log.Info("Database read " + ds.Tables[0].Rows.Count.ToString() + " rows at DBOps.ReadInventoryTable");
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
                log.Error("Database Error in DBOps.GetInventoryColumns:  ", ex);
            }
            string res = string.Join(Environment.NewLine, dbs.Tables[0].Rows.OfType<DataRow>().Select(x => string.Join(" ; ", x.ItemArray))); //https://stackoverflow.com/questions/1104121/how-to-convert-a-datatable-to-a-string-in-c
            log.Info("Database retrieved " + res + " Inventory columns from method DBOps.GetInventoryColumns");
            return dbs;
        }

        public int DeleteInventoryTable(string itemcode, string lab, string category, string model)
        {
            int retval = 0;
            try
            {
                using (var con = new SQLiteConnection(this.DataBaseSource))
                {
                    con.Open();
                    var cmd = new SQLiteCommand(con);

                    if((String.Equals(category, "")) && (String.Equals(model, ""))){
                        cmd.CommandText = "delete from inventory where itemcode = @itemcode and lab = @lab";

                        cmd.Parameters.AddWithValue("@itemcode", itemcode);
                        cmd.Parameters.AddWithValue("@lab", lab);
                        log.Info("DBOps.DeleteInventoryTable: Passed Only itemcode : " + itemcode + " and lab : " + lab);
                    }
                    else if (String.Equals(category, ""))
                    {
                        cmd.CommandText = "delete from inventory where itemcode = @itemcode and lab = @lab and model = @model";

                        cmd.Parameters.AddWithValue("@itemcode", itemcode);
                        cmd.Parameters.AddWithValue("@lab", lab);
                        cmd.Parameters.AddWithValue("@model", model);
                        log.Info("DBOps.DeleteInventoryTable: Passed itemcode: " + itemcode + " and lab: " + lab + " and model: " + model);
                    }
                    else if(String.Equals(model, ""))
                    {
                        cmd.CommandText = "delete from inventory where itemcode = @itemcode and lab = @lab and category = @category";

                        cmd.Parameters.AddWithValue("@itemcode", itemcode);
                        cmd.Parameters.AddWithValue("@lab", lab);
                        cmd.Parameters.AddWithValue("@category", category);
                        log.Info("DBOps.DeleteInventoryTable: Passed itemcode: " + itemcode + " and lab: " + lab + " and category: " + category);
                    }
                    else
                    {
                        cmd.CommandText = "delete from inventory where itemcode = @itemcode and lab = @lab and category = @category and model = @model";

                        cmd.Parameters.AddWithValue("@itemcode", itemcode);
                        cmd.Parameters.AddWithValue("@lab", lab);
                        cmd.Parameters.AddWithValue("@category", category);
                        cmd.Parameters.AddWithValue("@model", model);
                        log.Info("DBOps.DeleteInventoryTable: Passed itemcode: " + itemcode + " and lab: " + lab  + " and model: " + model + " and category: " + category);
                    }

                    cmd.Prepare();

                    retval = cmd.ExecuteNonQuery();
                    if (retval <= 0)
                    {
                        log.Info("DBOps.DeleteInventoryTable:  0 rows deleted");
                    }
                    else if (retval == 1)
                    {
                        log.Info("DBOps.DeleteInventoryTable:  1 rows deleted");
                    }
                    else
                    {
                        log.Error("DBOps.DeleteInventoryTable: " + retval.ToString() + " rows deleted");
                    }

                }
            }
            catch (Exception ex)
            {
                log.Error("Database Error in DBOps.DeleteInventoryTable:  ", ex);
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
        /// <summary>
        /// ReadLabsTable(): single purpose dp read function to get all records from the labs table in the DB
        /// </summary>
        /// <returns>
        /// a DataSet with one table, table[0], with all the rows of the labs table.
        /// </returns>
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
                log.Error("Database Error in DBOps.ReadLabsTable:  ", ex);
            }
            log.Info("Database Info " + ds.Tables[0].Rows.Count.ToString() + " rows read from method DBOps.ReadLabsTable");
            return ds;
        }

        /// <summary>
        /// InsertLabsTable(): single purpose db update function written to insert new labs into the DB. 
        /// </summary>
        /// <param name="labname"></param>
        /// <param name="building"></param>
        /// <param name="roomno"></param>
        /// <returns>
        /// retval: 1 - row inserted successfully, 0 unsuccessful insertion.
        /// </returns>
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
                        //if (retval != 0)
                        //{
                        //    //this.logger.WriteAsync("row inserted");
                        //}
                        //else
                        //{
                        //    //this.logger.WriteAsync("No row inserted");
                        //}
                    }
                }
            }
            catch (Exception ex)
            {
                log.Error("Database Error in DBOps.InsertLabsTable:  ", ex);
            }
            log.Info("Database updated " + retval.ToString() + " row inserted from method DBOps.InsertLabsTable");
            return retval;
        }

        /// <summary>
        /// UpdateLabsTable(): single ppurpose db update function written to update lab details in the Lab table in the DB.
        /// </summary>
        /// <param name="labname"></param>
        /// <param name="building"></param>
        /// <param name="roomno"></param>
        /// <returns>
        /// retval: number of rows updated with this query
        /// </returns>
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
                        //if (retval != 0)
                        //{
                        //    //this.logger.WriteAsync("row updated");
                        //    log.Info("DBOps.UpdateLabsTable:" + retval.ToString()  +" rows updated");
                        //}
                        //else
                        //{
                        //    //this.logger.WriteAsync("No row updated");
                        //    log.Info("Database Error in DBOps.UpdateLabsTable: 0 rows updated");
                        //}
                    }
                }
            }
            catch (Exception ex)
            {
                //using (StreamWriter logger = new StreamWriter(this._loggerpath))
                //{
                //    logger.WriteAsync("DBOps.UpdateLabsTable: " + ex.Message);
                //}
                log.Error("Database Error in DBOps.UpdateLabsTable:  ", ex);
            }
            log.Info("Database updated " + retval.ToString() + " rows from method DBOps.UpdateLabsTable");
            return retval;
        }

        /// <summary>
        /// GetLabColumns(): single purpose DB query function written to get column names from the labs table. Usable only in SQLite, not for SQL Server or MySQL
        /// </summary>
        /// <returns>
        /// Dataset with one table, which has rows with the neames of columns of the labs table
        /// </returns>
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

                log.Error("Database Error in DBOps.GetLabColumns:  ", ex);
            }
            string res = string.Join(Environment.NewLine, dbs.Tables[0].Rows.OfType<DataRow>().Select(x => string.Join(" ; ", x.ItemArray))); //https://stackoverflow.com/questions/1104121/how-to-convert-a-datatable-to-a-string-in-c
            log.Info("Database retrieved " + res + " lab columns from method DBOps.GetLabColumns");
            return dbs;
        }

        /// <summary>
        /// GetLabs(): A single purpose function written to query the Labs table in the DB to get stored values.
        /// </summary>
        /// <returns>
        /// DataSet with one table, table[0] with categories for rows
        /// </returns>
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
                //using (StreamWriter logger = new StreamWriter(this._loggerpath))
                //{
                //    logger.WriteAsync(" " + ex.Message);
                //}
                log.Error("Database Error in DBOps.GetLabs::  ", ex);
            }
            log.Info("Database retrieved " + ds.Tables[0].Rows.Count.ToString() + " labs from method DBOps.GetLabs");
            return ds;
        }
        #endregion

        #region Category DB Interface
        /// <summary>
        /// GetCategories() A single purpose function written queries the Inventory table to return unique categories in the DB.
        /// </summary>
        /// <returns>
        /// DataSet with one table, table[0] with categories for rows
        /// </returns>
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
                //using (StreamWriter logger = new StreamWriter(this._loggerpath))
                //{
                //    logger.WriteAsync("DBOps.GetCategories: " + ex.Message);
                //}
                log.Error("Database Error in DBOps.GetCategories:  ", ex);
            }
            
            log.Info("Database retrieved " + ds.Tables[0].Rows.Count.ToString() + " categories from method DBOps.GetConfig: ");
            return ds;
        }
        #endregion

        #region EMail Interfaces

        /// <summary>
        /// GetConfig() : A general purpose function written to query the appconfigs table in the DB to get stored values.
        /// </summary>
        /// <param name="configName"></param>
        /// <returns>
        /// A DataSet with one table, table[0] with the configs as rows and their values in a second column
        /// </returns>
        public DataSet GetConfig(string configName)
        {
            DataSet ds = new DataSet();
            try
            {
                using (SQLiteConnection con = new SQLiteConnection(this.DataBaseSource))
                {
                    con.Open();
                    string sql = "select * from appconfigs where configname = @configName";
                    SQLiteCommand command = new SQLiteCommand(con);
                    command.CommandText = sql;
                    command.Parameters.AddWithValue("@configName", configName);
                    command.Prepare();
                    SQLiteDataAdapter da = new SQLiteDataAdapter(command);
                    da.Fill(ds);
                }
            }
            catch (Exception ex)
            {
                log.Error("Database Error in DBOps.GetConfig:  ", ex);
            }

            log.Info("Database retrieved " + ds.Tables[0].Rows.Count.ToString() + " configs from method DBOps.GetConfig: ");
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