<%@ Page Title="" Language="C#" MasterPageFile="~/Site1.Master" AutoEventWireup="true" CodeBehind="MasterPageExperiment.aspx.cs" Inherits="Inventory_WebApp.WebForm3" %>
<asp:Content ID="Content1" ContentPlaceHolderID="head" runat="server">
    <link rel="stylesheet" href="App_Data/bulma.min.css"/>
</asp:Content>
<asp:Content ID="Content2" ContentPlaceHolderID="ContentPlaceHolder1" runat="server">
    <asp:Label runat="server">Database: </asp:Label><asp:DropDownList runat="server" ID="ddlDB"></asp:DropDownList>
    <asp:Button ID="btnLoad" runat="server" OnClick="btnLoad_Click"/>
    <asp:DataGrid ID ="dgRecords" runat="server"></asp:DataGrid>
</asp:Content>
