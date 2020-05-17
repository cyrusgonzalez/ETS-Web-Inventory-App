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
            //string DataBaseSource = "Data Source=C:\\Users\\sanketm\\Documents\\ETS_Inventory\\sample_inventory.db";
            //First time page load commands to go inside this if block
            if (!IsPostBack)
            {
                try
                {
                    DBOps db = new DBOps();
                    db.CheckVersion();
                }
                catch (Exception ex)
                {
                    lblErr.Text = ex.ToString();
                    throw ex;

                }
            }
            

        }
        
        protected void Button1_Click(object sender, EventArgs e)
        {
            DBOps db = new DBOps();
            db.CheckVersion();
            db.CreateTable();
            RefreshTable();
            //db.InsertTable();
            //db.UpdateTable();
            //db.ReadTable();
            //db.DeleteTable();
        }

        protected void RefreshTable()
        {
            DBOps db = new DBOps();
            //db.ReadTable();
            //
        }

        protected void btnInsert_Click(object sender, EventArgs e)
        {
            DBOps db = new DBOps();
            db.InsertTable();
        }

        protected void btnUpdate_Click(object sender, EventArgs e)
        {
            DBOps db = new DBOps();
            db.UpdateTable("BMW",1023300);
        }
    }
}