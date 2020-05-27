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
                    DBOps db = new DBOps();
                    //db.CheckVersion();
                    //db.InitConfig();
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
            db.CreateTable();
            RefreshTable();
        }

        protected void RefreshTable()
        {
            DBOps db = new DBOps();
            
            DataSet ds = db.ReadTable();
            dgitem.DataSource = ds;
            dgitem.DataBind();
                        
        }

        protected void btnInsert_Click(object sender, EventArgs e)
        {
            DBOps db = new DBOps();
            
            string _key = txtInsertKey.Text;
            long _value = long.Parse(txtInsertValue.Text);

            int retval = db.InsertInventoryTable(_key,_value);
            lblInsertInfo.Text = retval.ToString() + " rows inserted";
            lblInsertInfo.DataBind();

            RefreshTable();
        }

        protected void btnUpdate_Click(object sender, EventArgs e)
        {
            DBOps db = new DBOps();
            
            string _key = txtKey.Text;
            long _value = long.Parse(txtValue.Text);

            int retval = db.UpdateInventoryTable(_key,_value);
            lblUpdateInfo.Text = retval.ToString() + " rows updated";
            lblUpdateInfo.DataBind();

            RefreshTable();
        }

        protected void dgitem_PageIndexChanged(object source, DataGridPageChangedEventArgs e)
        {
            dgitem.CurrentPageIndex = e.NewPageIndex;
            RefreshTable();

            //Dim currentSortExpression As String
            //            Dim currentSortOrder As enuSortOrder

            //            'set new page index and rebind the data
            //            dgBooks.CurrentPageIndex = e.NewPageIndex

            //            'get the current sort expression and order from the viewstate
            //            currentSortExpression = CStr(viewstate(VS_CURRENT_SORT_EXPRESSION))
            //            currentSortOrder = CType(viewstate(VS_CURRENT_SORT_ORDER), enuSortOrder)

            //            'rebind the data in the datagrid
            //            bindData(currentSortExpression, _
            //                     currentSortOrder)

        }

        //private Enum enuSortOrder {
        //    soAscending = 0
        //    soDescending = 1
        //}
    }
}