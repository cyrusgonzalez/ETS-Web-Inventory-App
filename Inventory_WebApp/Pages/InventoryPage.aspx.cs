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
        DataSet searchColumns = new DBOps().getInventoryColumns();
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
            gvitem.DataSource = ds;
            gvitem.DataBind();
                        
        }

        protected void PopulateSearchColumnsDropdown()
        {
            DBOps db = new DBOps();
            DataSet ds = db.getInventoryColumns();
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
            string _lab = "Lockheed Martin Magellan Lab and Design Studios";

            int retval = db.UpdateInventoryTable(_key,_value,_lab);
            lblUpdateInfo.Text = retval.ToString() + " row updated";
            lblUpdateInfo.DataBind();

            RefreshTable();
        }



        protected Type GetMyType(string SQLiteType)
        {
            Type returnType;
            try
            {
                switch (SQLiteType)
                {
                    case "integer":
                        returnType = typeof(int);
                        break;
                    case "string":
                        returnType = typeof(string);
                        break;
                    default:
                        returnType = typeof(string);
                        break;
                }
            }
            catch(Exception ex)
            {
                throw ex;
            }
            return returnType;
        }

        protected void btnSearch_Click(object sender, EventArgs e)
        {
            string col = ddlColumn.SelectedValue;
            var type = from d in searchColumns.Tables["Table"].AsEnumerable()
                          where d.Field<string>("name") == col
                          select d.Field<string>("type");

            Type searchColumnType = GetMyType(type.ToArray()[0]);

            DataSet ds = (DataSet)gvitem.DataSource;
            DataTable dt = ds.Tables["Table"];
            DataTable result;


            if (searchColumnType == typeof(int)){
                var query = from row in dt.AsEnumerable()
                            where row.Field<Int64>(col) == Int64.Parse(txtSearchtext.Text)
                            select row;
                result = query.CopyToDataTable();
            }
            else 
            {
                var query = from row in dt.AsEnumerable()
                            where row.Field<string>(col) == (txtSearchtext.Text)
                            select row;
                result = query.CopyToDataTable();
            }
            

            int count = result.Rows.Count;
            lblSearchInfo.Text = count.ToString() + " row(s) matched";
            lblSearchInfo.DataBind();

            dgSearchResult.DataSource = result;
            dgSearchResult.DataBind();
        }

       
        protected void gvitem_PageIndexChanging(object sender, GridViewPageEventArgs e)
        {
            gvitem.PageIndex = e.NewPageIndex;
            RefreshTable();
        }

        protected void gvitem_RowDataBound(object sender, GridViewRowEventArgs e)
        {
            //Add formatting code, if any
        }

        protected void gvitem_RowEditing(object sender, GridViewEditEventArgs e)
        {
            gvitem.EditIndex = e.NewEditIndex;
            RefreshTable();
        }

        protected void gvitem_RowCancelingEdit(object sender, GridViewCancelEditEventArgs e)
        {
            gvitem.EditIndex = -1;
            RefreshTable();
        }

        protected void gvitem_RowCreated(object sender, GridViewRowEventArgs e)
        {
            GridViewRow row = e.Row;

            TableCell cell = row.Cells[0];
            row.Cells.Remove(cell);
            row.Cells.Add(cell);
        }

        protected void gvitem_RowUpdating(object sender, GridViewUpdateEventArgs e)
        {

        }
    }
}