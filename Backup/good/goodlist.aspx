<%@ Page Title="" Language="C#" MasterPageFile="~/common/mainpage.Master" AutoEventWireup="true"
    CodeBehind="goodlist.aspx.cs" Inherits="wstcp.goodlist" %>

<%@ Register Src="~/UC/TabControl.ascx" TagName="TabControl" TagPrefix="uc1" %>
<%@ Register Src="../UC/CommandPanel4Profile.ascx" TagName="CommandPanel4Profile"
    TagPrefix="uc2" %>
<asp:Content ID="Content1" ContentPlaceHolderID="head" runat="server">
    <link href="../tooltip.css" rel="stylesheet" type="text/css" />
    <style type="text/css">
        .qtyfield
        {
            width: 50px;
        }
        .descritem
        {
            width: 150px;
        }
        .blocktn
        {
            font-size: 100%;
            margin: 5px;
            padding: 5px;
            border: 1px dotted #BBBBBB;
        }
        .blocktk
        {
            padding-left: 0px;
        }
        .blocktk ul li
        {
            /*list-style-type:none; */
            display: inline-block;
        }
        label.check
        {
            font-weight: bold;
            color: #0000FF;
            font-size: 90%;
        }
        label.uncheck
        {
            font-weight: normal;
            color: #777777;
            font-size: 90%;
        }
        label.finded
        {
            background-color: Orange;
            color: #000000;
        }
        img.arrowtolist
        {
            border: 0;
            vertical-align: middle;
        }
        img.arrowtolist:hover
        {
            border: 0;
            vertical-align: middle;
            background-image: url('../simg/32/arrow16x32sh.png');
        }
        .selitem
        {
            background-color: #FFD991;
        }
        .itemover
        {
            background-color: #eef;
        }
        
        .goodincash-little
        {
            font-size: smaller;
            color: Green;
            font-weight: normal;
        }
        .goodincash-full
        {
            font-size: smaller;
            color: Green;
            font-weight: bold;
        }
        .goodbyorder
        {
            font-size: smaller;
            color: Navy;
        }
        .goodsoon
        {
            font-size: smaller;
            color: Olive;
        }
    </style>
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
    <script type="text/javascript">
        function tkInStack(thischeckboxid, val) {

            var thStack = $("#tkstack");
            var chbox = $("#" + thischeckboxid);
            var curval = thStack.val();
            
            if (chbox.prop("checked")) {
                if (curval.indexOf(val) < 0)
                    thStack.val(curval + ((curval != "") ? "," : "") + val);
            }
            else {
                if (curval.indexOf(val) >= 0) {
                    //alert("из " + curval + " убираем " + val);
                    thStack.val(curval.replace("," + val, "").replace(val, "").replace(",,", ","));
                }
                if (thStack.val().indexOf(",") == 0)
                    thStack.val(thStack.val().substring(1));


            }
        }
        function itemInStack(thischeckboxid, val) {

            var thStack = $("#itemstack");
            var chbox = $("#" + thischeckboxid);
            var curval = thStack.val();

            if (chbox.prop("checked")) {
                if (curval.indexOf(val) < 0)
                    thStack.val(curval + ((curval != "") ? "," : "") + val);
            }
            else {
                if (curval.indexOf(val) >= 0)
                    thStack.val(curval.replace("," + val, "").replace(val, "").replace(",,", ","));

                if (thStack.val().indexOf(",") == 0)
                    thStack.val(thStack.val().substring(1));


            }
        }

        function shw(thischeckboxid, num) {
            var chbox = $("#" + thischeckboxid);

            if (chbox.prop("checked")) {
                $("#tr_" + num).addClass("selitem");
            }
            else {
                $("#tr_" + num).removeClass("selitem");
            }
        }
        function recount(goodid) {
            var q = $("#q_" + goodid).val();
            var pr = $("#pr_" + goodid).text();
            $("#sm_" + goodid).text("" + pr * q);
            $("#refresh").val("Y");
            $("form").submit();
        }

        $(document).ready(function () {
            $("tr[id*='tr_']").mouseover(function (e) { $(this).addClass('itemover'); });
            $("tr[id*='tr_']").mouseout(function (e) { $(this).removeClass('itemover'); });
        });


        /*
        $(document).ready(function () {
        $(".menu_o").hide();
        });


        $(document).ready(function () {
        $('#tns').hide();
        });

        $(document).ready(function () {

        $(".atns").click(function (e) {
        var offset = $(this).offset();
        var relX = (e.pageX - offset.left);
        var relY = (e.pageY - offset.top);
        $('#tns').css("top", e.relY);
        $('#tns').css("left", e.relX);

        $('#tns').show();
        });
        $(':not(.atns) :not(#tns)').click(function (e) {
        $('#tns').hide('fast');
        });

        $(".atns").mouseover(function (e) {

        $('#tns').show('fast');
        });


        $(':not(.menu_o)').click(function (e) {
        $('.menu_o').hide('fast');
        });
        });

        */

        function findtks() {
            $("label[for*='ch_tk']").removeClass('finded');
            var txs = "" + $("#searchtks").val();
            if (txs.length < 3) return;
            $("input[value*='" + txs + "']").each(function () {
                $("label[for='" + this.id + "']").addClass('finded');
            });
        }
    </script>
    <style type="text/css">
        #tns
        {
            position: absolute;
            background-color: #EFF3FB;
            padding: 5px;
            border: 1px solid #999999;
            line-height: 14pt;
            display: none;
        }
    </style>
