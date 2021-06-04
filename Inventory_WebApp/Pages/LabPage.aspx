<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="LabPage.aspx.cs" Inherits="Inventory_WebApp.Pages.LabPage" %>

<!DOCTYPE html>

<html xmlns="http://www.w3.org/1999/xhtml">
<head runat="server">
    <meta charset="utf-8" />
    <meta name="viewport" content="width=device-width, initial-scale=1" />
    <meta http-equiv="X-UA-Compatible" content="IE=edge" />
    <title>Add a New Lab</title>
    <link rel="stylesheet" href="https://cdn.jsdelivr.net/npm/bulma@0.8.2/css/bulma.min.css" />
    <link rel="stylesheet" href="../App_Data/bulma-collapsible.min.css" />
    <script src="../App_Data/bulma-collapsible.min.js"></script>
    <!-- Load Font Awesome 5 -->
    <script defer="" src="https://use.fontawesome.com/releases/v5.8.1/js/all.js"></script>
</head>
<body>
    <form id="form1" runat="server">
        <section class="hero is-primary is-bold">
            <div class="hero-body">
                <div class="container">
                    <h1 class="title">Inventory @ ETS
                    </h1>
                    <h2 class="subtitle">Department of Engineering
                    </h2>
                </div>
            </div>
            <div class="hero-foot is-primary">
            </div>
        </section>
        <div class="tabs">
            <ul>
                <li class="is-active"><a runat="server" href="~/Pages/LabPage.aspx">Labs</a></li>
                <%--<li><a href="ItemsPage.aspx">Items</a></li>--%>
                <li><a runat="server" href="~/Pages/InventoryPage.aspx">Inventory</a></li>
                <%-- <li><a href="SuppliersPage.aspx">Suppliers</a></li>
                <li><a href="DB_Select_Page.aspx">Choose your DB</a></li>--%>
            </ul>
            <a onclick="showHideInsertPane(this);"><i class="fa fa-bars"></i></a>
            <script type="text/javascript">
                function showHideInsertPane(e) {
                    //e is the whole a tage and all its attributes
                    var element = document.getElementById("insert_lab");
                    if (element.style.display == "none") {
                        element.style.removeProperty("display");
                    }
                    else {
                        element.style.display = "none";
                    }
                }
            </script>
        </div>
        <%--        <div class="column" style="direction: rtl;">
        </div>--%>
        <div class="columns has-addons-centered has-text-centered">
            <div class="column">
                <asp:Label ID="lblErr" runat="server" ForeColor="Red"></asp:Label>
                <asp:GridView runat="server" ID="gvitem" CssClass="table"
                    HorizontalAlign="Center"
                    BorderColor="000080"
                    BorderWidth="2px"
                    Width="100%"
                    AutoGenerateColumns="False"
                    AllowPaging="true"
                    AllowSorting="false"
                    PageSize="15"
                    PagerSettings-Position="Bottom"
                    PagerSettings-Mode="Numeric"
                    PagerStyle-HorizontalAlign="Center"
                    PagerSettings-NextPageText="Next"
                    PagerSettings-PreviousPageText="Prev"
                    OnPageIndexChanging="gvitem_PageIndexChanging"
                    AutoGenerateEditButton="true"
                    OnRowCreated="gvitem_RowCreated"
                    OnRowEditing="gvitem_RowEditing"
                    OnRowCancelingEdit="gvitem_RowCancelingEdit"
                              OnRowCommand="gvitem_OnRowCommand"
                              OnRowUpdating="gvitem_OnRowUpdating">
                    <Columns>
                        <asp:ButtonField CommandName="info dialog" Text="<i class='fa fa-info'></i>"
                            ButtonType="Link"
                            ControlStyle-CssClass="btn btn-primary" />
                        <asp:TemplateField HeaderText="Name" HeaderStyle-HorizontalAlign="Center" ItemStyle-VerticalAlign="Middle" ItemStyle-HorizontalAlign="Center">
                            <%--<EditItemTemplate>
                                <asp:TextBox ID="txtName" Text='<%# Bind("Name") %>' runat="server" Style="text-align: center"></asp:TextBox>
                            </EditItemTemplate>--%>
                            <ItemTemplate>
                                <asp:Label ID="lblName" Text='<%# Bind("Name") %>' runat="server" Style="text-align: center"></asp:Label>
                            </ItemTemplate>
                        </asp:TemplateField>
                        <asp:TemplateField HeaderText="Building" HeaderStyle-HorizontalAlign="Center" ItemStyle-VerticalAlign="Middle" ItemStyle-HorizontalAlign="Center">
                            <EditItemTemplate>
                                <asp:TextBox ID="txtBuilding" Text='<%# Bind("Building") %>' runat="server" Style="text-align: center"></asp:TextBox>
                            </EditItemTemplate>
                            <ItemTemplate>
                                <asp:Label ID="lblBuilding" Text='<%# Bind("Building") %>' runat="server" Style="text-align: center"></asp:Label>
                            </ItemTemplate>
                        </asp:TemplateField>
                        <asp:TemplateField HeaderText="Room No." HeaderStyle-HorizontalAlign="Center" ItemStyle-VerticalAlign="Middle" ItemStyle-HorizontalAlign="Center">
                            <EditItemTemplate>
                                <asp:TextBox ID="txtRoomNo" Text='<%# Eval("RoomNo") %>' runat="server" Style="text-align: center"></asp:TextBox>
                            </EditItemTemplate>
                            <ItemTemplate>
                                <asp:Label ID="lblRoomNo" Text='<%# Bind("RoomNo") %>' runat="server" Style="text-align: center"></asp:Label>
                            </ItemTemplate>
                        </asp:TemplateField>
                        <asp:TemplateField HeaderText="Delete" ShowHeader="False">
                                <ItemTemplate>
                                    <asp:LinkButton ID="DeleteButton" runat="server" Text="<i class='fa fa-times'></i>" ButtonType="Link"
                                        CommandName="delete" OnClientClick="return confirm('Are you sure you want to delete this item?');"
                                        AlternateText="Delete" />
                                </ItemTemplate>
                            </asp:TemplateField>
                    </Columns>
                </asp:GridView>
                <asp:Button ID="btnLoad" runat="server" OnClick="btnLoad_Click" Text="Load Table" CssClass="button " />
            </div>
            <div class="column" id="insert_lab" style="display: none">
                <div class="box">
                    <div class="columns">
                        <div class="column">
                            <div class="field">
                                <asp:Label ID="Label1" CssClass="label" runat="server">Lab Name:   </asp:Label>
                                <div class="control">
                                    <asp:TextBox ID="txtInsertLab" CssClass="input" runat="server"></asp:TextBox>
                                </div>
                            </div>
                            <div class="field">
                                <asp:Label ID="Label3" CssClass="label" runat="server">Building:   </asp:Label>
                                <div class="control">
                                    <asp:TextBox ID="txtInsertBuilding" CssClass="input" runat="server"></asp:TextBox>

                                </div>
                            </div>
                            <div class="field">
                                <asp:Label ID="Label2" CssClass="label" runat="server">Classroom:   </asp:Label>
                                <div class="control">
                                    <asp:TextBox ID="txtInsertRoom" CssClass="input" runat="server"></asp:TextBox>

                                </div>
                            </div>
                            <div class="field is-grouped">
                                <div class="control has-text-centered">
                                    <asp:Button ID="btnInsert" runat="server" Text="Insert" OnClick="btnInsert_Click" CssClass="button" />
                                </div>
                                <br />
                                <asp:Label ID="lblInsertInfo" runat="server" CssClass="label" ForeColor="Red"></asp:Label>
                            </div>

                        </div>
                    </div>
                </div>
                <div class="box">
                    <div class="columns">
                        <div class="column">
                            <div class="field">
                                <asp:Label ID="Label4" CssClass="label" runat="server">Lab Name:   </asp:Label>
                                <div class="control">
                                    <asp:TextBox ID="txtUpdateLab" CssClass="input" runat="server"></asp:TextBox>
                                </div>
                            </div>
                            <div class="field">
                                <asp:Label ID="Label5" CssClass="label" runat="server">Building:   </asp:Label>
                                <div class="control">
                                    <asp:TextBox ID="txtUpdateBuilding" CssClass="input" runat="server"></asp:TextBox>
                                </div>
                            </div>
                            <div class="field">
                                <asp:Label ID="Label6" CssClass="label" runat="server">Classroom:   </asp:Label>
                                <div class="control">
                                    <asp:TextBox ID="txtUpdateRoom" CssClass="input" runat="server"></asp:TextBox>
                                </div>
                            </div>

                            <div class="field is-grouped">
                                <div class="control">
                                    <asp:Button ID="btnUpdate" runat="server" Text="Update" OnClick="btnUpdate_Click" CssClass="button" />
                                </div>
                                <asp:Label ID="lblUpdateInfo" CssClass="label" runat="server" ForeColor="Red"></asp:Label>
                            </div>
                        </div>
                    </div>
                </div>
                <div class="box">
                    <div class="field">
                        <asp:Label runat="server" CssClass="label">Column: </asp:Label>
                        <div class="control select">
                            <asp:DropDownList ID="ddlColumn" CssClass="select" runat="server"></asp:DropDownList>
                        </div>
                    </div>
                    <div class="field">
                        <div class="control">
                            <asp:TextBox ID="txtSearchtext" CssClass="input" runat="server"></asp:TextBox>
                        </div>
                    </div>
                    <div class="field is-grouped">
                        <div class="control">
                            <asp:Button runat="server" ID="btnSearch" OnClick="btnSearch_Click" Text="Search" CssClass="button" />
                        </div>
                        <asp:Label ID="lblSearchInfo" CssClass="label" runat="server" ForeColor="Red"></asp:Label>
                    </div>
                    <asp:DataGrid runat="server" ID="dgSearchResult" AllowPaging="true" PageSize="5" CssClass="table"></asp:DataGrid>
                </div>
            </div>
        </div>
    </form>
</body>
</html>
