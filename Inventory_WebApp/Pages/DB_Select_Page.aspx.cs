using System;
using System.Data;
using Inventory_WebApp.DatabaseInterface;

namespace Inventory_WebApp.Pages
{
    public partial class WebForm2 : System.Web.UI.Page
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            var db = new DBOps();
            DataSet dbs = new DataSet("Databases");
            dbs = db.GetDBs();
            ddlDB.DataSource = dbs;
            ddlDB.DataValueField = "name";
            ddlDB.DataTextField = "name";
            ddlDB.DataBind();
        }

        protected void btnLoad_Click(object sender, EventArgs e)
        {
            string selecteddb = ddlDB.SelectedValue;

        }
    }
}