<%@ Page Title="" Language="C#" MasterPageFile="~/common/Preview.Master" AutoEventWireup="true" CodeBehind="angood.aspx.cs" Inherits="wstcp.good.angood" %>

<asp:Content ID="Content1" ContentPlaceHolderID="head" runat="server">
        <script type="text/javascript">

            function closethis() {
                if (window.parent && window.parent.$("#dialog")) {
                    window.parent.$("#dialog").dialog("close");
                    window.parent.$("#dialog").html("");
                }
            }
            function closethiswithrefresh() {
                if (window.parent && window.parent.$("#refresh")) {
                    window.parent.$("#refresh").val("Y");
                    window.parent.$("form").submit();
                } else if (window.parent && window.parent.$("#dialog")) {
                    window.parent.$("#dialog").dialog("close");
                    window.parent.$("#dialog").html("");
                }
            }
        </script>

</asp:Content>
<asp:Content ID="Content2" ContentPlaceHolderID="ContentPlaceHolder1" runat="server">

    <asp:Literal ID="lb" runat="server"></asp:Literal>
<asp:Repeater ID="rpK" runat="server">
        <HeaderTemplate><h4>Комплектующие</h4></HeaderTemplate>
        <ItemTemplate>
            <div>
                <strong><%#Eval("Name") %></strong>
                <br/><a href="angood.aspx?good=<%= Request["good"] %>&sel=<%#Eval("goodcode") %>&ant=k" title="Внимание! добавляется в соответствии с количеством основного товара">добавить в заявку</a>
                <p class="leftpadding">цена <%#Eval("price","{0:N2}") %>руб.</p><p class="leftpadding">наличие <%#Eval("qty","{0:N2}") %><%#Eval("ed") %></p>
            </div>
        </ItemTemplate>
    </asp:Repeater>    
    <asp:Repeater ID="rpS" runat="server">
        <HeaderTemplate><h4>Сопутствующие товары</h4></HeaderTemplate>
        <ItemTemplate>
            <div>
                <strong><%#Eval("Name") %></strong>
                <br/><a href="angood.aspx?good=<%= Request["good"] %>&sel=<%#Eval("goodcode") %>&ant=s" title="Внимание! добавляется в соответствии с количеством основного товара">добавить в заявку</a>
                <p class="leftpadding">цена <%#Eval("price","{0:N2}") %>руб.</p><p class="leftpadding">наличие <%#Eval("qty","{0:N2}") %><%#Eval("ed") %></p>
            </div>
        </ItemTemplate>
    </asp:Repeater>


<asp:Repeater ID="rpA" runat="server">
        <HeaderTemplate><h4>Аналоги</h4></HeaderTemplate>
        <ItemTemplate>
            <div><strong><%#Eval("Name") %></strong><br/><a href="angood.aspx?good=<%= Request["good"] %>&sel=<%#Eval("goodcode") %>&ant=a">заменить на этот</a>
                <p class="leftpadding">цена <%#Eval("price","{0:N2}") %>руб.</p><p class="leftpadding">наличие <%#Eval("qty","{0:N2}") %><%#Eval("ed") %></p></div>
        </ItemTemplate>
    </asp:Repeater>
</asp:Content>

