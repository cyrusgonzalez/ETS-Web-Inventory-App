using System;
using System.Data;
using Inventory_WebApp.DatabaseInterface;

namespace Inventory_WebApp.Pages
{
    public partial class WebForm2 : System.Web.UI.Page
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            // first case security to not bypass login
            if (Session["username"] == null) { Response.Redirect("Login.aspx"); }

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

        protected void Clk_Logout_Click(object sender, EventArgs e)
        {
            Session.Abandon();
            Response.Redirect("Login.aspx");
        }
    }
}