<%@ Page Title="" Language="C#" MasterPageFile="common/mainpage.Master" AutoEventWireup="true"
    CodeBehind="default.aspx.cs" Inherits="wstcp._default" %>

<asp:Content ID="Content1" ContentPlaceHolderID="head" runat="server">
    <style type="text/css">
        .reclamaplace
        {
            border: 2px dotted #AAAAAA;
            padding: 8px;
            margin: 5px;
            text-align: center;
            vertical-align: middle;
            font-size: 24pt;
            font-weight: bold;
            color: #AAAAAA;
        }
        .picover
        {
            width:25% !important;
            /*height:300% !important;*/
            
        }
        .pic-
        {
            width:15%;
            /*height: 24px;*/
        }
        .pic-ar
        {
            width:80px;
            height:32px;
        }
        
        
    </style>
    <script type="text/javascript">
        $(document).ready(function () {

            $(".pic").mouseover(function (e) { $(this).addClass('picover'); });
            $(".pic").mouseout(function (e) { $(this).removeClass('picover');  });

        });
    
    </script>
</asp:Content>
<asp:Content ID="Content2" ContentPlaceHolderID="ContentPlaceHolder1" runat="server">
    <h2>
        На этом сайте</h2>
    <p>
        Можно самостоятельно оформить заказ на СТИ в компании ООО "УЦСК "Сантехкомплект-Урал"</p>
    <asp:Panel ID="pnlScreens" runat="server">
 <div style="text-align:center;">
    <img class="pic-" src='../media/screens/s1.jpg' alt='s1' title="Определиться с товарным направлением" >
    <img class="pic-ar" src='../media/screens/ar_right.jpg' alt='s2' >
    <img class="pic-" src='../media/screens/s2.jpg' alt='s2' title="Выбрать номенклатуру, используюя фильтры и поиск" >
    <img class="pic-ar" src='../media/screens/ar_right.jpg' alt='s2' >
    <img class="pic-" src='../media/screens/s3.jpg' alt='s3'   title="Указать требуемое количество. Добавить к заявке наименование и комментарий. Сохранить">
    <img class="pic-ar" src='../media/screens/ar_right.jpg' alt='s2' ><br />
    <img class="pic-ar" src='../media/screens/ar_right.jpg' alt='s2' >
    <img class="pic-" src='../media/screens/s4.jpg' alt='s4'  title="Заявка автоматически попадет в учетную систему УЦСК ">    
    <img class="pic-ar" src='../media/screens/ar_right.jpg' alt='s2' >
    <img class="pic-" src='../media/screens/s5.jpg' alt='s5' title="и через несколько мунут в Вашей заявке будут указаны цены и скидки предоставляемые по условиям договора">
    <img class="pic-ar" src='../media/screens/ar_right.jpg' alt='s2' >
    <img class="pic-" src='../media/screens/s6.jpg' alt='s6' title="После этого наш менеджер свяжется с Вами для выставления счета на оплату"><br /></div>
     </asp:Panel>
    
    
    
    <br/><br/>
    <asp:MultiView ID="MultiView1" runat="server">
        <asp:View ID="vLogin" runat="server">
        <p>Это доступно только для зарегистрированных клиентов.<br/>
