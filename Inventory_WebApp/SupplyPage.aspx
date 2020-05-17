<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="SupplyPage.aspx.cs" Inherits="Inventory_WebApp.WebForm1" %>

<!DOCTYPE html>

<html xmlns="http://www.w3.org/1999/xhtml">
<head runat="server">
    <title></title>
</head>
<body>
    <form id="form1" runat="server">
        <div>
            <asp:TextBox ID="txtText" runat="server"></asp:TextBox>
            <asp:DataGrid
                runat="server"
                ID="dgitem"></asp:DataGrid>
        </div>
        <asp:TextBox ID="txtErr" runat="server" ForeColor="Red">Test Error</asp:TextBox>
        <p>
            <asp:Button ID="btnFill" runat="server" OnClick="Button1_Click" Text="Fill Grid" />
        </p>
    </form>
</body>
</html>
