<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="quefly.aspx.cs" Inherits="wstcp.order.quefly" %>

<!DOCTYPE  html PUBLIC "-//W3C//DTD XHTML 1.0 Transitional//EN" "http://www.w3.org/TR/xhtml1/DTD/xhtml1-transitional.dtd">
<html xmlns="http://www.w3.org/1999/xhtml">
<head runat="server">
    <title>Набор для прайс-листа</title>

    <script src="../js/jquery.js" type="text/javascript"></script>
    <link href="../js/jquery-ui.min.css" rel="stylesheet" />
    <script type="text/javascript" src="../js/jquery-ui.min.js"></script>
    <link href="../js/jquery-ui.structure.min.css" rel="stylesheet" />
    <link href="../js/jquery-ui.theme.min.css" rel="stylesheet" />
    <script src="../js/ensoCom.js" type="text/javascript"></script>

    <link href="../custom.css" rel="stylesheet" type="text/css" />
    

    <script type="text/javascript">
        var ordid = '<%=Request["id"]%>';
        $(function () {
            var yach = "#recnum_" + ordid;
            if (window.parent && window.parent.$(yach)) {
                window.parent.$(yach).removeClass("red");
                window.parent.$(yach).html("№" + ordid);
                window.parent.$(yach).attr("title", "уже посмотрели");
            }
        });


        function closethis() {
            if (window.parent && window.parent.$("#dialog")) {
                window.parent.$("#dialog").dialog("close");
                window.parent.$("#dialog").html("");
            }
        }
        function closethiswithrefresh() {
            if (window.parent && window.parent.$("#refresh")) {
                window.parent.$("#refresh").val("Y");
            }
            window.parent.$("form").submit();
            //if (window.parent && window.parent.$("#ord_" + ordid)) {
            //    window.parent.$("#ord_" + ordid).hide();
            //}
        }
    </script>
</head>
<body>
    <form id="form1" runat="server">
        <div class="message">
            <asp:Literal ID="lbMess" runat="server" EnableViewState="False"></asp:Literal><asp:Button ID="btnClosemess" runat="server" Text="Ок" EnableViewState="False" Visible="False" />
        </div>
        <div>
            <div>
                <table width="100%">
                    <tr>
                        <td width="120px">
                            <a href="#" onclick="printBlock('divorder')" class="button">печать прайс-листа</a>
                        </td>
                        <td>
                            
                           <%--<asp:LinkButton CssClass="button" ID="btnMakefile" runat="server" OnClick="btnMakefile_Click">сформировать файл Excel</asp:LinkButton>--%>
<div><asp:Literal ID="linkInvoice" runat="server"></asp:Literal></div>

                        </td>
                        <td class="right" width="250px">
<asp:LinkButton ID="btnRefresh" CssClass="button"  runat="server" ToolTip="проверить  статус (обновить информацию)" OnClick="btnRefresh_Click">обновить</asp:LinkButton>
                        </td>

                    </tr>
                </table>
            </div>

            <div id="divorder">
                <div >
                    <h2>
                        <asp:Literal ID="lbTitle" runat="server"></asp:Literal></h2>
                    <div>
                        Заказчик
                <strong>
                    <asp:Literal ID="lbSubject" runat="server"></asp:Literal></strong>
                    </div>
                    <div>
                        Прайс-лист
                <asp:Literal ID="lbAttr" runat="server"></asp:Literal>
                    </div>
                    <div>
                        Примечание:<br />
                        <asp:Literal ID="lbDescr" runat="server"></asp:Literal>
                    </div>
                </div>
                <asp:DataGrid ID="DataGrid1" runat="server" CellPadding="4" ForeColor="#333333" OnItemDataBound="DataGrid1_ItemDataBound"
                    AutoGenerateColumns="False" ShowFooter="True">
                    <AlternatingItemStyle BackColor="White" ForeColor="#284775" />
                    <Columns>
                        <asp:BoundColumn DataField="GoodCode" HeaderText="Код"></asp:BoundColumn>
                        <asp:BoundColumn DataField="Name" HeaderText="Наименование"></asp:BoundColumn>
                        <asp:BoundColumn DataField="Price" HeaderText="Цена" DataFormatString="{0:n}"></asp:BoundColumn>
                        <asp:BoundColumn DataField="Qty" HeaderText="Наличие"></asp:BoundColumn>
                        
                        <asp:BoundColumn DataField="Comment" ItemStyle-CssClass="small" HeaderText="">
                            <ItemStyle CssClass="small"></ItemStyle>
                        </asp:BoundColumn>
                        <asp:BoundColumn DataField="Descr" HeaderText="комментарий заказчика"></asp:BoundColumn>
                    </Columns>
                    <FooterStyle BackColor="#5D7B9D" Font-Bold="True" ForeColor="White" />
                    <HeaderStyle BackColor="#5D7B9D" Font-Bold="True" ForeColor="White" />
                    <ItemStyle BackColor="#F7F6F3" ForeColor="#333333" />
                    <PagerStyle BackColor="#284775" ForeColor="White" HorizontalAlign="Center" />
                    
                </asp:DataGrid>

            </div>
        </div>
        <asp:Literal ID="lbRecount" runat="server"></asp:Literal>
    </form>
</body>
</html>
