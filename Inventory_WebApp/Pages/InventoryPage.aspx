<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="InventoryPage.aspx.cs" Inherits="Inventory_WebApp.Pages.InventoryETS" %>

<!DOCTYPE html>

<html xmlns="http://www.w3.org/1999/xhtml">
<head runat="server">
    <meta charset="utf-8" />
    <meta name="viewport" content="width=device-width, initial-scale=1" />
    <meta http-equiv="X-UA-Compatible" content="IE=edge" />
    <!-- The above 3 meta tags *must* come first in the head; any other head content must come *after* these tags -->
    
    <title>Inventory at ETS</title>
    
    <%--Bulma and Bulma extensions--%>
    <link rel="stylesheet" href="https://cdn.jsdelivr.net/npm/bulma@0.8.2/css/bulma.min.css" />
    <link rel="stylesheet" href="../dependencies/bulma-extensions.min.css" />
    <link rel="stylesheet" href="StyleSheet1.css"/>
    <script src="https://cdn.jsdelivr.net/npm/bulma-extensions@6.2.7/dist/js/bulma-extensions.min.js"></script>

    <!-- Load Font Awesome 5 -->
    <script defer="" src="https://use.fontawesome.com/releases/v5.8.1/js/all.js"></script>
</head>
<body>
    <section class="hero is-primary is-bold">
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
            <li><a runat="server" href="~/Pages/LabPage.aspx">Labs</a></li>
            <li class="is-active"><a runat="server" href="~/Pages/InventoryPage.aspx">Inventory</a></li>
        </ul>
        <a onclick="showHideInsertPane(this);" style="direction: rtl;"><i class="fa fa-bars"></i></a>
        <script type="text/javascript">
            function showHideInsertPane(e) {
                //e is the whole a tag and all its attributes
                var element = document.getElementById("insert_inventory");
                var searchbar = document.getElementById('<%=HiddenFieldShowHideSearchPanel.ClientID%>');

                if (element.style.display === "none") {
                    element.style.removeProperty("display");
                    searchbar.value = "Show";
                }
                else {
                    element.style.display = "none";
                    searchbar.value = "Hidden";
                }
            }
        </script>
    </div>

    <form id="form1" runat="server">
        <section runat="server">
            <div class="container is-fluid">
                <nav class="level">
                    <div class="level-item has-text-centered">
                        <div class="field is-horizontal">
                            <div class="field-label">
                                <asp:Label runat="server" CssClass="label" ID="Label12"> Lab: </asp:Label>
                            </div>
                            <div class="field-body">
                                <div class="control">
                                    <div class="select is-one-third-mobile">
                                        <asp:DropDownList ID="ddlLabselect" CssClass="" runat="server" EnableTheming="true" OnSelectedIndexChanged="ddlLabselect_SelectedIndexChanged" AutoPostBack="true"></asp:DropDownList>
                                    </div>
                                </div>
                            </div>
                        </div>
                    </div>
                    <div class="level-item has-text-centered">
                        <div class="field is-horizontal">
                            <div class="field-label">
                                <asp:Label runat="server" CssClass="label" ID="Label10"> Category: </asp:Label>
                            </div>
                            <div class="field-body">
                                <div class="control">
                                    <div class="select">
                                        <asp:DropDownList ID="ddlCategorySelect" CssClass="" runat="server" EnableTheming="true" OnSelectedIndexChanged="ddlCategorySelect_SelectedIndexChanged" AutoPostBack="true"></asp:DropDownList>
                                    </div>
                                </div>
                            </div>
                        </div>
                    </div>
                    <div class="level-item has-text-centered">
                        <div class="field is-horizontal">
                            <div class="field-label">
                                <asp:Label runat="server" CssClass="label" ID="Label5"> Item: </asp:Label>
                            </div>
                            <div class="field-body">
                                <div class="control">
                                    <div class="select is-flex">
                                        <asp:DropDownList ID="ddlItemSelect" CssClass="" runat="server" EnableTheming="true" OnSelectedIndexChanged="ddlItemSelect_SelectedIndexChanged" AutoPostBack="true"></asp:DropDownList>
                                    </div>
                                </div>
                            </div>
                        </div>
                    </div>
                    <div class="level-item">
                        <asp:Label runat="server" ID="lblPageInfo" CssClass="label" ForeColor="#0099ff"></asp:Label>
                    </div>
                </nav>

                <div class="field is-horizontal">
                    <div class="field is-horizontal">
                        <div class="field-body">
                            <div class="field is-narrow">
                            </div>
                        </div>
                    </div>
                </div>
            </div>
        </section>
        <div class="columns">
            <div class="column box has-text-centered">
                <div class="table-responsive">
                    <asp:GridView runat="server" ID="gvitem" CssClass="table  is-mobile"
                        HorizontalAlign="Center"
                        BorderColor="000080"
                        BorderWidth="2px"
                        Width="100%"
                        AutoGenerateColumns="false"
                        AllowPaging="true"
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
                        OnRowDeleting="gvitem_RowDeleting"
                        DataKeyNames="ItemCode,Model,lab,warning_quantity,alert_quantity"
                        AllowSorting="true"
                        OnSorting="gvitem_Sorting">
                        <Columns>
                            <asp:BoundField HeaderText="ID" DataField="ID" ReadOnly="true" Visible="false" />
                            <asp:BoundField HeaderText="Item" DataField="ItemCode" ReadOnly="true" SortExpression="Item" />
                            <asp:BoundField HeaderText="Model" DataField="model" ReadOnly="true">
                                <HeaderStyle CssClass="is-hidden-mobile"></HeaderStyle>
                                <ItemStyle CssClass="is-hidden-mobile"></ItemStyle>
                            </asp:BoundField>
                            <asp:BoundField HeaderText="Description" DataField="description" ReadOnly="true" >
                            <HeaderStyle CssClass="is-hidden-mobile"></HeaderStyle>
                                <ItemStyle CssClass="is-hidden-mobile"></ItemStyle>
                            </asp:BoundField>
                            <asp:BoundField HeaderText="Categories/Tags" DataField="category" ReadOnly="true" SortExpression="Category" >
                                <HeaderStyle CssClass="is-hidden-mobile"></HeaderStyle>
                                <ItemStyle CssClass="is-hidden-mobile"></ItemStyle>
                                </asp:BoundField>
                            <asp:ButtonField CommandName="increment" Text="<i class='fa fa-plus'></i>"
                                ButtonType="Link"
                                ControlStyle-CssClass="btn btn-primary" />
                            <asp:TemplateField HeaderText="Quantity" ItemStyle-VerticalAlign="Middle" ItemStyle-HorizontalAlign="Center" ItemStyle-Width="40px" SortExpression="Quantity">
                                <EditItemTemplate>
                                    <asp:TextBox ID="txtQuantity" Text='<%# Bind("Quantity") %>' runat="server" Style="text-align: center"></asp:TextBox>
                                </EditItemTemplate>
                                <ItemTemplate>
                                    <asp:Label ID="lblQuantity" Text='<%# Bind("Quantity") %>' runat="server" Style="text-align: center"></asp:Label>
                                    <%--<asp:Label ID="lblCity" runat="server" Text='<%# Eval("City")%>'></asp:Label>--%>
                                </ItemTemplate>
                            </asp:TemplateField>
                            <asp:ButtonField CommandName="decrement" Text="<i class='fa fa-minus'></i>"
                                ButtonType="Link"
                                ControlStyle-CssClass="btn btn-primary" />
                            <asp:BoundField HeaderText="Lab" DataField="lab" ReadOnly="true" SortExpression="Lab" />
                            <asp:TemplateField ShowHeader="False">
                                <ItemTemplate>
                                    <asp:LinkButton ID="DeleteButton" runat="server" Text="<i class='fa fa-times'></i>" ButtonType="Link"
                                        CommandName="delete" OnClientClick="return confirm('Are you sure you want to delete this item?');"
                                        AlternateText="Delete" />
                                </ItemTemplate>
                            </asp:TemplateField>
                            <asp:BoundField HeaderText="Warning Quantity" DataField="warning_quantity" ReadOnly="true">
                                <HeaderStyle CssClass="is-hidden-mobile"></HeaderStyle>
                                <ItemStyle CssClass="is-hidden-mobile"></ItemStyle>
                            </asp:BoundField>
                            <asp:BoundField HeaderText="Alert Quantity" DataField="alert_quantity" ReadOnly="true" >
                                <HeaderStyle CssClass="is-hidden-mobile"></HeaderStyle>
                                <ItemStyle CssClass="is-hidden-mobile"></ItemStyle>
                            </asp:BoundField>
                        </Columns>
                    </asp:GridView>
                </div>
                <script type="text/javascript">

                    function confirmDelete() {
                        if (confirm("Are you sure you want to delete this?") === true)
                            return true;
                        else
                            return false;
                    }

                </script>
                <asp:Label ID="lblErr" runat="server" ForeColor="Red"></asp:Label>
                <asp:Button ID="btnLoad" runat="server" OnClick="btnLoad_Click" Text="Load Table" CssClass="button" />
            </div>
            <div id="insert_inventory" style="display: none; overflow: auto" class="column" runat="server">
                <div class="box">
                    <div class="columns">
                        <div class="column is-half">
                            <div class="field">
                                <asp:Label ID="Label1" CssClass="label" runat="server">Item Name:   </asp:Label>
                                <div class="control">
                                    <asp:TextBox ID="txtInsertItem" CssClass="input" placeholder="HDMI Cable" runat="server"></asp:TextBox>
                                </div>
                            </div>
                            <div class="field">
                                <asp:Label ID="Label3" CssClass="label" runat="server">Quantity:   </asp:Label>
                                <div class="control">
                                    <asp:TextBox ID="txtInsertQuantity" class="input" runat="server" placeholder="40"></asp:TextBox>
                                </div>
                            </div>
                            <div class="field">
                                <asp:Label ID="Label2" CssClass="label" runat="server">Lab:   </asp:Label>
                                <div class="control select">
                                    <asp:DropDownList ID="ddlInsertLab" runat="server"></asp:DropDownList>
                                </div>
                            </div>
                            <div class="field">
                                <asp:Label ID="Label6" CssClass="label" runat="server">Warning Quantity:   </asp:Label>
                                <div class="control">
                                    <asp:TextBox ID="txtInsertWarningQuant" CssClass="input" placeholder="15" runat="server"></asp:TextBox>
                                </div>
                            </div>
                        </div>
                        <div class="column">
                            <div class="field">
                                <asp:Label ID="Label9" CssClass="label" runat="server">Model:   </asp:Label>
                                <div class="control">
                                    <asp:TextBox ID="txtInsertItemCode" CssClass="input" placeholder="TN4956" runat="server"></asp:TextBox>
                                </div>
                            </div>
                            <div class="field">
                                <asp:Label ID="Label8" CssClass="label" runat="server">Description:   </asp:Label>
                                <div class="control">
                                    <asp:TextBox ID="txtInsertDescription" CssClass="input" placeholder="6 ft HDMI Cable" runat="server"></asp:TextBox>
                                </div>
                            </div>
                            <div class="field">
                                <asp:Label ID="Label11" CssClass="label" runat="server">Category:   </asp:Label>
                                <div class="control">
                                    <asp:TextBox ID="txtInsertCategory" CssClass="input" placeholder="Cables" runat="server"></asp:TextBox>
                                </div>
                            </div>
                            <div class="field">
                                <asp:Label ID="Label13" CssClass="label" runat="server">Alert Quantity:   </asp:Label>
                                <div class="control">
                                    <asp:TextBox ID="txtInsertAlertQuant" CssClass="input" placeholder="10" runat="server"></asp:TextBox>
                                </div>
                            </div>
                        </div>
                    </div>
                    <div class="field is-grouped">
                        <div class="control">
                            <asp:Button ID="btnInsert" runat="server" Text="Insert/Update" OnClick="btnInsert_Click" CssClass="button is-link is-light" />
                        </div>
                        <asp:Label ID="lblInsertInfo" CssClass="label" runat="server" ForeColor="Red"></asp:Label>
                    </div> 
                </div>

                <div id="UpdateDiv" class="box has-text-centered" style="display: none;">
                    <asp:Label ID="lblkey" runat="server">Key:   </asp:Label><asp:TextBox ID="txtKey" runat="server"></asp:TextBox>
                    <asp:Label ID="lblval" runat="server">Value:   </asp:Label><asp:TextBox ID="txtValue" runat="server"></asp:TextBox>
                    <asp:Button ID="btnUpdate" runat="server" Text="Update" OnClick="btnUpdate_Click" CssClass="button" />
                    <asp:Label ID="lblUpdateInfo" runat="server" ForeColor="Red"></asp:Label>
                </div>
                <div class="box message" style="overflow: auto" id="search_inventory">
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
                                <asp:GridView runat="server" ID="gvSearchResult" AllowPaging="True" PageSize="5" CssClass="table is-mobile is-half-desktop" AutoGenerateColumns="False" OnRowCommand="gvSearchResult_OnRowCommand">
                                    <Columns>
                                        <asp:BoundField HeaderText="ID" DataField="ID" ReadOnly="true" Visible="false" >
                                            <HeaderStyle CssClass="is-hidden-mobile"></HeaderStyle>
                                            <ItemStyle CssClass="is-hidden-mobile"></ItemStyle>
                                        </asp:BoundField>
                                        <asp:BoundField HeaderText="Item" DataField="ItemCode" ReadOnly="true" >
                                            <HeaderStyle CssClass=""></HeaderStyle>
                                            <ItemStyle CssClass=""></ItemStyle>
                                        </asp:BoundField>
                                        <asp:BoundField HeaderText="Quantity" DataField="quantity" ReadOnly="true"  >
                                            <HeaderStyle CssClass=""></HeaderStyle>
                                            <ItemStyle CssClass=""></ItemStyle>
                                        </asp:BoundField>
                                        <asp:BoundField HeaderText="Lab" DataField="lab" ReadOnly="true" >
                                            <HeaderStyle CssClass=""></HeaderStyle>
                                            <ItemStyle CssClass="" Width="40px"></ItemStyle>
                                        </asp:BoundField>
                                        <asp:BoundField HeaderText="Description" DataField="description" ReadOnly="true" >
                                            <HeaderStyle CssClass="is-hidden-mobile"></HeaderStyle>
                                            <ItemStyle CssClass="is-hidden-mobile" Width="40px"></ItemStyle>
                                        </asp:BoundField>
                                        <asp:BoundField HeaderText="Category" DataField="category" ReadOnly="true" >
                                            <HeaderStyle CssClass="is-hidden-mobile"></HeaderStyle>
                                            <ItemStyle CssClass="is-hidden-mobile"></ItemStyle>
                                        </asp:BoundField>
                                        <asp:BoundField HeaderText="Model" DataField="model" ReadOnly="true">
                                            <HeaderStyle CssClass="is-hidden-mobile"></HeaderStyle>
                                            <ItemStyle CssClass="is-hidden-mobile"></ItemStyle>
                                        </asp:BoundField>
                                        <asp:BoundField HeaderText="Warning Quant" DataField="warning_quantity" ReadOnly="true">
                                            <HeaderStyle CssClass="is-hidden-mobile" Width="40px"></HeaderStyle>
                                            <ItemStyle CssClass="is-hidden-mobile"></ItemStyle>
                                        </asp:BoundField>
                                        <asp:BoundField HeaderText="Alert Quant" DataField="alert_quantity" ReadOnly="true">
                                            <HeaderStyle CssClass="is-hidden-mobile" Width="40px"></HeaderStyle>
                                            <ItemStyle CssClass="is-hidden-mobile"></ItemStyle>
                                        </asp:BoundField>
                                        <asp:ButtonField CommandName="populate" Text="<i class='fa fa-file-upload'></i>"
                                                         ButtonType="Link"
                                                         ControlStyle-CssClass="btn btn-primary" />
                                    </Columns>
                                </asp:GridView>
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
    
        <asp:HiddenField ID="HiddenFieldShowHideSearchPanel" runat="server" Value="Hidden" />  


    </form>

</body>
</html>
