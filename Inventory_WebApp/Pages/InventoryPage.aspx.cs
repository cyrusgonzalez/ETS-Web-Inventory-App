using System;
using System.Collections.Generic;
using System.Data;
using System.Diagnostics;
using System.Linq;
using System.Web.UI.WebControls;
using Inventory_WebApp.DatabaseInterface;
using Exception = System.Exception;

namespace Inventory_WebApp.Pages
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
                    //throw ex;
                }
            }
            else
            {
                //Check if search results returned any rows
                try
                {
                    if (HiddenFieldShowHideSearchPanel.Value == "Show")
                    {
                        insert_inventory.Style.Remove("display");
                    }
                    if (HiddenFieldShowHideSearchPanel.Value == "Hidden")
                    {
                        insert_inventory.Style.Add("display","none");
                    }

                    if ((GridView)gvSearchResult.DataSource != null)
                    {
                        GridView tempGvDataSource = (GridView)gvSearchResult.DataSource;
                        if (tempGvDataSource.Rows.Count == 0)
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
                    lblPageInfo.Text = "An error occurred in Page Load." + ex.Message; 
                    //throw ex;
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
            ddlCategorySelect.SelectedIndex = -1;
            ddlItemSelect.SelectedIndex = -1;
            ViewState["gvitemsort"] = null;
            Session["SortedView"] = null;
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
                }
                else
                {
                    DBOps db = new DBOps();

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
            string column_sort = ViewState["gvitemsort"] != null ? ViewState["gvitemsort"].ToString().ToLower() : null;
            try
            {
                if (column_sort == "" ||
                    column_sort == "none" || column_sort == null)
                    return "";
                else if (column_sort == "quantity sortedasc")
                    return "Quantity ASC";
                else if (column_sort == "quantity sorteddesc")
                    return "Quantity DESC";
                else if (column_sort == "category sortedasc")
                    return "Category ASC";
                else if (column_sort == "category sorteddesc")
                    return "Category DESC";
                else if (column_sort == "item sortedasc")
                    return "ItemCode ASC";
                else if (column_sort == "item sorteddesc") return "ItemCode DESC";
            }
            catch (Exception ex)
            {
                lblPageInfo.Text = "An error occurred in CheckSort()." + ex.Message; 
                return "";
            }
            return "";
        }

        protected void RefreshTable(string callerfunc)
        {
            try
            {
                if (callerfunc == "gvitem_RowCommand" || callerfunc == "OnRowCommand" || callerfunc == "OnClick")  //OnClick - btnInsert , OnRowCommand - Inc/Dec/Update
                {
                    DBOps db = new DBOps();
                    DataSet ds = db.ReadInventoryTable();
                    ds.Tables["table"].DefaultView.Sort = CheckSort() ?? "";           //read from ViewState['gvitemsort'] as Quantity ASC/LAB DESC
                    Session["SortedView"] = ds.Tables["table"].DefaultView.ToTable();
                }

                if (Session["Filters"] == null)
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
                    var FilterExpression = Session["Filters"] as Dictionary<string, string>;
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

                    EnumerableRowCollection<DataRow> query = dt.AsEnumerable();

                    if (FilterExpression.ContainsKey("lab") && FilterExpression["lab"] != "All")
                    {
                        query = from row in dt.AsEnumerable()
                            where row.Field<string>("lab") == (FilterExpression["lab"])
                            select row;
                        dt = query.CopyToDataTable();
                    }
                    if (FilterExpression.ContainsKey("category") && FilterExpression["category"] != "All")
                    {
                        query = from row in dt.AsEnumerable()
                            where row.Field<string>("category") == (FilterExpression["category"])
                            select row;
                        dt = query.CopyToDataTable();
                    }
                    if (FilterExpression.ContainsKey("item") && FilterExpression["item"] != "All")
                    {
                        query = from row in dt.AsEnumerable()
                            where row.Field<string>("itemCode") == (FilterExpression["item"])
                            select row;
                        dt = query.CopyToDataTable();
                    }
                    
                    //var query = from row in dt.AsEnumerable()
                    //            where row.Field<string>("lab") == (ddlLabselect.SelectedValue)
                    //            select row;
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
                    lblPageInfo.Text = "There are no items that match your filters";
                    lblPageInfo.ForeColor = System.Drawing.Color.Red;
                    lblPageInfo.DataBind();
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
                for (int i = 0; i < ds.Tables[0].Rows.Count; i++)
                {
                    if (ds.Tables[0].Rows[i]["name"].ToString().Trim().ToLower().Contains("id") ||
                        ds.Tables[0].Rows[i]["name"].ToString().Trim().ToLower().Contains("quantity"))
                    {
                        ds.Tables[0].Rows.RemoveAt(i);
                    }   
                }
                //DataRow[] result = ds.Tables[0].Select("name = 'id'");
                //foreach (DataRow row in ds.Tables[0].Rows)
                //{
                //    if (row["name"].ToString().Trim().ToLower().Contains("id") || row["name"].ToString().Trim().ToLower().Contains("quantity"))
                //        ds.Tables[0].Rows.Remove(row);
                //}
                ddlColumn.DataSource = ds;
                ddlColumn.DataTextField = "name";
                ddlColumn.DataValueField = "name";
                ddlColumn.DataBind();
            }
            catch (Exception ex)
            {
                //throw ex;
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
                //throw ex;
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
            catch (Exception ex)
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
                string callerfunc = new StackFrame(0).GetMethod().Name;
                CreateUpdateFilterExpression(callerfunc, ddlLabselect.SelectedValue);
                RefreshTable(callerfunc);
            }
            catch (Exception ex)
            {
                //throw ex;
                if (ex.Message.ToLower().Contains("no datarows"))
                {
                    lblPageInfo.Text = "There are no items in this lab";
                    lblPageInfo.ForeColor = System.Drawing.Color.Red;
                    lblPageInfo.DataBind();
                }
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
            string callerfunc = new StackFrame(0).GetMethod().Name;
            CreateUpdateFilterExpression(callerfunc, ddlCategorySelect.SelectedValue);
            RefreshTable(callerfunc);
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
            string callerfunc = new StackFrame(0).GetMethod().Name;
            CreateUpdateFilterExpression(callerfunc, ddlItemSelect.SelectedValue);
            RefreshTable(callerfunc);
        }

        /// <summary>
        /// A function to be developed in the future to chain filters.
        /// </summary>
        protected void CreateUpdateFilterExpression(string callerFilter, string filterValue)
        {
            Dictionary<string, string> FilterExpression;
            if (Session["Filters"] != null)
            {
                FilterExpression = Session["Filters"] as Dictionary<string, string>;
            }
            else
            {
                FilterExpression = new Dictionary<string, string>();
            }

            var filtername = callerFilter.Split('_')[0].ToLower() == "ddllabselect" ? "lab" : callerFilter.Split('_')[0].ToLower() == "ddlcategoryselect" ? "category" : "item";
            FilterExpression[filtername] = filterValue;
            // FilterExpression.Add(filtername, filterValue);
            Session["Filters"] = FilterExpression;
        }

        #endregion

        #region button click event functions
        protected void btnInsert_Click(object sender, EventArgs e)
        {
            try
            {
                DBOps db = new DBOps();

                string itemName = txtInsertItem.Text;
                string itemCode = txtInsertItemCode.Text;
                string lab = ddlInsertLab.SelectedValue;
                string description = txtInsertDescription.Text;
                string category = txtInsertCategory.Text;
                int alertQuantity = int.Parse(txtInsertAlertQuant.Text?? string.Empty);
                int warnQuantity = int.Parse(txtInsertWarningQuant.Text?? string.Empty);
                long itemQuantity;
                if (long.TryParse(txtInsertQuantity.Text, out itemQuantity))
                {
                    // it's a valid integer => you could use the distance variable here
                    itemQuantity = long.Parse(txtInsertQuantity.Text);
                }
                else
                {
                    ClientScript.RegisterStartupScript(GetType(), "NumericCheck", "alert('Please enter a Numeric Quantity.');", true);
                    lblPageInfo.Text = "Please enter a Numeric Quantity.";
                    lblPageInfo.DataBind();
                }

                //int retval = db.InsertInventoryTable(itemName, itemCode, itemQuantity, lab, category, description);

                int retval = db.InsertUpdateInventoryTable(itemName, itemCode, itemQuantity, lab, category, description,warnQuantity, alertQuantity);   
                //lblInsertInfo.Text = retval.ToString() + " row inserted";
                //lblInsertInfo.DataBind();

                //lblPageInfo.Text = retval.ToString() + " row inserted";
                //lblPageInfo.DataBind();

                string callerfunc = new StackFrame(1).GetMethod().Name;
                RefreshTable(callerfunc);
            }
            catch (Exception ex)
            {

            }
            finally
            {
                txtInsertItem.Text = "";
                txtInsertQuantity.Text = "";
                txtInsertItemCode.Text = "";
                txtInsertCategory.Text = "";
                txtInsertDescription.Text = "";
                txtInsertAlertQuant.Text = "";
                txtInsertWarningQuant.Text = "";
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
            Type returnType = null;
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
                lblErr.Text = "Error in GetMyType()";
            }
            return returnType;
            //catch (Exception ex)
            //{
            //    //throw ex;
            //}
        }

        protected void btnSearch_Click(object sender, EventArgs e)
        {
            DataSet searchColumns = new DBOps().GetInventoryColumns();
            string col = ddlColumn.SelectedValue;
            var type = from d in searchColumns.Tables["Table"].AsEnumerable()
                       where d.Field<string>("name") == col
                       select d.Field<string>("type");

            Type searchColumnType = GetMyType(type.ToArray()[0]);

            //Optimization: Try querying/Linq querying the datasource of the main grid gvItems. Will save a DB Trip

            DBOps db = new DBOps();
            DataSet ds = db.ReadInventoryTable();
            DataTable dt = ds.Tables["Table"];
            DataTable result = new DataTable();


            if (searchColumnType == typeof(int))
            {
                try
                {
                    long itemQuantity;
                    if (long.TryParse(txtSearchtext.Text, out itemQuantity))
                    {
                        itemQuantity = long.Parse(txtSearchtext.Text);
                    }
                    else
                    {
                        ClientScript.RegisterStartupScript(GetType(), "NumericCheck", "alert('Please enter a Numeric Quantity in the Search Box or Check the selected column.');", true);
                        lblPageInfo.Text = "Please enter a Numeric Quantity in the Search Box or Check the selected column.";
                        lblPageInfo.DataBind();
                    }
                }
                catch(Exception ex)
                {
                    //log error here
                }

                var query = from row in dt.AsEnumerable()
                            where row.Field<Int64>(col) == Int64.Parse(txtSearchtext.Text)
                            select row;
                // Source: https://stackoverflow.com/questions/3259895/if-linq-result-is-empty to handle no matches 23 Jan 2021
                if (query.Any()) //attempts to see if there is at least one element in the list
                {
                    result = query.CopyToDataTable();
                }
                
            }
            else
            {
                var query = from row in dt.AsEnumerable()
                            where row.Field<string>(col) != null && row.Field<string>(col).ToLower().Contains(txtSearchtext.Text.ToLower())
                            select row;

                if (query.Any())
                {
                    result = query.CopyToDataTable();
                }
            }


            int count = result.Rows.Count;

            if (count > 0)
            {
                lblSearchInfo.Text = count.ToString() + " row(s) matched";
                lblSearchInfo.DataBind();

                gvSearchResult.DataSource = result;
                gvSearchResult.DataBind();
            }
            else
            {
                lblSearchInfo.Text = "No matches for your query: " + col + " = " + txtSearchtext.Text.ToLower();
                lblSearchInfo.DataBind();
            }
            
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
                int quantity = int.Parse((row.FindControl("lblQuantity") as Label)?.Text ?? string.Empty);
                int warnQuantity = int.Parse(gvitem.DataKeys[e.Row.RowIndex]["warning_quantity"].ToString());
                int alertQuantity = int.Parse(gvitem.DataKeys[e.Row.RowIndex]["alert_quantity"].ToString());
                //CheckRowQuantityColor(e.Row.RowIndex);

                if (quantity < warnQuantity)      
                {
                    row.BackColor = System.Drawing.Color.Orange;
                }
                if (quantity < alertQuantity)
                {
                    row.ForeColor = System.Drawing.Color.White;
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
                TableCell cell;

                if (row.Cells.Count > 7)
                {
                    //Code to move the Edit button to the right end of the table
                    cell = row.Cells[0];
                    row.Cells.Remove(cell);
                    row.Cells.AddAt(8, cell);
                    //row.Cells.Add(cell);
                }
                
            }
            catch (Exception ex)
            {
                throw;
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
                lblPageInfo.Text = "An error occurred while attempting to update the row. " + ex.Message;
                //throw ex;
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
                    string lab = gvitem.Rows[RowIndex].Cells[9].Text;
                    Int32 changedQuantity;

                    if (changedRow.FindControl("lblQuantity") is null)
                    {
                        changedQuantity = int.Parse((changedRow.FindControl("txtQuantity") as TextBox)?.Text ?? string.Empty);
                    }
                    else
                    {
                        changedQuantity = int.Parse((changedRow.FindControl("lblQuantity") as Label)?.Text ?? string.Empty);
                    }

                    try
                    {
                        DBOps db = new DBOps();
                        int retval = db.UpdateInventoryTable(item, changedQuantity + 1, lab);
                        lblPageInfo.Text = retval.ToString() + " row updated";
                    }
                    catch (Exception ex)
                    {
                        lblPageInfo.Text = "An error occurred while attempting to update the row." + ex.Message; 
                        //throw ex;
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
                    string lab = gvitem.Rows[RowIndex].Cells[9].Text;
                    Int32 changedQuantity;
                    if (changedRow.FindControl("lblQuantity") is null)
                    {
                        changedQuantity = int.Parse((changedRow.FindControl("txtQuantity") as TextBox)?.Text ?? string.Empty);
                    }
                    else
                    {
                        changedQuantity = int.Parse((changedRow.FindControl("lblQuantity") as Label)?.Text ?? string.Empty);
                    }

                    try
                    {
                        DBOps db = new DBOps();
                        int retval = db.UpdateInventoryTable(item, changedQuantity - 1, lab);
                        lblPageInfo.Text = retval.ToString() + " row updated";
                    }
                    catch (Exception ex)
                    {
                        lblPageInfo.Text = "An error occurred while attempting to update the row. " + ex.Message;
                        //throw;
                    }
                    string callerfunc = new StackFrame(1).GetMethod().Name;
                    RefreshTable(callerfunc);
                    //Change the respective color of the row after incrementing, decrementing or updating
                    CheckRowQuantityColor(int.Parse(e.CommandArgument.ToString()));
                }
                if (e.CommandName.ToLower() == "update")
                {
                    int RowIndex = int.Parse(e.CommandArgument.ToString());
                    GridViewRow changedRow = gvitem.Rows[RowIndex];
                    string item = gvitem.Rows[RowIndex].Cells[1].Text;
                    string lab = gvitem.Rows[RowIndex].Cells[9].Text;
                    Int32 changedQuantity;
                    if (changedRow.FindControl("lblQuantity") is null)
                    {
                        changedQuantity = int.Parse((changedRow.FindControl("txtQuantity") as TextBox)?.Text ?? string.Empty);
                    }
                    else
                    {
                        changedQuantity = int.Parse((changedRow.FindControl("lblQuantity") as Label)?.Text ?? string.Empty);
                    }
                    gvitem_RowUpdatingcustom(item, lab, changedQuantity);
                    //Change the respective color of the row after incrementing, decrementing or updating
                    CheckRowQuantityColor(int.Parse(e.CommandArgument.ToString()));
                }


            }
            catch (Exception ex)
            {
                lblPageInfo.Text = "Error Occured when taking Update Action.";
                lblPageInfo.DataBind();
                //throw;
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
                lblPageInfo.Text = "Error Occured when Deleting Row. " + ex.Message;
                lblPageInfo.DataBind();
                //throw;
            }
            RefreshTable();
        }

        //protected DataTable SortColumnBy(string columnName, Type columnType)
        //{
        //    try
        //    {

        //    }
        //    catch (Exception ex)
        //    {

        //    }
        //}

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
                        if (ViewState["gvitemsort"] != null)
                        {
                            string sortExpression = (string)ViewState["gvitemsort"];
                            if (sortExpression.StartsWith("Lab") ||
                                sortExpression.StartsWith("Category") ||
                                 sortExpression.StartsWith("Item"))
                            {
                                ViewState["gvitemsort"] = "NONE";
                            }
                        }

                        switch ((string)ViewState["gvitemsort"] ?? "NONE")
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
                                /*If the Sort Expression is quantity, lab or item, sort by category ascending*/
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
                            default:
                                /*If the Sort Expression is quantity, lab or item, sort by category ascending*/
                                query2 = from row in ds2.Tables["table"].AsEnumerable()
                                         orderby row.Field<string>("ItemCode")
                                         select row; // Asc query for Melder field;
                                result2 = query2.CopyToDataTable();
                                Session["SortedView"] = result2;

                                gvitem.DataSource = result2;
                                gvitem.DataBind();

                                ViewState["gvitemsort"] = "Category SORTEDASC";
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
                            case "Lab SORTEDDESC":
                                // For the future : - DataTable dt = ((DataSet)ViewState["gvitem"]).Tables["table"]; if storing and reading table rows from view/session state
                                query3 = from row in ds3.Tables["table"].AsEnumerable()
                                         orderby row.Field<string>("lab")
                                         select row;
                                result3 = query3.CopyToDataTable();
                                Session["SortedView"] = result3;

                                gvitem.DataSource = result3;
                                gvitem.DataBind();

                                ViewState["gvitemsort"] = "Lab SORTEDASC";
                                break;
                            case "Lab SORTEDASC":
                                //DataTable dt = ((DataSet)ViewState["gvitem"]).Tables["table"];
                                query3 = from row in ds3.Tables["table"].AsEnumerable()
                                         orderby row.Field<string>("lab") descending
                                         select row;
                                result3 = query3.CopyToDataTable();
                                Session["SortedView"] = result3;

                                gvitem.DataSource = result3;
                                gvitem.DataBind();
                                ViewState["gvitemsort"] = "Lab SORTEDDESC"; //quantity desc/asc for easier use in the refreshtable func
                                break;
                            default:
                                /*If the Sort Expression is quantity, lab or item, sort by category ascending*/
                                query3 = from row in ds3.Tables["table"].AsEnumerable()
                                         orderby row.Field<string>("lab")
                                         select row;
                                result3 = query3.CopyToDataTable();
                                Session["SortedView"] = result3;

                                gvitem.DataSource = result3;
                                gvitem.DataBind();

                                ViewState["gvitemsort"] = "Category SORTEDASC";
                                break;

                        }
                        break;
                }
            }
            catch (Exception ex)
            {
                throw;
            }
        }

        #endregion

        protected void CheckRowQuantityColor(Int32 rowIndex)
        {
            GridViewRow row = gvitem.Rows[rowIndex];
            int quantity = int.Parse((row.FindControl("lblQuantity") as Label)?.Text ?? "0");
            if (quantity < 15)       
            {
                row.BackColor = System.Drawing.Color.Orange;
            }
            if (quantity < 10)
            {
                row.BackColor = System.Drawing.Color.Red;
            }
        }


        protected void gvSearchResult_OnRowCommand(object sender, GridViewCommandEventArgs e)
        {
            if (e.CommandName.ToLower() == "populate")
            {
                //capture row arguments
                int RowIndex = int.Parse(e.CommandArgument.ToString());
                GridViewRow changedRow = gvSearchResult.Rows[RowIndex];
                string itemcodeText = gvSearchResult.Rows[RowIndex].Cells[1].Text;
                string quantityText = gvSearchResult.Rows[RowIndex].Cells[2].Text;
                string labText = gvSearchResult.Rows[RowIndex].Cells[3].Text;
                string descriptionText = gvSearchResult.Rows[RowIndex].Cells[4].Text;
                string categoryText = gvSearchResult.Rows[RowIndex].Cells[5].Text;
                string modelText = gvSearchResult.Rows[RowIndex].Cells[6].Text;
                string warningQuant = gvSearchResult.Rows[RowIndex].Cells[7].Text;
                string alertQuant = gvSearchResult.Rows[RowIndex].Cells[8].Text;

                //push them to insert/update form. 
                txtInsertItem.Text = DBquoteToHTML(itemcodeText);
                txtInsertQuantity.Text = DBquoteToHTML(quantityText);
                txtInsertDescription.Text = DBquoteToHTML(descriptionText);
                ddlInsertLab.SelectedValue = DBquoteToHTML(labText);
                txtInsertCategory.Text = DBquoteToHTML(categoryText);
                txtInsertItemCode.Text = DBquoteToHTML(modelText);
                txtInsertAlertQuant.Text = DBquoteToHTML(alertQuant);
                txtInsertWarningQuant.Text = DBquoteToHTML(warningQuant);
            }
        }

        protected String DBquoteToHTML(String text)
        {
            text = text.Replace("&#39;", "'");
            text = text.Replace("&quot;","\"");
            text = text.Replace("&nbsp;", "");

            return text;
        }
    }
}