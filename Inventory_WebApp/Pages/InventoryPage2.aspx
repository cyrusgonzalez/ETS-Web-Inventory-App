<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="InventoryPage2.aspx.cs" Inherits="Inventory_WebApp.InventoryETS2" %>

<!DOCTYPE html>

<html xmlns="http://www.w3.org/1999/xhtml">
<head runat="server">
    <meta charset="utf-8" />
    <meta name="viewport" content="width=device-width, initial-scale=1" />
    <title>Inventory at ETS</title>
    <link rel="stylesheet" href="https://cdn.jsdelivr.net/npm/bulma@0.8.2/css/bulma.min.css" />
    <script src="../App_Data/bulma-collapsible.js">    </script>
    <link rel="stylesheet" href="../App_Data/bulma-collapsible.css" />
    <!-- Load Font Awesome 5 -->
    <script defer="" src="https://use.fontawesome.com/releases/v5.8.1/js/all.js"></script>
</head>
<body>
    <%--<section  style="height:50px" class="box is-primary">
        <div class="container">
                    <h2 class="title">Inventory @ ETS</h2><h3 class="subtitle">Department of Engineering</h3>
                </div>
    </section>--%>
    <section class="hero is-primary is-bold" style="height: 120px">
        <div class="hero-body">
            <h2 class="title">Inventory @ ETS
            </h2>
            <h3 class="subtitle">Department of Engineering
            </h3>
        </div>
        <div class="hero-foot is-primary">
        </div>
    </section>
    <div class="tabs">
        <ul>
            <li><a href="LabPage.aspx">Labs</a></li>
            <li><a href="ItemsPage.aspx">Items</a></li>
            <li class="is-active"><a href="InventoryPage.aspx">Inventory</a></li>
            <%--<li><a href="SuppliersPage.aspx">Suppliers</a></li>
            <li><a href="DB_Select_Page.aspx">Choose your DB</a></li>--%>
        </ul>
        <a onclick="showHideInsertPane(this);" style="direction: rtl;"><i class="fa fa-bars"></i></a>
        <script type="text/javascript">
            function showHideInsertPane(e) {
                //e is the whole a tage and all its attributes
                var element = document.getElementById("insert_inventory");
                if (element.style.display == "none") {
                    element.style.removeProperty("display");
                }
                else {
                    element.style.display = "none";
                }
            }
        </script>
    </div>

    <form id="form1" runat="server">
        <div class="columns" style="height: 50px">
            <div class="column">
                <asp:Label runat="server"> Lab: </asp:Label>
                <asp:DropDownList ID="ddlLabselect" CssClass="select" runat="server" EnableTheming="true" OnSelectedIndexChanged="ddlLabselect_SelectedIndexChanged" AutoPostBack="true"></asp:DropDownList>
            </div>
            <div class="column">
                <asp:Label ID="lblPageInfo" CssClass="label" runat="server" ForeColor="#0099ff"></asp:Label>
            </div>
        </div>

        <div class="columns ">
            <div class="column box has-text-centered">
                <asp:GridView runat="server" ID="gvitem" CssClass="table"
                    HorizontalAlign="Center"
                    BorderColor="000080"
                    BorderWidth="2px"
                    Width="100%"
                    AutoGenerateColumns="false"
                    AllowPaging="true"
                    AllowSorting="true"
                    PageSize="7"
                    PagerSettings-Position="Bottom"
                    PagerSettings-Mode="Numeric"
                    PagerStyle-HorizontalAlign="Center"
                    PagerSettings-NextPageText="Next"
                    PagerSettings-PreviousPageText="Prev"
                    OnPageIndexChanging="gvitem_PageIndexChanging"
                    OnRowDataBound="gvitem_RowDataBound"
                    AutoGenerateEditButton="true"
                    OnRowEditing="gvitem_RowEditing"
                    OnRowCancelingEdit="gvitem_RowCancelingEdit"
                    OnRowCreated="gvitem_RowCreated"
                    OnRowUpdating="gvitem_RowUpdating"
                    OnRowCommand="gvitem_RowCommand"
                    OnRowDeleting="gvitem_RowDeleting">
                    <Columns>
                        <asp:BoundField HeaderText="ID" DataField="ID" ReadOnly="true" Visible="false" />
                        <asp:BoundField HeaderText="Item" DataField="ItemCode" ReadOnly="true" />
                        <asp:BoundField HeaderText="Model-ItemCode" ReadOnly="true" />
                        <asp:BoundField HeaderText="Description" DataField="" ReadOnly="true" />
                        <asp:BoundField HeaderText="Categories/Tags" DataField="" ReadOnly="true" />
                        <asp:ButtonField CommandName="increment" Text="<i class='fa fa-plus'></i>"
                            ButtonType="Link"
                            ControlStyle-CssClass="btn btn-primary" />
                        <asp:TemplateField HeaderText="Quantity" ItemStyle-Width="50px">
                            <EditItemTemplate>
                                <asp:TextBox ID="txtQuantity" Text='<%# Bind("Quantity") %>' runat="server"></asp:TextBox>
                            </EditItemTemplate>
                            <ItemTemplate>
                                <asp:Label ID="lblQuantity" Text='<%# Bind("Quantity") %>' runat="server"></asp:Label>
                                <%--<asp:Label ID="lblCity" runat="server" Text='<%# Eval("City")%>'></asp:Label>--%>
                            </ItemTemplate>
                        </asp:TemplateField>
                        <asp:ButtonField CommandName="decrement" Text="<i class='fa fa-minus'></i>"
                            ButtonType="Link"
                            ControlStyle-CssClass="btn btn-primary" />
                        <asp:BoundField HeaderText="Quantity" DataField="quantity" ItemStyle-Width="50px" ReadOnly="true" Visible="false" />
                        <asp:BoundField HeaderText="Lab" DataField="lab" ReadOnly="true" />
                        <%--<asp:ButtonField CommandName="edit" Text="<i class='fa fa-edit'></i>"
                            ButtonType="Link"
                            ControlStyle-CssClass="btn btn-primary" />
                        <asp:ButtonField CommandName="update" Text="<i class='fa fa-check'></i>"
                            ButtonType="Link"
                            ControlStyle-CssClass="btn btn-primary" />
                        <asp:ButtonField CommandName="editcancel" Text="<i class='fa fa-times'></i>"
                            ButtonType="Link"
                            ControlStyle-CssClass="btn btn-primary" />--%>
                        <asp:ButtonField CommandName="delete" Text="<i class='fa fa-times'></i>"
                            ButtonType="Link"
                            ControlStyle-CssClass="btn btn-primary" />
                    </Columns>
                </asp:GridView>
                <asp:Label ID="lblErr" runat="server" ForeColor="Red"></asp:Label>
                <%--<asp:Button ID="btnLoad" runat="server" OnClick="btnLoad_Click" Text="Load Table" CssClass="button" />--%>
            </div>
            <div id="insert_inventory" style="display: none" class="column">
                <div class="box">
                    <div class="columns">
                        <div class="column">
                            <div class="field">
                                <asp:Label ID="Label1" CssClass="label" runat="server">Item Name/Code:   </asp:Label>
                                <div class="control">
                                    <asp:TextBox ID="txtInsertItem" CssClass="input" placeholder="Text input" runat="server"></asp:TextBox>
                                </div>
                            </div>
                            <div class="field">
                                <asp:Label ID="Label3" CssClass="label" runat="server">Quantity:   </asp:Label>
                                <div class="control">
                                    <asp:TextBox ID="txtInsertQuantity" class="input" runat="server"></asp:TextBox>
                                </div>
                            </div>
                            <div class="field">
                                <asp:Label ID="Label2" CssClass="label" runat="server">Lab:   </asp:Label>
                                <div class="control select">
                                    <asp:DropDownList ID="ddlInsertLab" runat="server"></asp:DropDownList>
                                </div>
                            </div>
                            <div class="field is-grouped">
                                <div class="control">
                                    <asp:Button ID="btnInsert" runat="server" Text="Insert" OnClick="btnInsert_Click" CssClass="button" />
                                </div>
                                <asp:Label ID="lblInsertInfo" CssClass="label" runat="server" ForeColor="Red"></asp:Label>
                            </div>
                        </div>
                    </div>

                </div>

                <div class="box has-text-centered" style="display: none;">
                    <asp:Label ID="lblkey" runat="server">Key:   </asp:Label><asp:TextBox ID="txtKey" runat="server"></asp:TextBox>
                    <asp:Label ID="lblval" runat="server">Value:   </asp:Label><asp:TextBox ID="txtValue" runat="server"></asp:TextBox>
                    <asp:Button ID="btnUpdate" runat="server" Text="Update" OnClick="btnUpdate_Click" CssClass="button" />
                    <asp:Label ID="lblUpdateInfo" runat="server" ForeColor="Red"></asp:Label>
                </div>
                <div class="box message" id="search_inventory">
                    <div class="message-header">
                        <h4>Search</h4>
                    </div>
                    <div class="message-body">
                        <div class="columns">
                            <div class="column">
                                <div class="field">
                                    <asp:Label CssClass="label" runat="server">Column: </asp:Label>
                                    <div class="control select">
                                        <asp:DropDownList ID="ddlColumn" runat="server"></asp:DropDownList>
                                    </div>
                                </div>
                                <%--<div class="field">
                                        <asp:Label ID="Label6" runat="server">Lab:   </asp:Label>
                                        <div class="control select">
                                            <asp:DropDownList ID="ddlSearchInventory" runat="server"></asp:DropDownList>
                                        </div>
                                    </div>--%>
                            </div>
                            <div class="column">
                                <div class="field">
                                    <asp:Label ID="Label4" CssClass="label" runat="server">Value:   </asp:Label>
                                    <div class="control">
                                        <asp:TextBox ID="txtSearchtext" CssClass="input" runat="server"></asp:TextBox>
                                    </div>
                                </div>
                            </div>
                        </div>
                        <div class="field is-grouped">
                            <div class="control">
                                <asp:DataGrid runat="server" ID="dgSearchResult" AllowPaging="true" PageSize="5" CssClass="table"></asp:DataGrid>
                            </div>
                            <asp:Label ID="Label7" runat="server" ForeColor="Red"></asp:Label>
                        </div>
                        <div class="field">
                            <asp:Label ID="lblSearchInfo" runat="server" ForeColor="Red"></asp:Label>
                            <div class="control">
                                <asp:Button runat="server" ID="btnSearch" OnClick="btnSearch_Click" Text="Search" CssClass="button" />
                            </div>
                        </div>
                    </div>
                </div>
            </div>
        </div>
        <div class="modal">
            <div class="modal-background"></div>
            <div class="modal-content">
                <div style="align-content:center">
                    <p>
                        <asp:Label Text="Are you sure you want to delete this row?" runat="server" CssClass="label"></asp:Label>
                </p>
                <asp:Button Text="Ok" runat="server" ID="btnconfimDelete" CssClass="button" />
                </div>
                
            </div>
            <button class="modal-close is-large" aria-label="close"></button>
        </div>
    </form>
    <script></script>
</body>
</html>
