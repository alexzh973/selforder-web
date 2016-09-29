<%@ Page Title="" Language="C#" MasterPageFile="~/common/mainpage.Master" AutoEventWireup="true" CodeBehind="msg.aspx.cs" Inherits="wstcp.msg" %>
<asp:Content ID="Content1" ContentPlaceHolderID="head" runat="server">
</asp:Content>
<asp:Content ID="Content2" ContentPlaceHolderID="place_Left" runat="server">
</asp:Content>
<asp:Content ID="Content3" ContentPlaceHolderID="ContentPlaceHolder1" runat="server">
    <h1>Послать сообщение</h1>
    <asp:Literal ID="lbMess" runat="server"></asp:Literal>
    <h3>Получатели сообщения</h3>
    <div><asp:CheckBox ID="chTosupport" ClientIDMode="Static" runat="server" Text="Служба поддержки" /> <label for="chTosupport"><asp:Literal
            ID="lbsupportemail" runat="server"></asp:Literal></label></div>
    <div><asp:CheckBox ID="chToTa" ClientIDMode="Static" runat="server" Text="email персонального менеджера"  /> <label for="chToTa"><asp:Literal
            ID="lbtaemail" runat="server"></asp:Literal></label></div>
   <div>
    <asp:TextBox ID="txText" CssClass="bigsize" Width="99%" runat="server" Rows="10" MaxLength="500" TextMode="MultiLine"></asp:TextBox></div>
    <div class="center">
        <asp:Button ID="btnSend"
        runat="server" Text="Отправить" Width="80px" onclick="btnSend_Click" /> 
        <asp:Button ID="btnCancel" 
            runat="server" Text="Отмена" Width="80px" onclick="btnCancel_Click" /></div>
</asp:Content>
<asp:Content ID="Content4" ContentPlaceHolderID="place_Right" runat="server">
</asp:Content>