</asp:Content>
<asp:Content ID="left" ContentPlaceHolderID="place_Left" runat="server">
    <asp:Panel ID="pnlForSubj" runat="server">
        <asp:Repeater ID="rpTAs" runat="server">
            <HeaderTemplate>
            <div class="ipad shadow">
                <h3>
                    Ваш персональный менеджер</h3>
            </HeaderTemplate>
            <ItemTemplate>
                <asp:Literal ID="lbPhoto" runat="server"></asp:Literal>
                <div><img src="<%#Eval("photo") %>" /></div>
                <div><%#Eval("Name") %>
                    <asp:Literal ID="lbTA" runat="server"></asp:Literal></div>
                <div>
                    тел.<%#Eval("Phone") %><asp:Literal ID="lbPhone" runat="server"></asp:Literal></div>
                <div>
                    email <%#Eval("email") %>
                    <asp:Literal ID="lbEmail" runat="server"></asp:Literal><img src="../simg/16/email.png" ondblclick="openflywin('../common/gimail.aspx', 400, 300, 'Быстрое сообщение')" alt='email' title='быстрое сообщение'/></div>
            </ItemTemplate>
            <SeparatorTemplate>
                <hr />
            </SeparatorTemplate>
            <FooterTemplate></div></FooterTemplate>
        </asp:Repeater>
    </asp:Panel>
    <asp:Repeater ID="rpOrders" runat="server">
        <HeaderTemplate>
            <h4>
                Ранее сформированные заявки</h4>
        </HeaderTemplate>
        <ItemTemplate>
            <div class="ipad shadow" style="background-color:#Ffffe9;">
                <div class='micro' id='menu_o_<%#Eval("ID") %>' style="text-align: right">
                    <%#Eval("linkchange")%></div>
                <a style="padding: 5px;" title='от ' href='#' class="bold" onclick="openflywin('../order/detailfly.aspx?id=<%#Eval("ID") %>', 850, 500, 'Заявка <%#Eval("Name") %> от <%#Eval("RegDate") %>')">
                    заявка №<%#Eval("ID") %><br />от
                    <%#Eval("RegDate") %><br />
                    <span class="small">
                        <%#Eval("Name") %></span></a><br />
                <span class="small">сумма
                    <%#Eval("SummOrder") %>р.</span><br />
                <span class="small ">
                    <%#Eval("State") %></span><br />
            </div>
        </ItemTemplate>
        <SeparatorTemplate>
            <br />
        </SeparatorTemplate>
        <FooterTemplate>
        </FooterTemplate>
    </asp:Repeater>
    <asp:Repeater ID="rpArchive" runat="server">
        <HeaderTemplate>
            <h4>
                Архив</h4>
        </HeaderTemplate>
    </asp:Repeater>
