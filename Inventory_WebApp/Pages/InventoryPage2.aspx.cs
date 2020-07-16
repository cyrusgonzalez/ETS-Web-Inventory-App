using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Data.SQLite;
using System.Data;
using System.Diagnostics;

namespace Inventory_WebApp
{
    public partial class InventoryETS2 : System.Web.UI.Page
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
                    PopulateLabsDropdown();
                    RefreshTable();
                    gvitem.AllowSorting = true;
                }
                catch (Exception ex)
                {
                    lblErr.Text = ex.ToString();
                    lblErr.DataBind();
                    throw ex;
                }
            }
            else
            {
                lblInsertInfo.Text = "";
                lblPageInfo.Text = "";
                lblSearchInfo.Text = "";
                lblUpdateInfo.Text = "";
                //DataTable dt = (DataTable)dgSearchResult.DataSource;
                //if(dt.Rows.Count == 0)
                //{

                //}
            }
        }

        protected void btnLoad_Click(object sender, EventArgs e)
        {
            DBOps db = new DBOps();
            string callerfunc = new StackFrame(1).GetMethod().Name.Substring(4);
            RefreshTable(callerfunc);
        }

        protected void RefreshTable()
        {
            try
            {
                if (ddlLabselect.SelectedValue == "All")
                {
                    DBOps db = new DBOps();

                    DataSet ds = db.ReadInventoryTable();
                    gvitem.DataSource = ds;
                    gvitem.DataBind();
                    ViewState["gvitems"] = ds;
                }
                else
                {
                    DBOps db = new DBOps();
                    //DataSet ds = (DataSet)ViewState["gvitems"];    // Apparently grid view items are not stored persistently across postabacks so then either I have to hit the db again or use the viewstate i.e. gvitems.DataSource doesnt work here
                    DataSet ds = db.ReadInventoryTable();
                    DataTable dt = ds.Tables["Table"];
                    var query = from row in dt.AsEnumerable()
                                where row.Field<string>("lab") == (ddlLabselect.SelectedValue)
                                select row;
                    DataTable result = query.CopyToDataTable();

                    gvitem.DataSource = result;
                    gvitem.DataBind();
                }
            }
            catch (Exception ex)
            {
                if (ex.Message.Contains("no rows"))
                {
                    lblPageInfo.Text = "There are no items in this lab";
                    lblPageInfo.DataBind();
                }

            }

        }

        protected void RefreshTable(string callerfunc)
        {
            try
            {
                if (ddlLabselect.SelectedValue == "All")
                {
                    DBOps db = new DBOps();

                    DataSet ds = db.ReadInventoryTable();
                    gvitem.DataSource = ds;
                    gvitem.DataBind();
                    ViewState["gvitems"] = ds;
                }
                else
                {
                    DBOps db = new DBOps();
                    //DataSet ds = (DataSet)ViewState["gvitems"];    // Apparently grid view items are not stored persistently across postabacks so then either I have to hit the db again or use the viewstate i.e. gvitems.DataSource doesnt work here
                    DataSet ds = db.ReadInventoryTable();
                    DataTable dt = ds.Tables["Table"];
                    var query = from row in dt.AsEnumerable()
                                where row.Field<string>("lab") == (ddlLabselect.SelectedValue)
                                select row;
                    DataTable result = query.CopyToDataTable();

                    gvitem.DataSource = result;
                    gvitem.DataBind();
                }
            }
            catch (Exception ex)
            {
                if (ex.Message.Contains("no rows"))
                {
                    lblPageInfo.Text = "There are no items in this lab";
                }

            }
        }


        protected void PopulateSearchColumnsDropdown()
        {
            try
            {
                DBOps db = new DBOps();
                DataSet ds = db.getInventoryColumns();
                ddlColumn.DataSource = ds;
                ddlColumn.DataTextField = "name";
                ddlColumn.DataValueField = "name";
                ddlColumn.DataBind();
            }
            catch (Exception ex)
            {
                throw ex;
            }

        }

        protected void PopulateLabsDropdown()
        {
            try
            {
                DBOps db = new DBOps();
                DataSet ds = db.getLabs();
                DataRow dr = ds.Tables["table"].NewRow();
                dr["name"] = "All";
                ds.Tables["table"].Rows.InsertAt(dr, 0);
                ddlLabselect.DataSource = ds;
                ddlLabselect.DataTextField = "name";
                ddlLabselect.DataValueField = "name";
                ddlLabselect.DataBind();

                ddlInsertLab.DataSource = ds;
                ddlInsertLab.DataTextField = "name";
                ddlInsertLab.DataValueField = "name";
                ddlInsertLab.DataBind();

                //ddlSearchInventory.DataSource = ds;
                //ddlSearchInventory.DataTextField = "name";
                //ddlSearchInventory.DataValueField = "name";
                //ddlSearchInventory.DataBind();

            }
            catch (Exception ex)
            {
                throw ex;
            }

        }

        protected void ddlLabselect_SelectedIndexChanged(object sender, EventArgs e)
        {
            try
            {
                RefreshTable();
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        protected void btnInsert_Click(object sender, EventArgs e)
        {
            DBOps db = new DBOps();

            string _key = txtInsertItem.Text;
            long _value = long.Parse(txtInsertQuantity.Text);
            string lab = ddlInsertLab.SelectedValue;

            int retval = db.InsertInventoryTable(_key, _value, lab);
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

            int retval = db.UpdateInventoryTable(_key, _value, _lab);
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

            DBOps db = new DBOps();
            DataSet ds = db.ReadInventoryTable();
            //DataSet ds = (DataSet)gvitem.DataSource;
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
                            where row.Field<string>(col).Contains(txtSearchtext.Text)
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
            try
            {
                GridViewRow row = e.Row;

                //Code to move the Edit button to the right end of the table
                TableCell cell = row.Cells[0];
                row.Cells.Remove(cell);
                row.Cells.Add(cell);

                //code to move the delete button to the right end of the table after the edit button
                if (row.Cells.Count > 7)
                {
                    cell = row.Cells[8];
                    row.Cells.Remove(cell);
                    row.Cells.AddAt(row.Cells.Count, cell);
                }

            }
            catch (Exception ex)
            {
                throw ex;
            }



        }

        protected void gvitem_RowUpdating(object sender, GridViewUpdateEventArgs e)
        {
            //GridViewRow changedRow = gvitem.Rows[e.RowIndex];
            //string changedItem = changedRow.Cells[1].Text;
            //string changedLab = changedRow.Cells[6].Text;
            //Int32 changedQuantity = int.Parse(ViewState["changedQuantity"].ToString());//int.Parse((changedRow.FindControl("txtQuantity") as TextBox).Text);

            //try
            //{
            //    DBOps db = new DBOps();
            //    int retval = db.UpdateInventoryTable(changedItem, changedQuantity, changedLab);
            //    lblPageInfo.Text = "Row updated successfully.";
            //}
            //catch (Exception ex)
            //{
            //    lblPageInfo.Text = "An error occurred while attempting to update the row.";
            //    throw ex;
            //}
            //gvitem.EditIndex = -1;
            //RefreshTable();
        }

        protected void gvitem_RowUpdatingcustom(string changedItem, string changedLab, int changedQuantity)
        {

            try
            {
                DBOps db = new DBOps();
                int retval = db.UpdateInventoryTable(changedItem, changedQuantity, changedLab);
                lblPageInfo.Text = "Row updated successfully.";
            }
            catch (Exception ex)
            {
                lblPageInfo.Text = "An error occurred while attempting to update the row.";
                throw ex;
            }
            gvitem.EditIndex = -1;
            RefreshTable();
        }

        //protected void gvitem_RowUpdated(object sender, GridViewUpdatedEventArgs e)
        //{
        //    // Indicate whether the update operation succeeded.
        //    if (e.Exception == null)
        //    {
        //        lblPageInfo.Text = "Row updated successfully.";
        //    }
        //    else
        //    {
        //        e.ExceptionHandled = true;
        //        lblPageInfo.Text = "An error occurred while attempting to update the row.";
        //        //add logging here
        //    }
        //    gvitem.EditIndex = -1;
        //    RefreshTable();
        //}



        protected void gvitem_RowCommand(object sender, GridViewCommandEventArgs e)
        {
            try
            {


                if (e.CommandName.ToLower() == "increment")
                {
                    int RowIndex = int.Parse(e.CommandArgument.ToString());
                    GridViewRow changedRow = gvitem.Rows[RowIndex];
                    string item = gvitem.Rows[RowIndex].Cells[1].Text;
                    string lab = gvitem.Rows[RowIndex].Cells[6].Text;
                    Int32 changedQuantity;
                    if (changedRow.FindControl("lblQuantity") is null)
                    {
                        changedQuantity = int.Parse((changedRow.FindControl("txtQuantity") as TextBox).Text);
                    }
                    else
                    {
                        changedQuantity = int.Parse((changedRow.FindControl("lblQuantity") as Label).Text);
                    }
                    ViewState["changedQuantity"] = changedQuantity;
                    try
                    {
                        DBOps db = new DBOps();
                        int retval = db.UpdateInventoryTable(item, changedQuantity + 1, lab);
                        lblPageInfo.Text = retval.ToString() + " row updated";
                    }
                    catch (Exception ex)
                    {
                        lblPageInfo.Text = "An error occurred while attempting to update the row.";
                        throw ex;
                    }
                    //gvitem.EditIndex = -1;

                }
                if (e.CommandName.ToLower() == "decrement")
                {
                    int RowIndex = int.Parse(e.CommandArgument.ToString());
                    GridViewRow changedRow = gvitem.Rows[RowIndex];
                    string item = gvitem.Rows[RowIndex].Cells[1].Text;
                    string lab = gvitem.Rows[RowIndex].Cells[6].Text;
                    Int32 changedQuantity;
                    if (changedRow.FindControl("lblQuantity") is null)
                    {
                        changedQuantity = int.Parse((changedRow.FindControl("txtQuantity") as TextBox).Text);
                    }
                    else
                    {
                        changedQuantity = int.Parse((changedRow.FindControl("lblQuantity") as Label).Text);
                    }
                    ViewState["changedQuantity"] = changedQuantity;
                    try
                    {
                        DBOps db = new DBOps();
                        int retval = db.UpdateInventoryTable(item, changedQuantity - 1, lab);
                        lblPageInfo.Text = retval.ToString() + " row updated";
                    }
                    catch (Exception ex)
                    {
                        lblPageInfo.Text = "An error occurred while attempting to update the row.";
                        throw ex;
                    }
                    //gvitem.EditIndex = -1;

                }
                if (e.CommandName.ToLower() == "page")
                {
                    var pageno = e.CommandArgument;
                    gvitem.PageIndex = int.Parse(pageno.ToString());
                }
                if (e.CommandName.ToLower() == "update")
                {
                    int RowIndex = int.Parse(e.CommandArgument.ToString());
                    GridViewRow changedRow = gvitem.Rows[RowIndex];
                    string item = gvitem.Rows[RowIndex].Cells[1].Text;
                    string lab = gvitem.Rows[RowIndex].Cells[6].Text;
                    Int32 changedQuantity;
                    if (changedRow.FindControl("lblQuantity") is null)
                    {
                        changedQuantity = int.Parse((changedRow.FindControl("txtQuantity") as TextBox).Text);
                    }
                    else
                    {
                        changedQuantity = int.Parse((changedRow.FindControl("lblQuantity") as Label).Text);
                    }
                    gvitem_RowUpdatingcustom(item, lab, changedQuantity);
                }
            }
            catch (Exception ex)
            {
                throw ex;
            }
            RefreshTable();
        }

        protected void gvitem_RowDeleting(object sender, GridViewDeleteEventArgs e)
        {
            try
            {
                DBOps db = new DBOps();
                // GridViewRow gvr = (GridViewRow)(((Button)e.CommandSource).NamingContainer);

                int RowIndex = e.RowIndex;

                string item = gvitem.Rows[RowIndex].Cells[1].Text;
                string lab = gvitem.Rows[RowIndex].Cells[6].Text;

                int retval = db.DeleteInventoryTable(item, lab);
                if (retval > 0)
                {
                    lblPageInfo.Text = "Row deleted successfully.";
                    lblPageInfo.DataBind();
                }
                else
                {
                    lblPageInfo.ForeColor = System.Drawing.Color.Red;
                    lblPageInfo.Text = "An error occurred while attempting to delete the row.";
                    lblPageInfo.DataBind();
                    //lblPageInfo.ForeColor = System.Drawing.ColorTranslator.FromHtml("#0099ff"); //original blue color
                }

            }
            catch (Exception ex)
            {
                throw ex;
            }
            RefreshTable();
        }
        protected void gvitem_Sorting(object sender, GridViewSortEventArgs e)
        {

            switch (e.SortExpression)
            {
                case "Quantity":
                    if (e.SortDirection == SortDirection.Ascending)
                    {
                        DBOps db = new DBOps();

                        DataSet ds = db.ReadInventoryTable();
                        // For the future : - DataTable dt = ((DataSet)ViewState["gvitem"]).Tables["table"];
                        var query = from row in ds.Tables["table"].AsEnumerable()
                                    orderby row.Field<Int64>("Quantity")
                                    select row; // Asc query for Melder field;
                        DataTable result = query.CopyToDataTable();
                        Session["SortedView"] = result;

                        gvitem.DataSource = result;
                        gvitem.DataBind();

                    }
                    else
                    {
                        DBOps db = new DBOps();

                        DataSet ds = db.ReadInventoryTable();
                        //DataTable dt = ((DataSet)ViewState["gvitem"]).Tables["table"];
                        var query = from row in ds.Tables["table"].AsEnumerable()
                                    orderby row.Field<Int64>("Quantity") descending
                                    select row; // Desc query for Melder field ;
                        DataTable result = query.CopyToDataTable();
                        Session["SortedView"] = result;

                        gvitem.DataSource = result;
                        gvitem.DataBind();
                    }
                    break;
                // case statements for your other fields.
                case "Category":
                    if (e.SortDirection == SortDirection.Ascending)
                    {

                    }
                    else
                    {

                    }
                    break;

            }
        }

    }
}