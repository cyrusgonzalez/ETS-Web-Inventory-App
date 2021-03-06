<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="ItemsPage.aspx.cs" Inherits="Inventory_WebApp.Pages.ItemsPage" %>

<!DOCTYPE html>

<html xmlns="http://www.w3.org/1999/xhtml">
<head runat="server">
    <meta charset="utf-8" />
    <meta name="viewport" content="width=device-width, initial-scale=1" />
    <title>Items at ETS</title>
    <link rel="stylesheet" href="https://cdn.jsdelivr.net/npm/bulma@0.8.2/css/bulma.min.css" />
</head>
<body>
    <section class="hero is-primary is-bold">
        <div class="hero-body">
            <div class="container">
                <h1 class="title">Inventory @ ETS</h1>
                <h2 class="subtitle">Department of Engineering</h2>
            </div>
        </div>
        <div class="hero-foot">
        </div>
    </section>
    <div class="tabs">
        <ul>
            <li><a href="LabPage.aspx">Labs</a></li>
            <li class="is-active"><a href="ItemsPage.aspx">Items</a></li>
            <li><a href="InventoryPage.aspx">Inventory</a></li>
            <li><a href="SuppliersPage.aspx">Suppliers</a></li>
            <li><a href="DB_Select_Page.aspx">Choose your DB</a></li>
            <li><a href="../Temp/leaflet.drawplugin.html">Secret Dev</a></li>
        </ul>
    </div>
    <form id="form1" runat="server">

        <div class="columns ">
            <div class="column box has-text-centered">
                <asp:DataGrid runat="server" ID="dgitem" CssClass="table"
                    HorizontalAlign="Center"
                    BorderColor="000080"
                    BorderWidth="2px"
                    Width="100%"
                    AllowSorting="True"
                    AllowPaging="True"
                    PageSize="5"
                    PagerStyle-Mode="NumericPages"
                    PagerStyle-PageButtonCount="5"
                    PagerStyle-Position="Bottom"
                    PagerStyle-HorizontalAlign="Center"
                    PagerStyle-NextPageText="Next"
                    PagerStyle-PrevPageText="Prev" OnPageIndexChanged="dgitem_PageIndexChanged">
                </asp:DataGrid>

                <asp:Label ID="lblErr" runat="server" ForeColor="Red"></asp:Label>
                <asp:Button ID="btnLoad" runat="server" OnClick="btnLoad_Click" Text="Load Table" CssClass="button green" />
            </div>
            <div class="column">
                <div class="box has-text-centered">
                    <asp:Label ID="Label1" runat="server">Key:   </asp:Label><asp:TextBox ID="txtInsertKey" runat="server"></asp:TextBox>
                    <asp:Label ID="Label2" runat="server">Value:   </asp:Label><asp:TextBox ID="txtInsertValue" runat="server"></asp:TextBox>
                    <asp:Button ID="btnInsert" runat="server" Text="Insert" OnClick="btnInsert_Click" CssClass="button green" />
                    <br />
                    <asp:Label ID="lblInsertInfo" runat="server" ForeColor="Red"></asp:Label>
                </div>
                <div class="box has-text-centered">
                    <asp:Label ID="lblkey" runat="server">Key:   </asp:Label><asp:TextBox ID="txtKey" runat="server"></asp:TextBox>
                    <asp:Label ID="lblval" runat="server">Value:   </asp:Label><asp:TextBox ID="txtValue" runat="server"></asp:TextBox>
                    <asp:Button ID="btnUpdate" runat="server" Text="Update" OnClick="btnUpdate_Click" CssClass="button green" />
                    <asp:Label ID="lblUpdateInfo" runat="server" ForeColor="Red"></asp:Label>
                </div>
            </div>
        </div>
        <br />
        <asp:Button ID="Clk_Logout" runat="server" Text="Logout" OnClick="Clk_Logout_Click" />
    </form>
</body>
</html>
