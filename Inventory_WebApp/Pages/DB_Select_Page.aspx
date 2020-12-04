<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="DB_Select_Page.aspx.cs" Inherits="Inventory_WebApp.Pages.WebForm2" %>

<!DOCTYPE html>

<html xmlns="http://www.w3.org/1999/xhtml">
<head runat="server">
    <meta charset="utf-8" />
    <meta name="viewport" content="width=device-width, initial-scale=1" />
    <title>Inventory at ETS</title>
    <link rel="stylesheet" href="https://cdn.jsdelivr.net/npm/bulma@0.8.2/css/bulma.min.css" />
</head>
<body>
    <form id="form1" runat="server">
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
                        <li><a href="ItemsPage.aspx">Items</a></li>
                        <li><a href="InventoryPage.aspx">Inventory</a></li>
                        <li><a href="SuppliersPage.aspx">Suppliers</a></li>
                        <li class="is-active"><a href="DB_Select_Page.aspx">Choose your DB</a></li>
                    </ul>
                </div>
        <div>
            <div>
                <div>
                    <asp:Button ID="btnLoad" runat="server" OnClick="btnLoad_Click" Text="Load Table" CssClass="button green" />
                    <asp:DropDownList ID="ddlDB" runat="server">
                    </asp:DropDownList>
                    <br />
                    <asp:DataGrid runat="server" ID="dgRecords" CssClass="table"></asp:DataGrid>
                    <br />
                    <asp:Label ID="lblErr" runat="server" ForeColor="Red"></asp:Label>
                </div>
            </div>
            <p>
                &nbsp;
            </p>
        </div>
    </form>
</body>
</html>
