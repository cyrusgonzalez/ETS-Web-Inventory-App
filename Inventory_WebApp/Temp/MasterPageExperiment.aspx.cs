using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;

namespace Inventory_WebApp
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