</asp:Content>
<asp:Content ID="Content3" ContentPlaceHolderID="ContentPlaceHolder1" runat="server">
    <asp:LinkButton ID="btnHideExistsOrders" runat="server" CommandArgument="hide" OnClick="btnHideExistsOrders_Click"><< скрыть свои заявки</asp:LinkButton>
    <h2>
        Самостоятельный подбор заявки</h2>
    <table width="100%">
        <tr>
            <td style="padding: 5px; vertical-align:top;">
                <asp:HiddenField ClientIDMode="Static" ID="itemstack" runat="server" />
                <asp:HiddenField ClientIDMode="Static" ID="tkstack" runat="server" />
                <div class="message small">Новая функция! Двойной клик на ячейке Наличие или Цена - это быстрый запрос менеджеру по товару.</div>
                <div class="message">
                    <asp:Literal ID="lbMessage" runat="server"></asp:Literal></div>
                <uc1:TabControl ID="TabControl1" runat="server" />
                <div style="padding: 5px;" class="tabcontrol">
                    <asp:Panel ID="pnlStruct" runat="server">
                        <div class="italic">
                            найти категорию
                            <input class="italic" type="text" id='searchtks' onkeyup="findtks()" onchange="javascript:return false;"
                                value=''></div>
                        <asp:Literal ID="struct_place" runat="server"></asp:Literal>
                    </asp:Panel>
                    <asp:Panel ID="pnlList" runat="server">
                        <h2 class="floatLeft">
                            <asp:Literal Visible="false" ID="lbSelTN" runat="server"></asp:Literal>
                            <!--
                            <a href="#" title="выбрать товарное направление " onclick="showtns()" class='atns small'>
                                выбрать товарное направление ...</a>
                            -->
                        </h2>
                        <div class="clearBoth">
                        </div>
                        <div class="grayfon shadow " id="tns" style="width: 800px;">
                            <h3>
                                Выбор товарного направления</h3>
                            <div style="text-align: left;">
                                <asp:DataList ID="DataList1" runat="server" OnItemCommand="DataList1_SelectedIndexChanged"
                                    RepeatLayout="Flow" RepeatDirection="Horizontal">
                                    <HeaderTemplate>
                                        <asp:LinkButton ID="LinkButton1" CssClass="small linkbutton " runat="server" CommandName="select"
                                            CommandArgument='' Text='ВСЕ'></asp:LinkButton></HeaderTemplate>
                                    <ItemTemplate>
                                        <asp:LinkButton ID="LinkButton1" CssClass="small linkbutton " runat="server" CommandName="select"
                                            CommandArgument='<%# Eval("xTN") %>' Text='<%# Eval("xTN") %>'></asp:LinkButton></ItemTemplate>
                                    <SelectedItemTemplate>
                                        <asp:LinkButton CssClass="linkbutton bold selected shadowdirect" ID="LinkButton1"
                                            runat="server" CommandName="select" CommandArgument='<%# Eval("xTN") %>' Text='<%# Eval("xTN") %>'></asp:LinkButton></SelectedItemTemplate>
                                </asp:DataList>
                                <br />
                                <a href="#">отмена</a>
                            </div>
                        </div>
                        <div class="grayfon bold small" style="padding: 3px">
                            Товарная категория:
                            <asp:DropDownList ID="dlTK" runat="server" AutoPostBack="true" Width="120px" OnSelectedIndexChanged="dlTK_SelectedIndexChanged">
                            </asp:DropDownList>
                            &nbsp;&nbsp;Номенклатура:
                            <asp:DropDownList ID="dlNames" runat="server" AutoPostBack="true" OnSelectedIndexChanged="txSearch_TextChanged"
                                Width="120px">
                            </asp:DropDownList>
                            &nbsp;&nbsp;Бренд:
                            <asp:DropDownList ID="dlBrends" runat="server" AutoPostBack="true" Width="120px"
                                OnSelectedIndexChanged="dlBrends_SelectedIndexChanged">
                            </asp:DropDownList>
                            <br />
                            поиск
                            <asp:TextBox ID="txSearch" runat="server" Width="250px" MaxLength="150" OnTextChanged="txSearch_TextChanged"
                                AutoPostBack="True"></asp:TextBox><asp:Button ID="Button1" runat="server" Text="найти" />
                        </div>на странице <asp:RadioButtonList ID="rbPageSize" runat="server" AutoPostBack="true"
                            RepeatDirection="Horizontal" RepeatLayout="Flow" 
                            onselectedindexchanged="rbPageSize_SelectedIndexChanged">
                        <asp:ListItem Text="15" Value="15" Selected></asp:ListItem>
                        <asp:ListItem Text="30" Value="30"></asp:ListItem>
                        <asp:ListItem Text="50" Value="50"></asp:ListItem>
                        </asp:RadioButtonList> позиций
                        <asp:DataGrid ID="dgList" CssClass="grid" runat="server" AllowPaging="True" PageSize="15"
                            OnPageIndexChanged="dgList_PageIndexChanged" AutoGenerateColumns="False" CellPadding="4"
                            OnItemDataBound="dgList_ItemDataBound" Width="100%">
                            <Columns>
                                <asp:BoundColumn DataField="id" Visible="False"></asp:BoundColumn>
                                <asp:TemplateColumn HeaderText="выбор"></asp:TemplateColumn>
                                <asp:BoundColumn DataField="goodcode" HeaderText="код 1С"></asp:BoundColumn>
                                <asp:BoundColumn DataField="name" HeaderText="наименование"></asp:BoundColumn>
                                <asp:TemplateColumn HeaderText="наличие"></asp:TemplateColumn>
                                <asp:TemplateColumn HeaderText="цена, (руб.)"></asp:TemplateColumn>
                                <asp:TemplateColumn HeaderText=""></asp:TemplateColumn>
                            </Columns>
                            <HeaderStyle CssClass="mainbackcolor small bold white" />
                            <FooterStyle CssClass="mainbackcolor small bold white" />
                            <PagerStyle CssClass="mainbackcolor bold white mypager" HorizontalAlign="Center"
                                Font-Size="Large" Mode="NumericPages" Position="TopAndBottom" />
                            <SelectedItemStyle BackColor="#D1DDF1" Font-Bold="True" ForeColor="#333333" />
                        </asp:DataGrid>
                        
                    </asp:Panel>
                    <asp:Panel ID="pnlOrder" runat="server">
                        <div class="message">
                            <asp:Literal ID="lbMess" runat="server"></asp:Literal></div>
                        <asp:HiddenField ClientIDMode="Static" ID="refresh" runat="server" />

                        <div id="divorder">
                        <div class="border" style="margin:2px;padding:2px;">
                        Контрагент
                        <asp:TextBox ID="txNameSubject" runat="server"></asp:TextBox>, ИНН
                        <asp:TextBox ID="txINN" runat="server"></asp:TextBox>, код
                        <asp:TextBox ID="txCodeSubject" runat="server"></asp:TextBox></div>
                        <div class="border" style="margin:2px;padding:2px;">
                        <div>
                            Заявка № <span id="the_ord" class="bold">
                                <asp:Literal ID="lbOrder" runat="server"></asp:Literal></span> от
                            <asp:Literal ID="lbRegDate" runat="server"></asp:Literal>
                        </div>
                        <div>Код в учетной системе <span class="bold">
            <asp:Literal ID="lbCode" runat="server"></asp:Literal></span></div>
                        <div class="bold">
                            Наименование заявки
                            <asp:TextBox ID="txNameOrder" runat="server" MaxLength="150" Width="300px"></asp:TextBox></div>
                        <div>
                            Статус заявки
                            <asp:Literal ID="lbStateOrder" runat="server"></asp:Literal></div>