Обратитесь в компанию за получением кода доступа.<br/>
ООО "УЦСК "Сантехкомплект-Урал" тел. (343) 270-04-04</p>
            <table class="profile shadow ui-dialog-content">
                <tr>
                    <td colspan="2">
                        <div class="message">
                            <asp:Literal ID="lbMessageLogin" runat="server"></asp:Literal></div>
                    </td>
                </tr>
                <tr>
                    <td>
                        логин (e-mail)
                    </td>
                    <td>
                        <asp:TextBox ID="txLogin" runat="server" Width="150px" MaxLength="150"></asp:TextBox>
                    </td>
                </tr>
                <tr>
                    <td>
                        пароль
                    </td>
                    <td>
                        <asp:TextBox ID="txPwd" runat="server" TextMode="Password" Width="150px" MaxLength="150"></asp:TextBox>
                    </td>
                </tr>
                <tr>
                    <td colspan="2" class="center">
                        <asp:Button ID="btnLogin" runat="server" Text="вход" OnClick="btnLogin_Click" Width="75px" />
                    </td>
                </tr>
            </table>
        </asp:View>
        <asp:View ID="vOrders" runat="server">
            <asp:Literal ID="litInfo" runat="server"></asp:Literal>
            <asp:Repeater ID="rpOrders" runat="server">
                <HeaderTemplate>
                    <h3>
                        Сохраненные заявки</h3>
                    
                </HeaderTemplate>
                <ItemTemplate>
                    <div class="floatLeft ipad shadow small" style="width:120px; margin:10px; padding:6px;" title="<%#Eval("SubjectName") %>">
                        
                        <a title='' onclick="openflywin('../order/detailfly.aspx?id=<%#Eval("ID") %>', 850, 500, 'Заявка <%#Eval("Name") %> от <%#Eval("RegDate") %>')"
                            href='#'><span class="bold"><%#Eval("Name") %></span><br/>№<%#Eval("ID") %>от
                            <%#Eval("RegDate") %></a><br/>
                        <%#Eval("State") %>
                        <br/>
                        <%#Eval("linkchange") %></div></ItemTemplate>
                <FooterTemplate>
                    <div class="clearBoth"></div></FooterTemplate>
            </asp:Repeater>
            <asp:Repeater ID="rpArchive" runat="server">
            <HeaderTemplate><h4>Архив</h4></HeaderTemplate>
            </asp:Repeater>
        </asp:View>
    </asp:MultiView>
    
      
    <div>
    <h2>Распродажа прошлогодней коллекции!!!</h2>
        <div style="float: left; width: 200px;" class="reclamaplace">
            <a href="good/goodlist.aspx?act=recl&code=b0100021594"><img src="media/recl/b0100021594.png" /></a></div>
            <div style="float: left; width: 200px;" class="reclamaplace">
            <a href="good/goodlist.aspx?act=recl&code=b0100007323"><img src="media/recl/b0100007323.png" /></a></div>
        
        <div style="float: left; width: 200px;" class="reclamaplace">
            <a href="good/goodlist.aspx?act=recl&code=b0100030051"><img src="media/recl/b0100030051.png" /></a></div>
        <div style="float: left; width: 200px;" class="reclamaplace">
            <a href="good/goodlist.aspx?act=recl&code=b0100015906"><img src="media/recl/b0100015906.png" /></a></div>
        <div style="clear: both;">
        </div>
    </div>
    <!--
    <h2>Функции web-сервисаа</h2>
    <h3>
        начало работы</h3>
    <ul>
        <li>InitConnection()</li>
    </ul>
    <h3>
        получение информации</h3>
    <ul>
        <li>GetGoodID()</li>
        <li>wGetIncashTable(string listgoodcodes) - return Table: OwnerId, OwnerName, GoodCode, zn, zn_z, BasePrice, Qty</li>
        <!--<li>GetGoodesQtyByGoodID(int good_id, int wh_owner_id, int warehouse_id = 0)</li>
        <li>GetGoodesQtyByGoodCode(string good_code, int wh_owner_id, int warehouse_id = 0)</li>
        
        <li>GetLastupdateQtyByGoodID(int good_id, int wh_owner_id, int warehouse_id = 0)</li>
        <li>GetGoodesQtyTableByGoodID(int good_id, int wh_owner_id, int warehouse_id = 0)</li>
        <li>GetGoodies()</li>
        <li>GetGoodiesByOwner()</li>
    </ul>
    
    <h3>
        обновление данных</h3>
    <ul>
        <li>BulkUpdate(int goodsowner_id, string session_id, int warehouse_id, DataTable dt)</li>
        <li>SetGoodQtyByGoodCode()</li>
        <li>SetGoodQtyByGoodID()</li>
        <li>FinishUpdateQty()</li>
    </ul>-->
    <!--
    <h3>
        административные функции</h3>
    <ul>
        <li>AddNewGood(int owner_id, string owner_code, string name, string corrector )</li>
        <li>JoinToGood(int good_id, string good_code, int code_owner_id)</li>       
    </ul>-->
</asp:Content>
