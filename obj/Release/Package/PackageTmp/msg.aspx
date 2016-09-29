<%@ Page Title="" Language="C#" MasterPageFile="~/common/Preview.Master" AutoEventWireup="true" CodeBehind="msg.aspx.cs" Inherits="wstcp.msg" %>
<asp:Content ID="Content1" ContentPlaceHolderID="head" runat="server">
    <script type="text/javascript">

        function closethis() {
            if (window.parent && window.parent.$("#dialog")) {
                window.parent.$("#dialog").dialog("close");
                window.parent.$("#dialog").html("");
            }
        }

        </script>
</asp:Content>

<asp:Content ID="Content3" ContentPlaceHolderID="ContentPlaceHolder1" runat="server">
    <div class="message"><asp:Literal ID="lbMessage" runat="server"></asp:Literal></div>
    
    <asp:PlaceHolder ID="pnlMsg" runat="server">
    <h1>Напишите нам письмо</h1>
    <asp:Literal ID="lbMess" runat="server"></asp:Literal>
    <h3>Получатели сообщения</h3>
    <div><asp:CheckBox ID="chTosupport" ClientIDMode="Static" runat="server" Text="Служба поддержки" /> <label for="chTosupport"><asp:Literal
            ID="lbsupportemail" runat="server"></asp:Literal></label></div>
    <div><asp:CheckBox ID="chToTa" ClientIDMode="Static" runat="server" Text="email персонального менеджера"  /> <label for="chToTa"><asp:Literal
            ID="lbtaemail" runat="server"></asp:Literal></label></div>
<asp:TextBox ID="txAny" runat="server"></asp:TextBox>
   <div>
    <asp:TextBox ID="txText" CssClass="bigsize" Width="99%" runat="server" Rows="10" MaxLength="500" TextMode="MultiLine"></asp:TextBox></div>
    <div class="center">
        <asp:Button ID="btnSend"
        runat="server" Text="Отправить" Width="80px" onclick="btnSend_Click" /> 
        <asp:Button ID="btnCancel" 
            runat="server" Text="Отмена" Width="80px" onclick="btnCancel_Click" OnPreRender="btnCancel_PreRender" /></div></asp:PlaceHolder>
</asp:Content>

