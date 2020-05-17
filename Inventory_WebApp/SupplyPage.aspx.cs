using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Data.SQLite;
using System.Data;


namespace Inventory_WebApp
{
    public partial class WebForm1 : System.Web.UI.Page
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            string DataBaseSource = "Data Source=C:\\Users\\sanketm\\Documents\\ETS_Inventory\\sample_inventory.db";
            //First time page load commands to go inside this if block
            if (!IsPostBack)
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
                    txtErr.Text = ex.ToString();
                    throw ex;

                }
            }
            

        }
        
        protected void Button1_Click(object sender, EventArgs e)
        {
            DBOps db = new DBOps();
            db.CheckVersion();
            db.CreateTable();
        }
    }
}