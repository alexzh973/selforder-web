<%@ Page Title="" Language="C#" MasterPageFile="common/mainpage.Master" AutoEventWireup="true"
    CodeBehind="default01.aspx.cs" Inherits="wstcp._default01" %>




<asp:Content ID="Content1" ContentPlaceHolderID="head" runat="server">
    <link href="css/bootstrap.min.css" rel="stylesheet" />
</asp:Content>
<asp:Content ID="Content2" ContentPlaceHolderID="ContentPlaceHolder1" runat="server">

    <div class="message">
        <asp:Literal ID="lbMess" runat="server"></asp:Literal>
    </div>


    <asp:Panel ID="pnlSubjectFilter" runat="server">

        <br />
        Выбрать клиента
                <asp:DropDownList CssClass="slowfunc" ID="dlSubjects" runat="server" AutoPostBack="True" OnSelectedIndexChanged="dlSubjects_SelectedIndexChanged" ClientIDMode="Static" data-placeholder="Выберите клиента" Width="300px"></asp:DropDownList>
        <asp:LinkButton ID="btnRempsw" ToolTip="помочь вспомнить пароль" runat="server" OnClick="btnRempsw_Click">напомнить пароль</asp:LinkButton>
        <a href='../account/login.aspx?act=new&own=<%= iam.OwnerID %>' class="button">Добавить нового клиента</a>
        <br />
        <br />




    </asp:Panel>



    <asp:Repeater ID="rpNews" runat="server">
        <HeaderTemplate>
            <h2>Новости</h2>
        </HeaderTemplate>
        <ItemTemplate>
            <div class="shadow floatLeft border padding5 " style="width: 400px; margin: 4px;">
                <span class="bluefon white small bold padding2 "><%#Eval("reg_date") %></span>&nbsp;
                    <span class="bold"><a href="javascript:return 0" class="italic small" onclick="openflywin('../news/preview.aspx?id=<%#Eval("ID") %>',600,700,'<%#Eval("Name") %>')"><%#Eval("Name") %></a></span>
                <p>
                    <%#Eval("Descr") %>
                    <a href="javascript:return 0" class="italic small" onclick="openflywin('../news/preview.aspx?id=<%#Eval("ID") %>',600,700,'<%#Eval("Name") %>')">подробнее...</a>
                </p>
            </div>

        </ItemTemplate>
        <FooterTemplate>
            <div class="clearBoth"></div>
        </FooterTemplate>
    </asp:Repeater>



    <asp:Literal ID="litInfo" runat="server"></asp:Literal>


    <asp:Repeater ID="rpTeo" runat="server">
        <HeaderTemplate>
            <h2>Запланированные доставки</h2>
            <div>
        </HeaderTemplate>
        <ItemTemplate>
            <div class="floatLeft border shadow padding5" style="width: 250px; margin: 5px;">
                <p>!<span class="orangefon white bold"><%# Eval("TeoDate","{0:d}") %></span></p>
                <p title="<%# Eval("Name") %>">
                    <a onclick="openflywin('../order/detailfly.aspx?id=<%# Eval("Id") %>', 870, 850, '<%# Eval("Name") %>')" href='javascript:return false;'><%# Eval("Name") %></a>



                </p>
                <p><%# Eval("TeoTrans") %></p>
                <p>адрес доставки <strong><%# Eval("TeoAddress") %></strong></p>
            </div>

        </ItemTemplate>
        <FooterTemplate></div><div class="clearBoth"></div>
        </FooterTemplate>
    </asp:Repeater>


    <asp:Repeater ID="rpAvailible" runat="server" Visible="False">
        <HeaderTemplate>
            <h2>Вы можете получить сегодня!!!</h2>
            <table>
        </HeaderTemplate>
        <ItemTemplate>
            <tr>
                <td><%# Eval("GoodName") %></td>
                <td><%# Eval("Можно") %></td>
                <td title="<%# Eval("OrderName") %>">
                    <a onclick="openflywin('../order/detailfly.aspx?id=<%# Eval("OrderId") %>', 870, 850, '<%# Eval("OrderName") %>')" href='javascript:return false;'><%# Eval("OrderName") %></a>
                    [ID <%# Eval("OrderId") %>::<%# Eval("OrderCode") %>от <%# Eval("RegDate") %>]
                            

                </td>
            </tr>

        </ItemTemplate>
        <FooterTemplate></table></FooterTemplate>
    </asp:Repeater>

    <br />
    <div>
        Найти заявку по номеру, коду или товару:
                    <asp:TextBox ID="txSearch" runat="server"></asp:TextBox>
        <asp:Button ID="btnSearch" runat="server" OnClick="btnSearch_Click" CssClass="f-bu f-bu-default slowfunc" Text="найти" />
    </div>
    <br />
    <asp:HiddenField ID="hdRefresh" ClientIDMode="Static" runat="server" />


    <asp:RadioButtonList ID="rbStateOrders" OnSelectedIndexChanged="btnSearch_Click" runat="server" RepeatDirection="Vertical" AutoPostBack="true" CssClass="small normalweight">

        <asp:ListItem Value="W" Selected="True">Ожидающие решения клиента <i class="glyphicon glyphicon-question-sign big"></i></asp:ListItem>
        <asp:ListItem Value="Z">В процессе обработки с Сантехкомплекте <i class="glyphicon glyphicon-time big"></i></asp:ListItem>
        <asp:ListItem Value="R">На комплектации и готовы к отгрузке <i class="glyphicon glyphicon-share big"></i></asp:ListItem>
        <asp:ListItem Value="D">Отмененные <i class="glyphicon glyphicon-trash big"></i></asp:ListItem>
        <asp:ListItem Value="F">Выполненные <i class="glyphicon glyphicon-check big"></i></asp:ListItem>

    </asp:RadioButtonList>
    <asp:Literal ID="lbMsgOrders" runat="server"></asp:Literal>

    <asp:DataGrid ID="dgList" runat="server" AutoGenerateColumns="False"
        CellPadding="6"
        OnItemDataBound="dgList_ItemDataBound"
        AllowPaging="True"
        OnPageIndexChanged="dgList_PageIndexChanged"
        ForeColor="#333333"
        GridLines="None"
        CellSpacing="-1"
        Width="100%"
        OnItemCreated="dgList_ItemCreated"
        AllowSorting="True"
        OnSortCommand="dgList_SortCommand">
        <Columns>
            <asp:TemplateColumn Visible="false">
                <HeaderStyle Width="10px" />
            </asp:TemplateColumn>
            <asp:BoundColumn DataField="id" Visible="true" DataFormatString="№{0}" SortExpression="id" HeaderText="№">
                <ItemStyle Width="80px" />
            </asp:BoundColumn>
            <asp:BoundColumn DataField="RegDate" DataFormatString="{0:d}" SortExpression="regdate" HeaderText="Дата">

                <ItemStyle Width="80px" />

            </asp:BoundColumn>
            <asp:BoundColumn DataField="Name" HeaderText="Заявка"></asp:BoundColumn>
            <asp:BoundColumn DataField="SummOrder" ItemStyle-CssClass="right" DataFormatString="{0:n}р." SortExpression="summ" HeaderText="Сумма">
                <ItemStyle Width="80px" CssClass="right" />

            </asp:BoundColumn>
            <asp:TemplateColumn ItemStyle-CssClass="small" HeaderText="Состояние">

                <ItemStyle CssClass="small" Width="100px"></ItemStyle>
            </asp:TemplateColumn>
            <asp:TemplateColumn ItemStyle-CssClass="small" HeaderText="Возможные">

                <ItemStyle CssClass="small" Width="100px"></ItemStyle>
            </asp:TemplateColumn>
            <asp:TemplateColumn HeaderText="действия">
                <ItemStyle Width="80px" />

            </asp:TemplateColumn>


            <asp:TemplateColumn></asp:TemplateColumn>


        </Columns>
        <EditItemStyle BackColor="#999999" />
        <FooterStyle BackColor="#5D7B9D" Font-Bold="True" ForeColor="White" />
        <HeaderStyle BackColor="#5D7B9D" Font-Bold="True" ForeColor="White" />
        <ItemStyle CssClass="selectablehover" BackColor="#F7F6F3" ForeColor="#333333" />
        <AlternatingItemStyle CssClass="selectablehover" BackColor="White" ForeColor="#284775" />

        <PagerStyle NextPageText=" следующие 10 &amp;gt;" Position="TopAndBottom" PrevPageText="&amp;lt; предыдущие 10 " BackColor="#EBEEF2" ForeColor="#003366" CssClass="mypager center" HorizontalAlign="Center" Mode="NumericPages" />
        <SelectedItemStyle BackColor="#E2DED6" Font-Bold="True" ForeColor="#333333" />
    </asp:DataGrid>



    <asp:DataGrid ID="dgQuery" runat="server" AutoGenerateColumns="False"
        CellPadding="6"
        OnItemDataBound="dgQuery_ItemDataBound"
        AllowPaging="True"
        OnPageIndexChanged="dgQuery_PageIndexChanged"
        ForeColor="#333333"
        GridLines="None"
        CellSpacing="-1"
        Width="100%"
        OnItemCreated="dgQuery_ItemCreated">
        <Columns>
            <asp:TemplateColumn Visible="false">
                <HeaderStyle Width="10px" />
            </asp:TemplateColumn>
            <asp:BoundColumn DataField="id" Visible="true" DataFormatString="№{0}" SortExpression="id" HeaderText="№">
                <ItemStyle Width="80px" />
            </asp:BoundColumn>
            <asp:BoundColumn DataField="RegDate" DataFormatString="{0:d}" SortExpression="regdate" HeaderText="Дата">

                <ItemStyle Width="80px" />

            </asp:BoundColumn>
            <asp:BoundColumn DataField="Name" HeaderText="Заявка"></asp:BoundColumn>
            <asp:TemplateColumn ItemStyle-CssClass="small" HeaderText="Подробности">
                <ItemStyle Width="200px" CssClass="small" />

            </asp:TemplateColumn>

            <asp:TemplateColumn HeaderText="действия">
                <ItemStyle Width="80px" />

            </asp:TemplateColumn>


            <asp:TemplateColumn></asp:TemplateColumn>


        </Columns>
        <EditItemStyle BackColor="#999999" />
        <FooterStyle BackColor="#5D7B9D" Font-Bold="True" ForeColor="White" />
        <HeaderStyle BackColor="#5D7B9D" Font-Bold="True" ForeColor="White" />
        <ItemStyle CssClass="selectablehover" BackColor="#F7F6F3" ForeColor="#333333" />
        <AlternatingItemStyle CssClass="selectablehover" BackColor="White" ForeColor="#284775" />

        <PagerStyle NextPageText=" следующие 10 &amp;gt;" Position="TopAndBottom" PrevPageText="&amp;lt; предыдущие 10 " BackColor="#EBEEF2" ForeColor="#003366" CssClass="mypager center" HorizontalAlign="Center" Mode="NumericPages" />
        <SelectedItemStyle BackColor="#E2DED6" Font-Bold="True" ForeColor="#333333" />
    </asp:DataGrid>


















    <div></div>


</asp:Content>

<asp:Content runat="server" ID="scr1" ContentPlaceHolderID="placeScripts">


    <script type="text/javascript">
        $(document).ready(function () {

            $(".pic").mouseover(function (e) { $(this).addClass('picover'); });
            $(".pic").mouseout(function (e) { $(this).removeClass('picover'); });


        });

        function checkemail(emailfield, messagefieldId) {

            if ($('#' + emailfield) && '' + $('#' + emailfield).val() == '') {
                alert('в поле логин (еmail) нужно указать свой еmail');
                return false;
            }
            else {
                return true;
            }

        }

        function setnewstate(id, newstate) {
            $.ajax({
                url: "../order/orderajax.ashx?sid=" + sid + "&act=sns&ns=" + newstate + "&id=" + id,
                cache: false,
                success: function (data) {
                    if (newstate == 'D' || newstate == 'X')
                    { $("tr[id*='ord_" + id + "']").hide(); }

                    $('#hdRefresh').val('Y');
                    $('form').submit();


                },
                error: function (jqxhr, status, errorMsg) {
                    $("#qmsg").text(errorMsg);
                    setInterval(clearmsg, 7000);
                }
            });
        }
    </script>




    <script type="text/javascript">

        $(function () {
            $('select').chosen({ no_results_text: 'Нет результатов по' });

        });




    </script>
    <script src="js/bootstrap.min.js"></script>
</asp:Content>
