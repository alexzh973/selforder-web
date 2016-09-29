<%@ Control Language="C#" AutoEventWireup="true" CodeBehind="ListOrders.ascx.cs" Inherits="wstcp.ListOrders" %>
<asp:Repeater ID="rpNU" runat="server" OnItemCommand="rpNU_ItemCommand">
    <HeaderTemplate>
        <h3>Заявки новые. Ожидают предварительного подтверждения клиентом</h3>
        <ul>
    </HeaderTemplate>
    <ItemTemplate>

        <li><a title='<%#Eval("Descr") %>' onclick="openflywin('../order/detailfly.aspx?id=<%#Eval("ID") %>', 850, 800, 'Заявка <%#Eval("Name") %> от <%#Eval("RegDate") %>')"
                            href='javascript:return false;'><span class="<%#Eval("css") %>"><%#Eval("Name") %></span>, №<%#Eval("ID") %>от
                            <%#Eval("RegDate") %> {<%#Eval("SummOrder") %>руб.}</a> <%#Eval("linkchange") %></li>


    </ItemTemplate>
    <FooterTemplate>
        </ul>
    </FooterTemplate>
</asp:Repeater>

<asp:Repeater ID="rpAZ" runat="server">
    <HeaderTemplate>
        <h3>Заявки в процессе согласования у поставщика</h3>
        <ul>
    </HeaderTemplate>
    <ItemTemplate>

        <li><a title='<%#Eval("Descr") %>' onclick="openflywin('../order/detailfly.aspx?id=<%#Eval("ID") %>', 850, 800, 'Заявка <%#Eval("Name") %> от <%#Eval("RegDate") %>')"
                            href='javascript:return false;'><span class="<%#Eval("css") %>"><%#Eval("Name") %></span>, №<%#Eval("ID") %>от
                            <%#Eval("RegDate") %> на <%#Eval("SummOrder") %> руб.</a> - <%#Eval("state") %> <%#Eval("linkchange") %></li>

    </ItemTemplate>
    <FooterTemplate>
        </ul>
    </FooterTemplate>
</asp:Repeater>

<asp:Repeater ID="rpS" runat="server">
    <HeaderTemplate>
        <h3>Заявки согласованы поставщиком и ожидают окончательного подтверждения клиентом</h3>
        <ul>
    </HeaderTemplate>
    <ItemTemplate>

        <li><a title='<%#Eval("Descr") %>' onclick="openflywin('../order/detailfly.aspx?id=<%#Eval("ID") %>', 850, 800, 'Заявка <%#Eval("Name") %> от <%#Eval("RegDate") %>')"
                            href='javascript:return false;'><span class="<%#Eval("css") %>"><%#Eval("Name") %></span>, №<%#Eval("ID") %>от
                            <%#Eval("RegDate") %> {<%#Eval("SummOrder") %>руб.}</a></li>


    </ItemTemplate>
    <FooterTemplate>
        </ul>
    </FooterTemplate>
</asp:Repeater>





<asp:Repeater ID="rpMR" runat="server">
    <HeaderTemplate>
        <h3>Заявки на комплектации</h3>
        <ul>
    </HeaderTemplate>
    <ItemTemplate>
        <li><a title='<%#Eval("Descr") %>' onclick="openflywin('../order/detailfly.aspx?id=<%#Eval("ID") %>', 850, 800, 'Заявка <%#Eval("Name") %> от <%#Eval("RegDate") %>')"
                            href='javascript:return false;'><span class="bold"><%#Eval("Name") %></span>, №<%#Eval("ID") %>от
                            <%#Eval("RegDate") %> {<%#Eval("SummOrder") %>руб.}</a> <i>тек. статус: <%#Eval("State") %></i></li>
    </ItemTemplate>
    <FooterTemplate>
        </ul>
    </FooterTemplate>
</asp:Repeater>
<asp:Repeater ID="rpF" runat="server">
    <HeaderTemplate>
        <h3>Выполненные заявки</h3>
        <ul>
    </HeaderTemplate>
    <ItemTemplate>

        <li><a title='<%#Eval("Descr") %>' onclick="openflywin('../order/detailfly.aspx?id=<%#Eval("ID") %>', 850, 800, 'Заявка <%#Eval("Name") %> от <%#Eval("RegDate") %>')"
                            href='javascript:return false;'><span class="bold"><%#Eval("Name") %></span>, №<%#Eval("ID") %>от
                            <%#Eval("RegDate") %> {<%#Eval("SummOrder") %>руб.}</a></li>


    </ItemTemplate>
    <FooterTemplate>
        </ul>
    </FooterTemplate>
</asp:Repeater>
<asp:Repeater ID="rpDX" runat="server">
    <HeaderTemplate>
        <h3>Отмененные заявки и архивные, не пошедшие в дело</h3>
        <ul>
    </HeaderTemplate>
    <ItemTemplate>
        <li>
                        
                        <a title='' onclick="openflywin('../order/detailfly.aspx?id=<%#Eval("ID") %>', 850, 800, 'Заявка <%#Eval("Name") %> от <%#Eval("RegDate") %>')"
                            href='javascript:return false;'><span class="bold"><%#Eval("Name") %></span>, №<%#Eval("ID") %>от
                            <%#Eval("RegDate") %> {<%#Eval("SummOrder") %>руб.}</a> (<i>состояние: <%#Eval("State") %></i>) <%#Eval("linkchange") %>
                        
                        
                        </li>
    </ItemTemplate>
    <FooterTemplate>
        <ul>
    </FooterTemplate>
</asp:Repeater>

