using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Configuration;
using System.Web.Security;

namespace WebApplication1
{
    public partial class Login : System.Web.UI.Page
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            errorCreds.Visible = false;
            errorUser.Visible = false;
            errorPass.Visible = false;
        }

        protected void Clk_Login_Click(object sender, EventArgs E)
        {
            //FIXME!!! MAKE SURE THE CONNECTION IS SET TO A PROJECT FILE FOR READING ON IPAD OR ON THE COMPUTER SERVERF DIRECTORY
            //SqlConnection sqlconn = new SqlConnection(@"Data Source=.\; AttachDbFilename= C:\Users\cyrusgon\source\repos\WebApplication1\WebApplication1\App_Data\Database1.mdf; Security = True");
            //SqlCommand command = new SqlCommand();
            //SqlDataAdapter dataAdap = new SqlDataAdapter();
            //DataSet set = new DataSet();
            string adminUser = "administrator";
            string adminPass = "ENSHelpDesk4912917";
            string userN = TextBoxUser.Text;
            string passW = TextBoxPass.Text;
            if(userN == adminUser && passW == adminPass)
            {
                Session["username"] = TextBoxUser.Text.Trim();
                Response.Redirect("InventoryPage.aspx");
            } 
            else 
            {
                errorCreds.Visible = true;
            }
        }
    }
}