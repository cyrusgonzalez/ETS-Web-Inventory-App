﻿using System;
using System.Linq;
using System.Web.UI.WebControls;
using System.Data;
using System.Diagnostics;

namespace Inventory_WebApp
{
    /// <summary>
    /// Class InventoryETS:
    /// Content: Backend Logic controlling InventoryPage.aspx
    /// </summary>
    public partial class InventoryETS : System.Web.UI.Page
    {


        /// <summary>
        /// Function Page_Load 
        /// Functionality: 
        /// contains code that runs when the page first loads and also when any page event is triggered that causes a postback
        /// A postback is a trip to the ASP server in response to some user action on an ASP element.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        protected void Page_Load(object sender, EventArgs e)
        {
            //First time page load commands to go inside this if block
            if (!IsPostBack)
            {
                try
                {
                    PopulateSearchColumnsDropdown();
                    PopulateLabsDropdown();
                    PopulateItemsDropDown();
                    PopulateCategoriesDropDown();
                    RefreshTable();
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
                //Check if search results returned any rows
                try
                {
                    if (txtSearchtext.Text != null || txtSearchtext.Text != "")         //If user has searched for something then do not hide the search pane
                    {
                        insert_inventory.Attributes.Remove("display");
                    }
                    else
                    {
                        insert_inventory.Attributes.Add("display", "none");
                    }
                    if ((DataTable)dgSearchResult.DataSource != null)
                    {
                        DataTable dt = (DataTable)dgSearchResult.DataSource;
                        if (dt.Rows.Count == 0)
                        {
                            lblPageInfo.Text = "No rows matched your search key";
                            lblPageInfo.ForeColor = System.Drawing.Color.Red;
                            lblPageInfo.DataBind();
                        }
                        else
                        {

                        }
                    }

                }
                catch (Exception ex)
                {

                }
                //lblPageInfo.ForeColor = System.Drawing.ColorTranslator.FromHtml("#0099ff"); //original blue color

                //Clear all Inserted, Updated or Search textboxes
                lblInsertInfo.Text = "";
                lblPageInfo.Text = "";
                lblSearchInfo.Text = "";
                lblUpdateInfo.Text = "";
            }
        }

        #region Getting data from DB and setting table and related functions
        protected void btnLoad_Click(object sender, EventArgs e)
        {
            DBOps db = new DBOps();
            DataSet ds = db.ReadInventoryTable();
            gvitem.DataSource = ds;
            gvitem.DataBind();
            ddlLabselect.SelectedIndex = -1;
        }

        protected void RefreshTable()
        {
            try
            {
                if (ddlLabselect.SelectedValue == "All")
                {
                    DBOps db = new DBOps();

                    if (Session["SortedView"] != null)          //Added a check to use the sorted rows in case the user sorts then filters.
                    {
                        gvitem.DataSource = Session["SortedView"];
                        gvitem.DataBind();
                    }
                    else
                    {
                        DataSet ds = db.ReadInventoryTable();
                        gvitem.DataSource = ds;
                        gvitem.DataBind();
                    }
                    //Session["gvitems"] = ds;//.Tables["table"];
                }
                else
                {
                    DBOps db = new DBOps();
                    //DataSet ds = (DataSet)ViewState["gvitems"];    // Apparently grid view items are not stored persistently across postabacks so then either I have to hit the db again or use the viewstate i.e. gvitems.DataSource doesnt work here

                    DataTable dt;

                    if (Session["SortedView"] != null)           //Added a check to use the sorted rows in case the user sorts then filters.
                    {
                        dt = (DataTable)Session["SortedView"];
                    }
                    else
                    {
                        DataSet ds = db.ReadInventoryTable();
                        dt = ds.Tables["Table"];
                    }

                    var query = from row in dt.AsEnumerable()
                                where row.Field<string>("lab") == (ddlLabselect.SelectedValue)
                                select row;
                    DataTable result = query.CopyToDataTable();
                    if (result.Rows.Count == 0)
                    {
                        lblPageInfo.Text = "There are no items in this lab";
                        lblPageInfo.ForeColor = System.Drawing.Color.Red;
                        lblPageInfo.DataBind();
                    }
                    else
                    {
                        gvitem.DataSource = result;
                        gvitem.DataBind();
                    }

                }
            }
            catch (Exception ex)
            {
                if (ex.Message.ToLower().Contains("no datarows"))
                {
                    lblPageInfo.Text = "There are no items in this lab";
                    lblPageInfo.ForeColor = System.Drawing.Color.Red;
                    lblPageInfo.DataBind();
                }

            }
        }

        protected string CheckSort()
        {
            string column_sort = "";
            try
            {
                switch ((string)ViewState["gvitemsort"].ToString().ToLower())
                {
                    case "":
                    case "none":
                        return column_sort;
                    case "quantity sortedasc":
                        return "Quantity ASC";
                    case "quantity sorteddesc":
                        return "Quantity DESC";
                    case "category sortedasc":
                        return "Category ASC";
                    case "category sorteddesc":
                        return "Category DESC";
                    case "item sortedasc":
                        return "ItemCode ASC";
                    case "item sorteddesc":
                        return "ItemCode DESC";
                }

            }
            catch (Exception ex)
            {
                return "";
            }
            return "";
        }

        protected void RefreshTable(string callerfunc)
        {
            try
            {
                if (callerfunc == "gvitem_RowCommand")  //this is what is passed, idk may have to force non-inline the updating custom command gvitem_RowUpdatingcustom
                {
                    DBOps db = new DBOps();
                    DataSet ds = db.ReadInventoryTable();
                    ds.Tables["table"].DefaultView.Sort = CheckSort() ?? "";//"Quantity ASC";   //read from ViewState['gvitemsort'] as Quantity ASC/LAB DESC
                    Session["SortedView"] = ds;     //notowkring FIX HERE 30/6/20

                }
                else if (callerfunc == "OnRowCommand")  // write case for "OnRowCommand" - passed in case of increment/decrement operator 
                {
                    DBOps db = new DBOps();
                    DataSet ds = db.ReadInventoryTable();
                    ds.Tables["table"].DefaultView.Sort = ((string)ViewState["gvitemsort"]) ?? "";  //read from ViewState['gvitemsort'] as Quantity ASC/LAB DESC

                    Session["SortedView"] = ds.Tables["table"].DefaultView.ToTable();     //notowkring FIX HERE 30/6/20
           
                }

                if (ddlLabselect.SelectedValue == "All")
                {
                    DBOps db = new DBOps();

                    if (Session["SortedView"] != null)          //Added a check to use the sorted rows in case the user sorts then filters.
                    {
                        gvitem.DataSource = Session["SortedView"];
                        gvitem.DataBind();
                    }
                    else
                    {
                        DataSet ds = db.ReadInventoryTable();
                        gvitem.DataSource = ds;
                        gvitem.DataBind();
                    }

                }
                else
                {
                    DBOps db = new DBOps();
                    //DataSet ds = (DataSet)ViewState["gvitems"];    // Apparently grid view items are not stored persistently across postabacks so then either I have to hit the db again or use the viewstate i.e. gvitems.DataSource doesnt work here

                    DataTable dt;

                    if (Session["SortedView"] != null)           //Added a check to use the sorted rows in case the user sorts then filters.
                    {
                        dt = (DataTable)Session["SortedView"];
                    }
                    else
                    {
                        DataSet ds = db.ReadInventoryTable();
                        dt = ds.Tables["Table"];
                    }

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
        #endregion

        #region Dropdown population
        protected void PopulateSearchColumnsDropdown()
        {
            try
            {
                DBOps db = new DBOps();
                DataSet ds = db.GetInventoryColumns();
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
                DataSet ds = db.GetLabs();

                ddlInsertLab.DataSource = ds;
                ddlInsertLab.DataTextField = "name";
                ddlInsertLab.DataValueField = "name";
                ddlInsertLab.DataBind();

                //Adding new row to lab filter to show all labs. 
                DataRow dr = ds.Tables["table"].NewRow();
                dr["name"] = "All";

                ds.Tables["table"].Rows.InsertAt(dr, 0);
                ddlLabselect.DataSource = ds;
                ddlLabselect.DataTextField = "name";
                ddlLabselect.DataValueField = "name";
                ddlLabselect.DataBind();

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

        protected void PopulateItemsDropDown()
        {
            try
            {
                DBOps db = new DBOps();
                DataSet ds = db.GetItems();

                //Adding new row to lab filter to show all labs. 
                DataRow dr = ds.Tables["table"].NewRow();
                dr["itemCode"] = "All";

                ds.Tables["table"].Rows.InsertAt(dr, 0);

                ddlItemSelect.DataSource = ds;
                ddlItemSelect.DataTextField = "itemCode";
                ddlItemSelect.DataValueField = "itemCode";
                ddlItemSelect.DataBind();

            }
            catch(Exception ex)
            {

            }
        }

        protected void PopulateCategoriesDropDown()
        {
            try
            {
                DBOps db = new DBOps();
                DataSet ds = db.GetCategories();

                //Adding new row to lab filter to show all labs. 
                DataRow dr = ds.Tables["table"].NewRow();
                dr["category"] = "All";

                ds.Tables["table"].Rows.InsertAt(dr, 0);

                ddlCategorySelect.DataSource = ds;
                ddlCategorySelect.DataTextField = "category";
                ddlCategorySelect.DataValueField = "category";
                ddlCategorySelect.DataBind();
            }
            catch (Exception ex)
            {

            }
        }
        #endregion

        #region Dropdown functions fired when index changed(new item selected)
        protected void ddlLabselect_SelectedIndexChanged(object sender, EventArgs e)
        {
            try
            {
                if (ddlLabselect.SelectedValue == "All")
                {
                    DBOps db = new DBOps();

                    if (Session["SortedView"] != null)          //Added a check to use the sorted rows in case the user sorts then filters.
                    {
                        gvitem.DataSource = Session["SortedView"];
                        gvitem.DataBind();
                    }
                    else
                    {
                        DataSet ds = db.ReadInventoryTable();
                        gvitem.DataSource = ds;
                        gvitem.DataBind();
                    }
                    //Session["gvitems"] = ds;//.Tables["table"];
                }
                else
                {
                    DBOps db = new DBOps();
                    //DataSet ds = (DataSet)ViewState["gvitems"];    // Apparently grid view items are not stored persistently across postabacks so then either I have to hit the db again or use the viewstate i.e. gvitems.DataSource doesnt work here

                    DataTable dt;

                    if (Session["SortedView"] != null)           //Added a check to use the sorted rows in case the user sorts then filters.
                    {
                        dt = (DataTable)Session["SortedView"];
                    }
                    else
                    {
                        DataSet ds = db.ReadInventoryTable();
                        dt = ds.Tables["Table"];
                    }

                    var query = from row in dt.AsEnumerable()
                                where row.Field<string>("lab") == (ddlLabselect.SelectedValue)
                                select row;
                    DataTable result = query.CopyToDataTable();
                    if (result.Rows.Count == 0)
                    {
                        lblPageInfo.Text = "There are no items in this lab";
                        lblPageInfo.ForeColor = System.Drawing.Color.Red;
                        lblPageInfo.DataBind();
                    }
                    else
                    {
                        gvitem.DataSource = result;
                        gvitem.DataBind();
                    }

                }
            }
            catch (Exception ex)
            {
                //throw ex;
            }
        }

        protected void ddlCategorySelect_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (ddlCategorySelect.SelectedValue == "All")
            {
                DBOps db = new DBOps();

                if (Session["SortedView"] != null)          //Added a check to use the sorted rows in case the user sorts then filters.
                {
                    gvitem.DataSource = Session["SortedView"];
                    gvitem.DataBind();
                }
                else
                {
                    DataSet ds = db.ReadInventoryTable();
                    gvitem.DataSource = ds;
                    gvitem.DataBind();
                }
                //Session["gvitems"] = ds;//.Tables["table"];
            }
            else
            {
                DBOps db = new DBOps();
                //DataSet ds = (DataSet)ViewState["gvitems"];    // Apparently grid view items are not stored persistently across postabacks so then either I have to hit the db again or use the viewstate i.e. gvitems.DataSource doesnt work here

                DataTable dt;

                if (Session["SortedView"] != null)           //Added a check to use the sorted rows in case the user sorts then filters.
                {
                    dt = (DataTable)Session["SortedView"];
                }
                else
                {
                    DataSet ds = db.ReadInventoryTable();
                    dt = ds.Tables["Table"];
                }

                var query = from row in dt.AsEnumerable()
                            where row.Field<string>("category") == (ddlCategorySelect.SelectedValue)
                            select row;
                DataTable result = query.CopyToDataTable();
                if (result.Rows.Count == 0)
                {
                    lblPageInfo.Text = "There are no items in this lab";
                    lblPageInfo.ForeColor = System.Drawing.Color.Red;
                    lblPageInfo.DataBind();
                }
                else
                {
                    gvitem.DataSource = result;
                    gvitem.DataBind();
                }

            }
        }

        protected void ddlItemSelect_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (ddlItemSelect.SelectedValue == "All")
            {
                DBOps db = new DBOps();

                if (Session["SortedView"] != null)          //Added a check to use the sorted rows in case the user sorts then filters.
                {
                    gvitem.DataSource = Session["SortedView"];
                    gvitem.DataBind();
                }
                else
                {
                    DataSet ds = db.ReadInventoryTable();
                    gvitem.DataSource = ds;
                    gvitem.DataBind();
                }
                //Session["gvitems"] = ds;//.Tables["table"];
            }
            else
            {
                DBOps db = new DBOps();
                //DataSet ds = (DataSet)ViewState["gvitems"];    // Apparently grid view items are not stored persistently across postabacks so then either I have to hit the db again or use the viewstate i.e. gvitems.DataSource doesnt work here

                DataTable dt;

                if (Session["SortedView"] != null)           //Added a check to use the sorted rows in case the user sorts then filters.
                {
                    dt = (DataTable)Session["SortedView"];
                }
                else
                {
                    DataSet ds = db.ReadInventoryTable();
                    dt = ds.Tables["Table"];
                }

                var query = from row in dt.AsEnumerable()
                            where row.Field<string>("itemCode") == (ddlItemSelect.SelectedValue)
                            select row;
                DataTable result = query.CopyToDataTable();
                if (result.Rows.Count == 0)
                {
                    lblPageInfo.Text = "There are no items in this lab";
                    lblPageInfo.ForeColor = System.Drawing.Color.Red;
                    lblPageInfo.DataBind();
                }
                else
                {
                    gvitem.DataSource = result;
                    gvitem.DataBind();
                }

            }
        }

        #endregion

        #region button click event functions
        protected void btnInsert_Click(object sender, EventArgs e)
        {
            try
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
            catch (Exception ex)
            {

            }
            finally
            {
                txtInsertItem.Text = "";
                txtInsertQuantity.Text = "";
                ddlInsertLab.SelectedIndex = 0;
            }

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
            DataSet searchColumns = new DBOps().GetInventoryColumns();
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
                            where row.Field<string>(col) != null && row.Field<string>(col).ToLower().Contains(txtSearchtext.Text.ToLower())
                            select row;
                result = query.CopyToDataTable();
            }


            int count = result.Rows.Count;
            lblSearchInfo.Text = count.ToString() + " row(s) matched";
            lblSearchInfo.DataBind();

            dgSearchResult.DataSource = result;
            dgSearchResult.DataBind();
        }

        #endregion

        #region GridView gvitem functions
        protected void gvitem_PageIndexChanging(object sender, GridViewPageEventArgs e)
        {
            if (Session["SortedView"] != null)
            {
                gvitem.PageIndex = e.NewPageIndex;
                gvitem.DataSource = Session["SortedView"];
                gvitem.DataBind();
            }
            else
            {
                gvitem.PageIndex = e.NewPageIndex;
                RefreshTable();
            }
        }

        protected void gvitem_RowDataBound(object sender, GridViewRowEventArgs e)
        {
            //Add formatting code, if any
            try
            {
                GridViewRow row = e.Row;
                int Quantity = int.Parse((row.FindControl("lblQuantity") as Label).Text);
                //CheckRowQuantityColor(e.Row.RowIndex);
                if (Quantity < 15)       //Change this hardcode to a properties file config. one for medium , one for low
                {
                    row.BackColor = System.Drawing.Color.Orange;
                }
                if (Quantity < 10)
                {
                    row.BackColor = System.Drawing.Color.Red;
                }
            }
            catch (Exception ex)
            {

            }

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
            //Placeholder method to handle fired event. Actual code behaviour initiated in gvitem_RowCommand under the 'update' case and the rest is in the gvitem_RowUpdatingCustom functions
        }

        protected void gvitem_RowUpdatingcustom(string changedItem, string changedLab, int changedQuantity)
        {

            try
            {
                DBOps db = new DBOps();
                int retval = db.UpdateInventoryTable(changedItem, changedQuantity, changedLab);
                if (retval > 0)
                {
                    lblPageInfo.ForeColor = System.Drawing.ColorTranslator.FromHtml("#0099ff"); //original blue color
                    lblPageInfo.Text = "Row updated successfully.";
                }
                else
                {
                    lblPageInfo.ForeColor = System.Drawing.Color.Red;
                    lblPageInfo.Text = "Row updating failed.";
                }
            }
            catch (Exception ex)
            {
                lblPageInfo.Text = "An error occurred while attempting to update the row.";
                throw ex;
            }
            gvitem.EditIndex = -1;
            string callerfunc = new StackFrame(1).GetMethod().Name;
            RefreshTable(callerfunc);
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
                    string lab = gvitem.Rows[RowIndex].Cells[8].Text;
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

                    string callerfunc = new StackFrame(1).GetMethod().Name;
                    RefreshTable(callerfunc);
                    //Change the respective color of the row after incrementing, decrementing or updating
                    CheckRowQuantityColor(int.Parse(e.CommandArgument.ToString()));
                }
                if (e.CommandName.ToLower() == "decrement")
                {
                    int RowIndex = int.Parse(e.CommandArgument.ToString());
                    GridViewRow changedRow = gvitem.Rows[RowIndex];
                    string item = gvitem.Rows[RowIndex].Cells[1].Text;
                    string lab = gvitem.Rows[RowIndex].Cells[8].Text;
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
                    string callerfunc = new StackFrame(1).GetMethod().Name;
                    RefreshTable(callerfunc);
                    //Change the respective color of the row after incrementing, decrementing or updating
                    CheckRowQuantityColor(int.Parse(e.CommandArgument.ToString()));
                }
                //if (e.CommandName.ToLower() == "page")
                //{
                //    var pageno = e.CommandArgument;
                //    gvitem.PageIndex = int.Parse(pageno.ToString());
                //}
                if (e.CommandName.ToLower() == "update")
                {
                    int RowIndex = int.Parse(e.CommandArgument.ToString());
                    GridViewRow changedRow = gvitem.Rows[RowIndex];
                    string item = gvitem.Rows[RowIndex].Cells[1].Text;
                    string lab = gvitem.Rows[RowIndex].Cells[8].Text;
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
                    //Change the respective color of the row after incrementing, decrementing or updating
                    CheckRowQuantityColor(int.Parse(e.CommandArgument.ToString()));
                }


            }
            catch (Exception ex)
            {
                throw ex;
            }

        }

        protected void gvitem_RowDeleting(object sender, GridViewDeleteEventArgs e)
        {
            try
            {
                DBOps db = new DBOps();
                // GridViewRow gvr = (GridViewRow)(((Button)e.CommandSource).NamingContainer);

                int RowIndex = e.RowIndex;

                string item = gvitem.Rows[RowIndex].Cells[1].Text;
                string lab = gvitem.Rows[RowIndex].Cells[8].Text;

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
            try
            {


                switch (e.SortExpression)
                {
                    case "Quantity":
                        DBOps db = new DBOps();
                        DataSet ds = db.ReadInventoryTable();
                        OrderedEnumerableRowCollection<DataRow> query;
                        DataTable result;
                        switch (((string)ViewState["gvitemsort"]) ?? "NONE")
                        {
                            case "NONE":
                            case "Quantity SORTEDDESC":
                                // For the future : - DataTable dt = ((DataSet)ViewState["gvitem"]).Tables["table"]; if storing and reading table rows from view/session state
                                query = from row in ds.Tables["table"].AsEnumerable()
                                        orderby row.Field<Int64>("Quantity")
                                        select row; // Asc query for Melder field;
                                result = query.CopyToDataTable();
                                Session["SortedView"] = result;

                                gvitem.DataSource = result;
                                gvitem.DataBind();

                                ViewState["gvitemsort"] = "Quantity SORTEDASC";
                                break;
                            case "Quantity SORTEDASC":
                                //DataTable dt = ((DataSet)ViewState["gvitem"]).Tables["table"];
                                query = from row in ds.Tables["table"].AsEnumerable()
                                        orderby row.Field<Int64>("Quantity") descending
                                        select row; // Desc query for Melder field ;
                                result = query.CopyToDataTable();
                                Session["SortedView"] = result;

                                gvitem.DataSource = result;
                                gvitem.DataBind();
                                ViewState["gvitemsort"] = "Quantity SORTEDDESC"; //quantity desc/asc for easier use in the refreshtable func
                                break;

                        }
                        break;
                    // case statements for your other fields.
                    case "Category":
                        DBOps db1 = new DBOps();
                        DataSet ds1 = db1.ReadInventoryTable();
                        OrderedEnumerableRowCollection<DataRow> query1;
                        DataTable result1;
                        switch (((string)ViewState["gvitemsort"]) ?? "NONE")    //Check if it exists, if it doesn't substitute NONE
                        {
                            case "NONE":
                            case "Category SORTEDDESC":
                                // For the future : - DataTable dt = ((DataSet)ViewState["gvitem"]).Tables["table"]; if storing and reading table rows from view/session state
                                query1 = from row in ds1.Tables["table"].AsEnumerable()
                                         orderby row.Field<string>("category")
                                         select row; // Asc query for Melder field;
                                result1 = query1.CopyToDataTable();
                                Session["SortedView"] = result1;

                                gvitem.DataSource = result1;
                                gvitem.DataBind();

                                ViewState["gvitemsort"] = "Category SORTEDASC";
                                break;
                            case "Category SORTEDASC":
                                //DataTable dt = ((DataSet)ViewState["gvitem"]).Tables["table"];
                                query1 = from row in ds1.Tables["table"].AsEnumerable()
                                         orderby row.Field<string>("category") descending
                                         select row; // Desc query for Melder field ;
                                result1 = query1.CopyToDataTable();
                                Session["SortedView"] = result1;

                                gvitem.DataSource = result1;
                                gvitem.DataBind();
                                ViewState["gvitemsort"] = " Category SORTEDDESC"; //quantity desc/asc for easier use in the refreshtable func
                                break;
                            default:
                                query1 = from row in ds1.Tables["table"].AsEnumerable()
                                    orderby row.Field<string>("category")
                                    select row; // Asc query for Melder field;
                                result1 = query1.CopyToDataTable();
                                Session["SortedView"] = result1;

                                gvitem.DataSource = result1;
                                gvitem.DataBind();

                                ViewState["gvitemsort"] = "Category SORTEDASC";
                                break;

                        }
                        break;
                    case "Item":
                        DBOps db2 = new DBOps();
                        DataSet ds2 = db2.ReadInventoryTable();
                        OrderedEnumerableRowCollection<DataRow> query2;
                        DataTable result2;

                        switch (((string)ViewState["gvitemsort"]) ?? "NONE")    //Check if it exists, if it doesn't substitute NONE
                        {
                            case "NONE":
                            case "Item SORTEDDESC":
                                // For the future : - DataTable dt = ((DataSet)ViewState["gvitem"]).Tables["table"]; if storing and reading table rows from view/session state
                                query2 = from row in ds2.Tables["table"].AsEnumerable()
                                         orderby row.Field<string>("ItemCode")
                                         select row; // Asc query for Melder field;
                                result2 = query2.CopyToDataTable();
                                Session["SortedView"] = result2;

                                gvitem.DataSource = result2;
                                gvitem.DataBind();

                                ViewState["gvitemsort"] = "Item SORTEDASC";
                                break;
                            case "Item SORTEDASC":
                                //DataTable dt = ((DataSet)ViewState["gvitem"]).Tables["table"];
                                query2 = from row in ds2.Tables["table"].AsEnumerable()
                                         orderby row.Field<string>("ItemCode") descending
                                         select row; // Desc query for Melder field ;
                                result2 = query2.CopyToDataTable();
                                Session["SortedView"] = result2;

                                gvitem.DataSource = result2;
                                gvitem.DataBind();
                                ViewState["gvitemsort"] = "Item SORTEDDESC"; //quantity desc/asc for easier use in the refreshtable func
                                break;

                        }

                        break;
                    case "Lab":
                        DBOps db3 = new DBOps();
                        DataSet ds3 = db3.ReadInventoryTable();
                        OrderedEnumerableRowCollection<DataRow> query3;
                        DataTable result3;

                        switch (((string)ViewState["gvitemsort"]) ?? "NONE")    //Check if it exists, if it doesn't substitute NONE
                        {
                            case "NONE":
                            case "SORTEDDESC":
                                // For the future : - DataTable dt = ((DataSet)ViewState["gvitem"]).Tables["table"]; if storing and reading table rows from view/session state
                                query3 = from row in ds3.Tables["table"].AsEnumerable()
                                         orderby row.Field<string>("lab")
                                         select row; // Asc query for Melder field;
                                result3 = query3.CopyToDataTable();
                                Session["SortedView"] = result3;

                                gvitem.DataSource = result3;
                                gvitem.DataBind();

                                ViewState["gvitemsort"] = "Lab SORTEDASC";
                                break;
                            case "SORTEDASC":
                                //DataTable dt = ((DataSet)ViewState["gvitem"]).Tables["table"];
                                query3 = from row in ds3.Tables["table"].AsEnumerable()
                                         orderby row.Field<string>("lab") descending
                                         select row; // Desc query for Melder field ;
                                result3 = query3.CopyToDataTable();
                                Session["SortedView"] = result3;

                                gvitem.DataSource = result3;
                                gvitem.DataBind();
                                ViewState["gvitemsort"] = "Lab SORTEDDESC"; //quantity desc/asc for easier use in the refreshtable func
                                break;

                        }
                        break;
                }
            }
            catch (Exception ex)
            {

            }
        }

        #endregion

        protected void CheckRowQuantityColor(Int32 rowIndex)
        {
            GridViewRow row = gvitem.Rows[rowIndex];
            int Quantity = int.Parse((row.FindControl("lblQuantity") as Label).Text);
            if (Quantity < 15)       //Change this hardcode to a properties file config. one for medium , one for low
            {
                row.BackColor = System.Drawing.Color.Orange;
            }
            if (Quantity < 10)
            {
                row.BackColor = System.Drawing.Color.Red;
            }
        }

    }
}