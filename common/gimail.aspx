<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="gimail.aspx.cs" Inherits="wstcp.common.gimail" %>

<!DOCTYPE html PUBLIC "-//W3C//DTD XHTML 1.0 Transitional//EN" "http://www.w3.org/TR/xhtml1/DTD/xhtml1-transitional.dtd">

<html xmlns="http://www.w3.org/1999/xhtml">
<head runat="server">
    <title></title>
</head>
<body>
    <form id="form1" runat="server">
    <div id="pnlForm" runat="server">
        <div><strong>Кому</strong><br/><asp:TextBox ID="txTo" runat="server" 
                MaxLength="250" Width="100%"></asp:TextBox></div>
        <div><strong>Сообщение</strong><br/><asp:TextBox ID="txMess" runat="server" 
                TextMode="MultiLine" MaxLength="500" Rows="7" Width="100%"></asp:TextBox></div>
        <div>
            <asp:Button
            ID="btnSend" runat="server" Text="Отправить" onclick="btnSend_Click" />
            <asp:HiddenField ID="hdS" runat="server" />
        </div>
    </div>
    <asp:Literal ID="lbMess" runat="server"></asp:Literal>
    </form>
</body>
</html>
