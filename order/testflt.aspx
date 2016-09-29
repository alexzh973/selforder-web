<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="testflt.aspx.cs" Inherits="wstcp.order.testflt" %>

<!DOCTYPE html>

<html xmlns="http://www.w3.org/1999/xhtml">
<head runat="server">
<meta http-equiv="Content-Type" content="text/html; charset=utf-8"/>
    <title></title>
    <link href="../css/framework.css" rel="stylesheet" />
</head>
<body>
    <form id="form1" runat="server">
    <div>
        владелец <asp:DropDownList ID="dlOwner" runat="server" AutoPostBack="True" OnSelectedIndexChanged="dlOwner_SelectedIndexChanged">
            <asp:ListItem Value="100000">УЦСК</asp:ListItem>
            <asp:ListItem Value="100001">Челяб</asp:ListItem>
        </asp:DropDownList><br/>
        категория <asp:TextBox ID="txTK" runat="server">Баки расширительные</asp:TextBox><asp:Button ID="btnGo" runat="server" Text="ок" />

        <asp:Panel ID="pnlFilter" runat="server"></asp:Panel>
        <hr/>
        <asp:GridView ID="dgGoods" runat="server"></asp:GridView>
    </div>
    </form>
</body>
</html>
