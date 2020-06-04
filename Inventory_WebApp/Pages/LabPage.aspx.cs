using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;

namespace Inventory_WebApp.Pages
{
    public partial class LabPage : System.Web.UI.Page
    {
        DataSet searchColumns = new DBOps().getLabColumns();
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

            DataSet ds = db.ReadLabsTable();
            dgitem.DataSource = ds;
            dgitem.DataBind();

            gvitem.DataSource = ds;
            gvitem.DataBind();

        }

        protected void PopulateSearchColumnsDropdown()
        {
            DBOps db = new DBOps();
            DataSet ds = db.getLabColumns();
            ddlColumn.DataSource = ds;
            ddlColumn.DataTextField = "name";
            ddlColumn.DataValueField = "name";
            ddlColumn.DataBind();
        }

        protected void btnInsert_Click(object sender, EventArgs e)
        {
            DBOps db = new DBOps();

            string _key = txtInsertLab.Text;
            string bldg = txtInsertBuilding.Text;
            string _value = txtInsertRoom.Text;

            int retval = db.InsertLabsTable(_key,bldg, _value);
            lblInsertInfo.Text = retval.ToString() + " row inserted";
            lblInsertInfo.DataBind();

            RefreshTable();
        }

        protected void btnUpdate_Click(object sender, EventArgs e)
        {
            DBOps db = new DBOps();

            string _key = txtUpdateLab.Text;
            string _value = txtUpdateBuilding.Text;
            string room = txtUpdateRoom.Text;

            int retval = db.UpdateLabsTable(_key, _value, room);
            lblUpdateInfo.Text = retval.ToString() + " row updated";
            lblUpdateInfo.DataBind();

            RefreshTable();
        }

        protected void dgitem_PageIndexChanged(object source, DataGridPageChangedEventArgs e)
        {
            dgitem.CurrentPageIndex = e.NewPageIndex;
            RefreshTable();
        }

        protected Type GetMyType(string SQLiteType)
        {
            Type returnType;
            try
            {
                switch (SQLiteType.ToLower())
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
            catch (Exception ex)
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

            DataSet ds = (DataSet)dgitem.DataSource;
            DataTable dt = ds.Tables["Table"];
            DataTable result;


            if (searchColumnType == typeof(int))
            {
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

        protected void dgitem_SortCommand(object source, DataGridSortCommandEventArgs e)
        {
            string sortby = e.SortExpression;
        }

        protected void gvitem_PageIndexChanged(object sender, EventArgs e)
        {

        }

        protected void gvitem_Sorted(object sender, EventArgs e)
        {

        }
    }
}