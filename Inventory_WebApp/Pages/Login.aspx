<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="Login.aspx.cs" Inherits="WebApplication1.Login" %>

<!DOCTYPE html>

<html xmlns="http://www.w3.org/1999/xhtml">
<head runat="server">
    <title>Login Page</title>
    <style>
        body{
            background-color: aliceblue;
        }
        .auto-style1 {
            height: 29px;
            width: 283px;
        }
        .auto-style2 {
            text-align: center;
        }
        .auto-style5 {
            width: 107px;
        }
        .auto-style6 {
            height: 29px;
            width: 107px;
        }
        .auto-style7 {
            text-align: center;
            font-size: xx-large;
            color: #336600;
        }
        .auto-style10 {
            color: #FF0000;
            font-size: medium;
        }
        .auto-style11 {
            width: 283px;
            text-align: left;
        }
        .auto-style13 {
            height: 29px;
            width: 283px;
            font-size: large;
            color: #CC0000;
            text-decoration: underline;
        }
    </style>
</head>
<body>
    <form id="form1" runat="server">
        <div class="auto-style7">

            ETS Inventory Login Page</div>
        <div>
            <table style = "margin:auto; border:7px blue" align="center">
                <tr>
                    <td class="auto-style5">
                        <strong>
                        <asp:Label ID="Label1" runat="server" Text="Username"></asp:Label></strong>:</td>
                    <td class="auto-style11">
                        <asp:TextBox ID="TextBoxUser" runat="server" Width="128px"></asp:TextBox><strong>
                        <asp:RequiredFieldValidator ID="errorUser" runat="server" ControlToValidate="TextBoxUser" CssClass="auto-style10" ErrorMessage="Username Required"></asp:RequiredFieldValidator>
                        </strong></td>
                </tr>
                <tr>
                    <td class="auto-style5">
                        <strong>
                        <asp:Label ID="Label2" runat="server" Text="Password"></asp:Label></strong>:</td>
                    <td class="auto-style11">
                        <asp:TextBox ID="TextBoxPass" runat="server" Width="128px" TextMode="Password"></asp:TextBox><strong>
                        <asp:RequiredFieldValidator ID="errorPass" runat="server" ControlToValidate="TextBoxPass" CssClass="auto-style10" ErrorMessage="Password Required"></asp:RequiredFieldValidator>
                        </strong></td>
                </tr>
                <tr>
                    <td class="auto-style6">
                        </td>
                    <td class="auto-style1">
                        <asp:Button ID="Clk_Login" runat="server" Text="Login" OnClick="Clk_Login_Click" Width="128px" /></td>
                </tr>
                <tr>
                    <td class="auto-style6"></td>
                    <td class="auto-style13">
                        <strong>
                        <asp:Label ID="errorCreds" runat="server" Text="Incorrect Credentials"></asp:Label>
                        </strong></td>
                </tr>
            </table>
        </div>
        <p>
            &nbsp;</p>
    </form>
</body>
</html>
