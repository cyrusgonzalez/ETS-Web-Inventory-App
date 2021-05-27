using System;
using System.Data;
using System.Linq;
using System.Web.UI.WebControls;
using Inventory_WebApp.DatabaseInterface;

namespace Inventory_WebApp.Pages
{
    public partial class LabPage : System.Web.UI.Page
    {
        DataSet searchColumns = new DBOps().GetLabColumns();
        protected void Page_Load(object sender, EventArgs e)
        {
            //First time page load commands to go inside this if block
            if (!IsPostBack)
            {
                try
                {
                    PopulateSearchColumnsDropdown();
                    RefreshTable();
                }
                catch (Exception ex)
                {
                    lblErr.Text = ex.ToString();
                    lblErr.DataBind();
                    //throw ex;
                }
            }
        }

        protected void btnLoad_Click(object sender, EventArgs e)
        {
            RefreshTable();
        }

        protected void RefreshTable()
        {
            DBOps db = new DBOps();

            DataSet ds = db.ReadLabsTable();

            gvitem.DataSource = ds;
            gvitem.DataBind();

        }

        protected void PopulateSearchColumnsDropdown()
        {
            DBOps db = new DBOps();
            DataSet ds = db.GetLabColumns();
            ddlColumn.DataSource = ds;
            ddlColumn.DataTextField = "name";
            ddlColumn.DataValueField = "name";
            ddlColumn.DataBind();
        }

        protected void btnInsert_Click(object sender, EventArgs e)
        {
            DBOps db = new DBOps();

            string _key = txtInsertLab.Text.Trim();
            string bldg = txtInsertBuilding.Text.Trim();
            string _value = txtInsertRoom.Text.Trim();

            if ((_key != String.Empty && _key != "") || (bldg != String.Empty && bldg != "") || (_value != String.Empty && _value != "") )
            {
                int retval = db.InsertLabsTable(_key, bldg, _value);
                if (retval > 0)
                {
                    txtInsertLab.Text = "";
                    txtInsertBuilding.Text = "";
                    txtInsertRoom.Text = "";
                    lblInsertInfo.ForeColor = System.Drawing.Color.LightSeaGreen;
                    lblInsertInfo.Text = retval.ToString() + " row inserted";
                    lblInsertInfo.DataBind();
                }
            }
            else
            {
                lblErr.Text = "Please Enter all necessary fields before clicking on \"Insert\" ";
                lblErr.DataBind();
            }

            

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

        protected void gvitem_PageIndexChanging(object sender, GridViewPageEventArgs e)
        {
            gvitem.PageIndex = e.NewPageIndex;
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

            }
            catch (Exception ex)
            {
                throw;
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

        protected Type GetMyType(string SQLiteType)
        {
            Type returnType;
            
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


        protected void gvitem_OnRowCommand(object sender, GridViewCommandEventArgs e)
        {
            if (e.CommandName.ToLower() == "update")
            {
                int RowIndex = int.Parse(e.CommandArgument.ToString());
                GridViewRow changedRow = gvitem.Rows[RowIndex];
                string lab, building, roomnumber;

                if (changedRow.FindControl("lblName") is null)
                {
                    lab = ((changedRow.FindControl("txtName") as TextBox)?.Text ?? string.Empty); //Not possible since Name is PK. Can't edit this field anymore.
                }
                else
                {
                    lab = ((changedRow.FindControl("lblName") as Label)?.Text ?? string.Empty);
                }
                if (changedRow.FindControl("lblBuilding") is null)
                {
                    building = ((changedRow.FindControl("txtBuilding") as TextBox)?.Text ?? string.Empty);
                }
                else
                {
                    building = ((changedRow.FindControl("lblBuilding") as Label)?.Text ?? string.Empty);
                }
                if (changedRow.FindControl("lblRoomNo") is null)
                {
                    roomnumber = ((changedRow.FindControl("txtRoomNo") as TextBox)?.Text ?? string.Empty);
                }
                else
                {
                    roomnumber = ((changedRow.FindControl("lblRoomNo") as Label)?.Text ?? string.Empty);
                }
                int retval = gvitem_RowUpdatingcustom(lab ,building , roomnumber);
                if (retval > 0)
                {
                    
                }
                else
                {
                    lblErr.Text = "There may have been an issue when updating the row.";
                    lblErr.DataBind();
                }
            }
            gvitem.EditIndex = -1;
            RefreshTable();
        }
        protected int gvitem_RowUpdatingcustom(string lab, string building, string roomnumber)
        {
            DBOps db = new DBOps();

            string _key = lab;
            string _value = building;
            string room = roomnumber;

            int retval = db.UpdateLabsTable(_key, _value, room);
            lblErr.Text = retval.ToString() + " row updated";
            lblErr.DataBind();

            return retval;
        }

        protected void gvitem_OnRowUpdating(object sender, GridViewUpdateEventArgs e)
        {
            //placeholder method to catch and handle event. Actual updating done in gvitem_RowUpdatingcustom
        }
    }
}