</div>

                        <asp:DataGrid ID="dgOrder" runat="server" AutoGenerateColumns="False" CellPadding="4"
                            CssClass="grid" OnItemDataBound="dgOrder_ItemDataBound" ShowFooter="True" OnDeleteCommand="dgOrder_DeleteCommand"
                            Width="100%">
                            <Columns>
                                <asp:BoundColumn DataField="goodid" Visible="False">
                                    <HeaderStyle Width="16px" />
                                </asp:BoundColumn>
                                <asp:ButtonColumn CommandName="delete" DataTextField="" Text="&lt;img title='удалить' src='../simg/16/delete.png'&gt;">
                                </asp:ButtonColumn>
                                <asp:BoundColumn DataField="goodcode" Visible="true" HeaderText="код"></asp:BoundColumn>
                                <asp:BoundColumn DataField="name" HeaderText="Наименование"></asp:BoundColumn>
                                <asp:BoundColumn DataField="pr" HeaderText="цена" DataFormatString="{0:N}">
                                    <HeaderStyle Width="60px" />
                                    <ItemStyle Font-Bold="False" Font-Italic="False" Font-Overline="False" Font-Strikeout="False"
                                        Font-Underline="False" HorizontalAlign="Right" />
                                </asp:BoundColumn>
                                <asp:BoundColumn DataField="qty" HeaderText="кол-во">
                                    <HeaderStyle Width="60px" />
                                </asp:BoundColumn><asp:BoundColumn DataField="ed" HeaderText="">
                                    <HeaderStyle Width="20px" />
                                </asp:BoundColumn>
                                <asp:BoundColumn DataField="sum" HeaderText="стоимость" DataFormatString="{0:N2}">
                                    <HeaderStyle Width="60px" />
                                    <ItemStyle Font-Bold="False" Font-Italic="False" Font-Overline="False" Font-Strikeout="False"
                                        Font-Underline="False" HorizontalAlign="Right" />
                                </asp:BoundColumn>
                                <asp:TemplateColumn HeaderText="Примечание"></asp:TemplateColumn>
                                <asp:BoundColumn DataField="comment" HeaderText="comment"></asp:BoundColumn>
                            </Columns>
                            <FooterStyle CssClass="mainbackcolor bold white" />
                            <HeaderStyle CssClass="mainbackcolor bold white" />
                            <PagerStyle CssClass="mypager" HorizontalAlign="Center" />
                        </asp:DataGrid>
                        <h4>
                            Примечание</h4>
                        &nbsp;<asp:TextBox ID="txDescr" runat="server" TextMode="MultiLine" Width="500px"></asp:TextBox>
                        </div>
                        <br />
                        <asp:LinkButton ID="lbtnSave" runat="server" CssClass="button" OnClick="btnSaveOrder_Click"
                            Visible="true" ><img alt="[v]" src="../simg/16/save.png"/> сохранить заявку</asp:LinkButton>
                        <asp:LinkButton ID="lbtnNew" runat="server" CssClass="button" OnClick="btnNewOrder_Click"><img alt="[c]" src="../simg/16/add.png"/> новая заявка</asp:LinkButton>
                        &nbsp;
                        <asp:LinkButton ID="lbtnDelete" runat="server" CssClass="button" OnClick="btnDelorder_Click"
                            OnClientClick="javascript:if(!confirm('Вы подтверждаете удаление заявки?')) return false;"><img alt="[x]" src="../simg/16/delete.png"/> удалить заявку</asp:LinkButton>
                        &nbsp;<a href="#" onclick="printBlock('divorder')" class="button"><img src="../simg/16/printer.png" /> распечатать</a>
                    </asp:Panel>
                </div>
            </td>
        </tr>
    </table>
    <script type="text/javascript">
        function checksaved() {
            var ret = false;
            if ($('.itemstack').val() != '' && $("span[id*='the_ord']").text().trim() == "") {
                ret = confirm('реально все очистить?');
            }
            else {
                ret = true;
                alert($("span[id*='the_ord']").text().trim());
            }
            return ret;
        }

        $("label").bind("click", selchks);
        $("input[name='ch_tks']").bind("click", selchks);





        function selchks() {

            $("input[name='ch_tks']").each(function () {
                var a = '#' + this.id;
                if ($(a).is(":checked")) {
                    $("label[for='" + this.id + "']").removeClass('uncheck');
                    $("label[for='" + this.id + "']").addClass('check');

                }
                else {
                    $("label[for='" + this.id + "']").removeClass('check');
                    $("label[for='" + this.id + "']").addClass('uncheck');

                }
            });

            //            $("input[name='ch_tks']").filter('input:checked').each(function () {
            //                $("label[for='" + this.id + "']").css("color", "blue");
            //            });
        }
    </script>
</asp:Content>
