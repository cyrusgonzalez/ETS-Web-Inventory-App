using System;
using System.Data;
using System.Web.UI.WebControls;
using Inventory_WebApp.DatabaseInterface;

namespace Inventory_WebApp.Pages
{
    public partial class SuppliersPage : System.Web.UI.Page
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            // first case security to not bypass login
            if (Session["username"] == null) { Response.Redirect("Login.aspx"); }

            //First time page load commands to go inside this if block
            if (!IsPostBack)
            {
                try
                {
                    DBOps db = new DBOps();
                }
                catch (Exception ex)
                {
                    lblErr.Text = ex.ToString();
                    lblErr.DataBind();
                    throw ex;

                }
            }
            RefreshTable();


        }

        protected void btnLoad_Click(object sender, EventArgs e)
        {
            DBOps db = new DBOps();
            RefreshTable();
        }


        protected void RefreshTable()
        {
            DBOps db = new DBOps();

            DataSet ds = db.ReadSupplierTable();
            dgitem.DataSource = ds;
            dgitem.DataBind();

        }

        protected void btnInsert_Click(object sender, EventArgs e)
        {
            DBOps db = new DBOps();

            string _key = txtInsertKey.Text;
            string _value = (txtInsertValue.Text);
            long contactno = long.Parse(txtContactNo.Text);

            int retval = db.InsertSupplierTable(_key, _value,contactno);
            lblInsertInfo.Text = retval.ToString() + " rows inserted";
            lblInsertInfo.DataBind();

            RefreshTable();
        }

        protected void btnUpdate_Click(object sender, EventArgs e)
        {
            DBOps db = new DBOps();

            string _key = txtKey.Text;
            long _value = long.Parse(txtValue.Text);

            int retval = db.UpdateInventoryTable(_key, _value,"");
            lblUpdateInfo.Text = retval.ToString() + " rows updated";
            lblUpdateInfo.DataBind();

            RefreshTable();
        }

        protected void dgitem_PageIndexChanged(object source, DataGridPageChangedEventArgs e)
        {
            dgitem.CurrentPageIndex = e.NewPageIndex;
            RefreshTable();
        }

        protected void Clk_Logout_Click(object sender, EventArgs e)
        {
            Session.Abandon();
            Response.Redirect("Login.aspx");
        }
    }
}