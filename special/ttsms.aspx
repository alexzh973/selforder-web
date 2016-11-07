<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="ttsms.aspx.cs" Inherits="wstcp.special.ttsms" %>

<!DOCTYPE html>

<html xmlns="http://www.w3.org/1999/xhtml">
<head runat="server">
<meta http-equiv="Content-Type" content="text/html; charset=utf-8"/>
    <title></title>
</head>
<body>
    <form id="form1" runat="server">
    <div>
        тел <asp:TextBox ID="txtel" runat="server"></asp:TextBox><br />
        мессадж<asp:TextBox ID="msg" runat="server"></asp:TextBox>
        <br/>
        <asp:Button ID="Button1" runat="server" Text="Button" OnClick="Button1_Click" />
        
        <div><asp:Label ID="Label1" runat="server" Text=""></asp:Label></div>
    </div>
    </form>
</body>
</html>
