<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="SupplyPage.aspx.cs" Inherits="Inventory_WebApp.WebForm1" %>

<!DOCTYPE html>

<html xmlns="http://www.w3.org/1999/xhtml">
<head runat="server">
    <title></title>
</head>
<body>
    <form id="form1" runat="server">
        <div>
            <asp:DataGrid
                runat="server"
                ID="dgitem"></asp:DataGrid>
        </div>
        <asp:Label ID="lblErr" runat="server" ForeColor="Red"></asp:Label>
        <p>
            <asp:Button ID="btnCreate" runat="server" OnClick="Button1_Click" Text="Create Table" />
        </p>
        <p>
            <asp:Label ID="Label1" runat="server">Key:   </asp:Label><asp:TextBox ID="TextBox1" runat="server" ></asp:TextBox>
            <asp:Label ID="Label2" runat="server">Value:   </asp:Label><asp:TextBox ID="TextBox2" runat="server" ></asp:TextBox>
            <br />
            <asp:Button ID="btnInsert" runat="server" Text="Insert" OnClick="btnInsert_Click"/>
        </p>
        <p>
            <asp:Label ID="lblkey" runat="server">Key:   </asp:Label><asp:TextBox ID="txtKey" runat="server" ></asp:TextBox>
            <asp:Label ID="lblval" runat="server">Value:   </asp:Label><asp:TextBox ID="txtValue" runat="server" ></asp:TextBox>
            <br />
            <asp:Button ID="btnUpdate" runat="server" Text="Update" OnClick="btnUpdate_Click"/>
        </p>
    </form>
</body>
</html>
