<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="SupplyPage.aspx.cs" Inherits="Inventory_WebApp.WebForm1" %>

<!DOCTYPE html>

<html xmlns="http://www.w3.org/1999/xhtml">
<head runat="server">
     <meta charset="utf-8"/>
    <meta name="viewport" content="width=device-width, initial-scale=1"/>
    <title>Inventory at ETS</title>
    <link rel="stylesheet" href="https://cdn.jsdelivr.net/npm/bulma@0.8.2/css/bulma.min.css"/>
</head>
<body>
    <section class="hero is-primary is-bold">
      <div class="hero-body">
        <div class="container">
          <h1 class="title">
            Inventory @ ETS
          </h1>
          <h2 class="subtitle">
            Department of Engineering
          </h2>
        </div>
      </div>
    </section>
     <div class="tabs">
          <ul>
            <li class="is-active"><a href="SupplyPage.aspx">Items</a></li>
            <li><a>Inventory</a></li>
            <li><a>Suppliers</a></li>
            <li><a>Departmental Allocations</a></li>
            <li><a href="DB_Select_Page.aspx">Choose your view :- DB</a></li>
          </ul>
        </div>
    <form id="form1" runat="server">
       
        <div>
            <asp:DataGrid runat="server"   ID="dgitem" CssClass="table"></asp:DataGrid>
        </div>
        <asp:Label ID="lblErr" runat="server" ForeColor="Red"></asp:Label>
        <p>
            <asp:Button ID="btnCreate" runat="server" OnClick="Button1_Click" Text="Load Table" CssClass="button green"/>
        </p>
        <br />
        <br />
        <br />
        <p>
            <asp:Label ID="Label1" runat="server">Key:   </asp:Label><asp:TextBox ID="TextBox1" runat="server" ></asp:TextBox>
            <asp:Label ID="Label2" runat="server">Value:   </asp:Label><asp:TextBox ID="TextBox2" runat="server" ></asp:TextBox>
            <br />
            <asp:Button ID="btnInsert" runat="server" Text="Insert" OnClick="btnInsert_Click" CssClass="button green"/>
        </p>
        <p>
            <asp:Label ID="lblkey" runat="server">Key:   </asp:Label><asp:TextBox ID="txtKey" runat="server" ></asp:TextBox>
            <asp:Label ID="lblval" runat="server">Value:   </asp:Label><asp:TextBox ID="txtValue" runat="server" ></asp:TextBox>
            <br />
            <asp:Button ID="btnUpdate" runat="server" Text="Update" OnClick="btnUpdate_Click" CssClass="button green"/>
        </p>
    </form>
</body>
</html>
