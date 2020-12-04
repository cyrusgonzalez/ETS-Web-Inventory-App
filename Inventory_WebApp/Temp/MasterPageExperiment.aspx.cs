using System;
using System.Data;
using Inventory_WebApp.DatabaseInterface;

namespace Inventory_WebApp.Temp
{
    public partial class WebForm3 : System.Web.UI.Page
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            var db = new DBOps();
            DataSet dbs = db.GetDBs();
            ddlDB.DataSource = dbs;
            ddlDB.DataBind();
        }

        protected void btnLoad_Click(object sender, EventArgs e)
        {

        }
    }
}