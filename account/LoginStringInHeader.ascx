<%@ Control Language="C#" AutoEventWireup="true" CodeBehind="LoginStringInHeader.ascx.cs" Inherits="wstcp.LoginStringInHeader" %>
<asp:MultiView ID="MultiView1" runat="server">
    <asp:View ID="logined" runat="server">
        <!--Дописать код для вывода менеджера + шаблон отображения на фронте-->
        <div class="col-md-3 col-xs-3"><span class="text-bl">Менеджер:</span> Мерзлякова Нина</div>
        <div class="col-md-6 col-xs-6"> 
        <span class="text-bl">Пользователь:</span>&nbsp;<asp:Label ID="spanUserName" runat="server" CssClass="bold link" Text=""></asp:Label>
        <asp:Label ID="lbSubject" runat="server" Text=""></asp:Label>
        </div>
        <div class="col-md-2 col-xs-2">
        <asp:Label ID="lbCurrBlnc" runat="server" ToolTip="Текущий баланс по взаиморасчетам" Text=""></asp:Label>
        </div>
        <div class="col-md-1 col-xs-1">
        <a style="color: #ED7A44;" href="../account/login.aspx?act=exit">Выход</a>
        </div>
                           
        

    </asp:View>
    <asp:View ID="logoffed" runat="server"><div class="col-md-12 col-xs-12 text-right sign-link"><a href="../account/login.aspx">Войти</a> / <a href="../account/login.aspx?act=new">Регистрация</a></div></asp:View>   
</asp:MultiView>