<%@ Page Title="" Language="C#" MasterPageFile="common/mainpage.Master" AutoEventWireup="true"
    CodeBehind="defaultold.aspx.cs" Inherits="wstcp._defaultold" %>


<%@ Register Src="~/order/ucOrdersTable.ascx" TagPrefix="uc1" TagName="ucOrdersTable" %>


<asp:Content ID="Content1" ContentPlaceHolderID="head" runat="server">
 
</asp:Content>
<asp:Content ID="Content2" ContentPlaceHolderID="ContentPlaceHolder1" runat="server">
    
    <div class="message">
        <asp:Literal ID="lbMess" runat="server"></asp:Literal>
    </div>
    <asp:LinkButton ID="btnToNewStyle" runat="server" OnClick="btnToNewStyle_Click">новый стиль главной страницы</asp:LinkButton>
    <asp:MultiView ID="MultiView1" runat="server">
        <asp:View ID="vLogin" runat="server">
            <div class="center white bluefon padding5">
                <h2 class="center white bluefon">На этом сайте</h2>
                <p class="center">
                    Можно самостоятельно оформить заказ на сантехнические изделия и оборудование
                </p>

                <p class="center">
                    Это доступно только для зарегистрированных клиентов.<br />
                    Обратитесь в компанию ООО &quot;УЦСК &quot;Сантехкомплект-Урал&quot;
                    <br />
                    к закрепленному за Вами менеджеру за получением кода доступа.<br />
                    либо по тел. (343) 270-04-04 (доб. 5297 )
                </p>
                <br />
                <p class="bold center">Либо, Самостоятельно! заполните заявку:</p>
                <br />
                <div style="padding: 40px; text-align: center;">
                    <a href='../account/login.aspx?act=new' style="padding: 30px; font-size: 200%; font-weight: bold; background-color: #ED7A44; color: #FFF; text-decoration: underline;">ХОЧУ ПОЛУЧИТЬ ДОСТУП!</a>
                </div>

            </div>
            <br />

            <div class="padding5 orangefon white">
                <div class="message">
                    <asp:Literal ID="lbMessageLogin" runat="server"></asp:Literal>
                </div>
                У МЕНЯ ЕСТЬ ДОСТУП
                <div class="f-row">
                    <label>логин (e-mail)</label>
                    <div class="f-input">
                        <asp:TextBox ID="txLogin" runat="server" MaxLength="150" CssClass="g-3"></asp:TextBox>
                    </div>
                    <!-- f-input -->
                </div>
                <!-- f-row -->
                <div class="f-row">
                    <label>пароль</label>
                    <div class="f-input">
                        <asp:TextBox ID="txPwd" runat="server" TextMode="Password" MaxLength="150" CssClass="g-3"></asp:TextBox>
                    </div>
                </div>
                <div class="f-row ">
                    <label></label>
                    <asp:Button ID="btnLogin" runat="server" Text="вход" OnClick="btnLogin_Click" Width="75px" />
                    <asp:LinkButton ID="lbtnRemember" runat="server" OnPreRender="lbtnRemember_PreRender" CssClass="white">забыли пароль?</asp:LinkButton>
                </div>



            </div>
            <br />
            <br />
            <asp:Panel ID="pnlScreens" runat="server">



                <h2 class="center white bluefon">Cценарий использования</h2>




                <div style="text-align: center;">
                    <img class="pic-" src='../media/screens/s1.jpg' alt='s1' title="Определиться с товарным направлением">
                    <img class="pic-ar" src='../media/screens/ar_right.jpg' alt='s2'>
                    <img class="pic-" src='../media/screens/s2.jpg' alt='s2' title="Выбрать номенклатуру, используюя фильтры и поиск">
                    <img class="pic-ar" src='../media/screens/ar_right.jpg' alt='s2'>
                    <img class="pic-" src='../media/screens/s3.jpg' alt='s3' title="Указать требуемое количество. Добавить к заявке наименование и комментарий. Сохранить">
                    <img class="pic-ar" src='../media/screens/ar_right.jpg' alt='s2'><br />
                    <img class="pic-ar" src='../media/screens/ar_right.jpg' alt='s2'>
                    <img class="pic-" src='../media/screens/s4.jpg' alt='s4' title="Заявка автоматически попадет в учетную систему Сантехкомплекта ">
                    <img class="pic-ar" src='../media/screens/ar_right.jpg' alt='s2'>
                    <img class="pic-" src='../media/screens/s5.jpg' alt='s5' title="и через несколько мунут в Вашей заявке будут указаны цены и скидки предоставляемые по условиям договора">
                    <img class="pic-ar" src='../media/screens/ar_right.jpg' alt='s2'>
                    <img class="pic-" src='../media/screens/s6.jpg' alt='s6' title="После этого наш менеджер свяжется с Вами для выставления счета на оплату"><br />
                </div>
            </asp:Panel>
        </asp:View>
        <asp:View ID="vOrders" runat="server">
            <asp:Panel ID="pnlSubjectFilter" runat="server">
                
                <br/>
                Выбрать клиента
                <asp:DropDownList CssClass="slowfunc" ID="dlSubjects" runat="server" AutoPostBack="True" OnSelectedIndexChanged="dlSubjects_SelectedIndexChanged" ClientIDMode="Static" data-placeholder="Выберите клиента" Width="300px" ></asp:DropDownList> 
                <asp:LinkButton ID="btnRempsw" ToolTip="помочь вспомнить пароль" runat="server" OnClick="btnRempsw_Click">напомнить пароль</asp:LinkButton> 
                <a href='../account/login.aspx?act=new&own=<%= iam.OwnerID %>' class="button">Добавить нового клиента</a>
                <br/>
                <br/>




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
            
            <asp:Panel ID="pnl2arch" runat="server" Visible="False" CssClass="sticker shadow padding5">
                <script type="text/javascript">
                    function showarchs(isshow) {

                        if (isshow) { $("img[src*='time']").css("vertical-align", "bottom"); }
                        else { $("img[src*='time']").css("vertical-align", "top"); }
                    }

                </script>
                <div class=" right">
                    <asp:LinkButton ID="lbtnCloseWin" runat="server" OnClick="lbtnCloseWin_Click">[X]</asp:LinkButton>
                </div>
                <div style="margin-top: -16px;">
                    <a href="javascript:return 0" onmouseover="showarchs(true)" onmouseout="showarchs(false)">У Вас есть устаревшие заявки 
                    <asp:Literal ID="lbQ2arch" runat="server"></asp:Literal>
                        шт</a>.<br />
                    Это заявки, которые больше месяца остаются без Вашего внимания.<br />
                    В списках они помечены кнопочкой
                <img onmouseover="showarchs(true)" onmouseout="showarchs(false)" src="../simg/16/time_go.png" title="При нажатии на эту кнопку Заявка будет перемещена в Архив. В будущем эту Заявку всегда можно будет вернуть к жизни." />.<br />
                    <asp:LinkButton ID="lbtnSet2Arch" runat="server" OnClick="lbtnSet2Arch_Click" CssClass="button" Visible="false">Нажмите эту кнопку, чтобы перенести эти заявки в Архив.</asp:LinkButton>
                </div>
            </asp:Panel>


            <asp:Literal ID="litInfo" runat="server"></asp:Literal>


            <asp:Repeater ID="rpTeo" runat="server" >
                <HeaderTemplate>
                    <h2>Запланированные доставки</h2>
                    <div>
                </HeaderTemplate>
                <ItemTemplate>
                    <div class="floatLeft border shadow padding5" style="width:250px; margin:5px;">
                        <p > !<span class="orangefon white bold"><%# Eval("TeoDate","{0:d}") %></span></p>
                        <p title="<%# Eval("Name") %>">
                            <a onclick="openflywin('../order/detailfly.aspx?id=<%# Eval("Id") %>', 870, 850, '<%# Eval("Name") %>')" href='javascript:return false;'><%# Eval("Name") %></a>
                           
                            

                        </p>
                        <p><%# Eval("TeoTrans") %></p>
                        <p>адрес доставки <strong><%# Eval("TeoAddress") %></strong></p>
                    </div>

                </ItemTemplate>
                <FooterTemplate></div><div class="clearBoth"></div></FooterTemplate>
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
  </div>   <br/>       
            <asp:HiddenField ID="hdRefresh" ClientIDMode="Static" runat="server" />
            <ul class="f-nav f-nav-tabs" style="font-size: 100%; font-weight: bold;">
                <%--<li id="tab1" runat="server" class="active">
                    <asp:LinkButton ID="lbtnShowActual" runat="server" OnClick="lbtnShowActual_Click"><span class="ipadmini">1</span><span class="ipadmini">2</span><span class="ipadmini">3</span> Заявки на согласовании</asp:LinkButton>

                </li>--%>
                <li id="tab1_1" runat="server" class="active" style="line-height: 22px !important;">
                    <asp:LinkButton CssClass="slowfunc" ID="lbtnShowNU" runat="server" OnClick="lbtnShowActualNU_Click" ToolTip="Заявки новые. Ожидают предварительного подтверждения клиентом.">
                        
                        Новые <asp:Label ID="qNU" CssClass="ipadmini" Visible="False" runat="server" Text=""></asp:Label>
                        <br/><span class="micro">(Ожидание решения клиента)</span>
                    </asp:LinkButton>

                </li>
                <li id="tab1_2" runat="server">
                    <asp:LinkButton CssClass="slowfunc" ID="lbtnShowAZ" runat="server" OnClick="lbtnShowActualAZ_Click" ToolTip="Заявки в процессе согласования в Сантехкомплект">
                        
                        На согласовании <asp:Label ID="qAZ" CssClass="ipadmini" Visible="False" runat="server" Text=""></asp:Label>
                        <br/><span class="micro">&nbsp;</span>
                    </asp:LinkButton>

                </li>
                <li id="tab1_3" runat="server" style="line-height: 22px !important;">
                    <asp:LinkButton ID="lbtnShowS" CssClass="slowfunc" runat="server" OnClick="lbtnShowActualS_Click" ToolTip="Заявки согласованы в Сантехкомплект и ожидают окончательного подтверждения клиентом">
                        
                        Согласованы <asp:Label ID="qS" CssClass="ipadmini" Visible="False" runat="server" Text=""></asp:Label>
                        <br/><span class="micro">(Ожидание решения клиента)</span>
                    </asp:LinkButton></li>

                <li id="tab2" runat="server" style="line-height: 22px !important;">
                    <asp:LinkButton ID="lbtnShowwork" CssClass="slowfunc" runat="server" OnClick="lbtnShowwork_Click" ToolTip="Заявки на комплектации /частично или полностью готовые к отгрузке ">
                        
                        На комплектации <asp:Label ID="qMR" CssClass="ipadmini" Visible="False" runat="server" Text=""></asp:Label>
                        <br/>/<span class="micro">готовы к отгрузке</span>
                    </asp:LinkButton>

                </li>
                <li id="tab3" runat="server">
                    <asp:LinkButton ID="lbtnShowarch" CssClass="slowfunc" runat="server" OnClick="lbtnShowarch_Click">Архив заявок
                        <br/><span class="micro">&nbsp;</span>

                    </asp:LinkButton></li>
                
                <li id="tab4" runat="server">
                    <asp:LinkButton ID="lbtnShowPrice" CssClass="slowfunc" runat="server" OnClick="lbtnShowPrice_Click">Прайс-листы
                        <br/><span class="micro">&nbsp;</span>

                    </asp:LinkButton></li>
                

                <%--<li id="tab4" runat="server">
                    <asp:LinkButton ID="lbtnShowdeleted" runat="server" OnClick="lbtnShowdel_Click">Отмененные заявки</asp:LinkButton></li>--%>
            </ul>



            <asp:MultiView ID="mvOrders" runat="server">
                <asp:View ID="vNU" runat="server">
                    <uc1:ucOrdersTable ID="ucOrdersTable_NU" runat="server" TitleList="Заявки новые. Ожидают Вашего предварительного подтверждения" />
                </asp:View>
                <asp:View ID="vAZ" runat="server">
                    <uc1:ucOrdersTable ID="ucOrdersTable_Z" runat="server" TitleList="Заявки в процессе согласования в Сантехкомплекте" />
                </asp:View>
                <asp:View ID="vS" runat="server">
                    <uc1:ucOrdersTable ID="ucOrdersTable_S" runat="server" TitleList="Заявки согласованы в Сантехкомплекте и ожидают Вашего окончательного подтверждения" />
                </asp:View>
                <asp:View ID="vMR" runat="server">
                    <uc1:ucOrdersTable ID="ucOrdersTable_R" runat="server" TitleList="Заявки на комплектации" />
                </asp:View>
                <asp:View ID="vFD" runat="server">
                    <uc1:ucOrdersTable ID="ucOrdersTable_F" runat="server" TitleList="Выполненные заявки" />
                    <uc1:ucOrdersTable ID="ucOrdersTable_D" runat="server" TitleList="Отмененные заявки и архивные, не пошедшие в дело" />
                </asp:View>
                <asp:View ID="vPrices" runat="server">
                    <asp:DataGrid ID="dgQuery" runat="server" AutoGenerateColumns="False"
            CellPadding="6"
            OnItemDataBound="dgQuery_ItemDataBound"
            AllowPaging="True"
            OnPageIndexChanged="dgQuery_PageIndexChanged"
            ForeColor="#333333"
            GridLines="None"
            CellSpacing="-1"
            Width="100%"
            OnItemCreated="dgQuery_ItemCreated"
            >
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
                <asp:TemplateColumn ItemStyle-CssClass="small"  HeaderText="Подробности">
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
                    

                </asp:View>

            </asp:MultiView>












        </asp:View>
    </asp:MultiView>


    <div></div>
    <asp:Repeater ID="rpOffer" runat="server">
        <HeaderTemplate>
            <h2 class="white orangefon padding5 center">Распродажа остатков!!!</h2><div id="reclContainer">
        </HeaderTemplate>
        <ItemTemplate>
            <div style="float: left;" class="reclamaplace" id="g_<%#Eval("GoodCode") %>">
                <table>
                    <tr>
                        <td>
                            <img src="<%#Eval("img") %>" width="100" /></td>
                        <td>
                            <div class="reclama-price"><%#Eval("price") %>р.</div>
                            <br />
                            <br />
                            <div><a class="bold" href="order/orderdefault.aspx?act=recl&code=<%#Eval("GoodCode") %>">купить</a></div>
                        </td>
                    </tr>
                    <tr>
                        <td colspan="2"><a class="reclama-title" href="javascript:return 0;" onclick="openflywin('http://santex-ens.webactives.ru/get/<%#Eval("ens") %>/?key=x9BS1EFXj0', 800,700,'Описание товара')"><%#Eval("Name") %></a></td>
                    </tr>
                    <tr>
                        <td colspan="2" class="right"></td>
                    </tr>
                </table>
            </div>

        </ItemTemplate>
        <FooterTemplate>
            <div style="clear: both;"></div>
            </div>
        </FooterTemplate>
    </asp:Repeater>

    
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

</asp:Content>
