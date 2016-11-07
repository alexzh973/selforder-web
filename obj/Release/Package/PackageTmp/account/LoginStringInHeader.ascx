<%@ Control Language="C#" AutoEventWireup="true" CodeBehind="LoginStringInHeader.ascx.cs" Inherits="wstcp.LoginStringInHeader" %>
<asp:MultiView ID="MultiView1" runat="server">
    <asp:View ID="logined" runat="server">
         
        <span class="grayfon">пользователь:</span>&nbsp;<asp:Label ID="spanUserName" runat="server" CssClass="bold link" Text=""></asp:Label>
        <asp:Label ID="lbSubject" runat="server" Text=""></asp:Label>
        <asp:Label ID="lbCurrBlnc" runat="server" ToolTip="Текущий баланс по взаиморасчетам" Text=""></asp:Label>
        <a class=" padding2" style="color: #ED7A44; width: 80px; padding-left: 8px !important; padding-right: 8px !important;" href="../account/login.aspx?act=exit">выход</a>

                           
        

    </asp:View>
    <asp:View ID="logoffed" runat="server"><a href="../account/login.aspx">авторизация</a> | <a href="../account/login.aspx?act=new">не зарегистрированы?</a></asp:View>   
</asp:MultiView>