<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="detailfly.aspx.cs" Inherits="wstcp.order.detailfly" %>

<!DOCTYPE html PUBLIC "-//W3C//DTD XHTML 1.0 Transitional//EN" "http://www.w3.org/TR/xhtml1/DTD/xhtml1-transitional.dtd">
<html xmlns="http://www.w3.org/1999/xhtml">
<head runat="server">
    <title>Заявка</title>
    <link href="../custom.css" rel="stylesheet" type="text/css" />
    <script src="../js/jquery.js" type="text/javascript"></script>
    <script src="../js/ensoCom.js" type="text/javascript"></script>

    <style type="text/css">
        .printSelected div
        {
            display: none;
        }
        /* скрываем весь контент на странице */
        .printSelected div.printSelection
        {
            display: block;
        }
        /* делаем видимым только тот блок, который подготовлен для печати */
        .printSelected div.printSelection div
        {
            display: block;
        }
        /* показываем всех его потомков, которые были скрыты первой строкой */
    </style>
</head>
<body>
    <form id="form1" runat="server">
    <div>
        <a href="#" onclick="printBlock('divorder')" class="button">
            <img src="../simg/16/printer.png" />
            печать</a>
        <div id="divorder">
            <h2>
                <asp:Literal ID="lbTitle" runat="server"></asp:Literal></h2>
            <div>
                Заявка
                <asp:Literal ID="lbAttr" runat="server"></asp:Literal></div>
            <div>
                Текущий статус: <strong>
                    <asp:Literal ID="lbState" runat="server"></asp:Literal></strong></div>
            <div>
                Код в учетной системе <span class="bold">
                    <asp:Literal ID="lbCode" runat="server"></asp:Literal></span></div>
            <div>
                Ответственный менеджер: <strong>
                    <asp:Literal ID="Literal1" runat="server"></asp:Literal></strong></div>
            <div>
                Примечание:<br />
                <asp:Literal ID="lbDescr" runat="server"></asp:Literal></div>
            <asp:DataGrid ID="dgItems" runat="server" CellPadding="4" ForeColor="#333333" OnItemDataBound="dgItems_ItemDataBound"
                AutoGenerateColumns="False" ShowFooter="True">
                <AlternatingItemStyle BackColor="White" ForeColor="#284775" />
                <Columns>
                    <asp:BoundColumn DataField="goodcode" HeaderText="Код"></asp:BoundColumn>
                    <asp:BoundColumn DataField="name" HeaderText="Наименование"></asp:BoundColumn>
                    <asp:BoundColumn DataField="pr" DataFormatString="{0:D}" HeaderText="Цена"></asp:BoundColumn>
                    <asp:BoundColumn DataField="qty" DataFormatString="{0:N2}" HeaderText="Кол-во"></asp:BoundColumn>
                    <asp:BoundColumn DataField="sum" DataFormatString="{0:N0}" HeaderText="Стоимость">
                    </asp:BoundColumn>
                    <asp:BoundColumn DataField="descr" HeaderText="комментарий заказчика"></asp:BoundColumn>
                    <asp:BoundColumn DataField="comment" HeaderText="комментарий УЦСК"></asp:BoundColumn>
                </Columns>
                <EditItemStyle BackColor="#999999" />
                <FooterStyle BackColor="#5D7B9D" Font-Bold="True" ForeColor="White" />
                <HeaderStyle BackColor="#5D7B9D" Font-Bold="True" ForeColor="White" />
                <ItemStyle BackColor="#F7F6F3" ForeColor="#333333" />
                <PagerStyle BackColor="#284775" ForeColor="White" HorizontalAlign="Center" />
                <SelectedItemStyle BackColor="#E2DED6" Font-Bold="True" ForeColor="#333333" />
            </asp:DataGrid>
        </div>
    </div>
    </form>
</body>
</html>
