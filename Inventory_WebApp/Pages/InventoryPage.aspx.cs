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
    public partial class InventoryETS: System.Web.UI.Page
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            //First time page load commands to go inside this if block
            if (!IsPostBack)
            {
                try
                {
                    PopulateSearchColumnsDropdown();
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
            
            DataSet ds = db.ReadInventoryTable();
            dgitem.DataSource = ds;
            dgitem.DataBind();
                        
        }

        protected void PopulateSearchColumnsDropdown()
        {
            DBOps db = new DBOps();
            DataSet ds = db.getColumns();
            ddlColumn.DataSource = ds;
            ddlColumn.DataTextField = "name";
            ddlColumn.DataValueField = "name";
            ddlColumn.DataBind();
        }

        protected void btnInsert_Click(object sender, EventArgs e)
        {
            DBOps db = new DBOps();
            
            string _key = txtInsertKey.Text;
            long _value = long.Parse(txtInsertValue.Text);

            int retval = db.InsertInventoryTable(_key,_value);
            lblInsertInfo.Text = retval.ToString() + " row inserted";
            lblInsertInfo.DataBind();

            RefreshTable();
        }

        protected void btnUpdate_Click(object sender, EventArgs e)
        {
            DBOps db = new DBOps();
            
            string _key = txtKey.Text;
            long _value = long.Parse(txtValue.Text);

            int retval = db.UpdateInventoryTable(_key,_value);
            lblUpdateInfo.Text = retval.ToString() + " row updated";
            lblUpdateInfo.DataBind();

            RefreshTable();
        }

        protected void dgitem_PageIndexChanged(object source, DataGridPageChangedEventArgs e)
        {
            dgitem.CurrentPageIndex = e.NewPageIndex;
            RefreshTable();
        }

        protected void btnSearch_Click(object sender, EventArgs e)
        {
            string col = ddlColumn.SelectedValue;
            string type = ddlColumn.DataSource['type'];
            //DataView dv = (DataView) dgitem.DataSource;
            DataSet ds = (DataSet)dgitem.DataSource;
            DataTable dt = ds.Tables["Table"];
            var query = from row in dt.AsEnumerable()
                           where row.Field<string>(col) == txtSearchtext.Text
                           select row;
            DataTable result = query.CopyToDataTable();

            int count = result.Rows.Count;
            lblSearchInfo.Text = count.ToString() + " row(s) matched";
            lblSearchInfo.DataBind();

            dgSearchResult.DataSource = result;
            dgSearchResult.DataBind();
        }

        protected void dgitem_SortCommand(object source, DataGridSortCommandEventArgs e)
        {
            string sortby = e.SortExpression;
        }
    }
}