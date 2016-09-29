<%@ Page Title="" Language="C#" MasterPageFile="~/common/mainpage.Master" AutoEventWireup="true"
    CodeBehind="goodlist.aspx.cs" Inherits="wstcp.goodlist" %>

<%@ Register Src="~/UC/TabControl.ascx" TagName="TabControl" TagPrefix="uc1" %>
<%@ Register Src="../UC/CommandPanel4Profile.ascx" TagName="CommandPanel4Profile"
    TagPrefix="uc2" %>
<%@ Register Src="../UC/CommentList.ascx" TagName="CommentList" TagPrefix="uc3" %>
<asp:Content ID="Content1" ContentPlaceHolderID="head" runat="server">
    <link href="../tooltip.css" rel="stylesheet" type="text/css" />
    <style type="text/css">
        .qtyfield {
            width: 50px;
        }

        .descritem {
            width: 150px;
        }

        .blocktn {
            font-size: 100%;
            margin: 5px;
            padding: 5px;
            border: 1px dotted #BBBBBB;
        }

        .blocktk {
            padding-left: 0px;
        }

            .blocktk ul li {
                /*list-style-type:none; */
                display: inline-block;
            }

        label.check {
            font-weight: bold;
            color: #0000FF;
            font-size: 90%;
        }

        label.uncheck {
            font-weight: normal;
            color: #777777;
            font-size: 90%;
        }

        label.finded {
            background-color: Orange;
            color: #000000;
        }

        img.arrowtolist {
            border: 0;
            vertical-align: middle;
        }

            img.arrowtolist:hover {
                border: 0;
                vertical-align: middle;
                background-image: url('../simg/32/arrow16x32sh.png');
            }

        .selitem {
            background-color: #FFD991;
        }

        .itemover {
            background-color: #eef;
        }

        .goodincash-little {
            font-size: smaller;
            color: Green;
            font-weight: normal;
        }

        .goodincash-full {
            font-size: smaller;
            color: Green;
            font-weight: bold;
        }

        .goodbyorder {
            font-size: smaller;
            color: Navy;
        }

        .goodsoon {
            font-size: smaller;
            color: Olive;
        }
    </style>
    <style type="text/css">
        .printSelected div {
            display: none;
        }
            /* скрываем весь контент на странице */
            .printSelected div.printSelection {
                display: block;
            }
                /* делаем видимым только тот блок, который подготовлен для печати */
                .printSelected div.printSelection div {
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
        #tns {
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
</asp:Content>
<asp:Content ID="Content3" ContentPlaceHolderID="ContentPlaceHolder1" runat="server">
    <asp:Panel ID="pnlForSubj" runat="server">
        <asp:HiddenField ID="hdSubjectID" runat="server" />
        <asp:Repeater ID="rpTAs" runat="server" Visible="false">
            <HeaderTemplate>
                <div class="">
                    <span>Ваш персональный менеджер:</span>
            </HeaderTemplate>
            <ItemTemplate>

                <span>
                    <img src="<%#Eval("photo") %>" />

                    <strong><%#Eval("Name") %>
                    </strong>, 
                 
                    тел.<%#Eval("Phone") %>, 
                 
                    email <%#Eval("email") %>
                    <img src="../simg/16/email.png" ondblclick="openflywin('../common/gimail.aspx', 400, 300, 'Быстрое сообщение')" alt='email' title='быстрое сообщение' />
                </span>
            </ItemTemplate>
            <SeparatorTemplate>
                ;&nbsp;
            </SeparatorTemplate>
            <FooterTemplate></div></FooterTemplate>
        </asp:Repeater>
    </asp:Panel>
    <style type="text/css">
        .sourcelist-active {
            background-image: url("../simg/16/tick.png");
            padding-left: 20px !important;
            background-repeat: no-repeat;
            font-weight: bold !important;
            text-decoration: none !important;
            color: #0057A8;
            margin-left:8px;
            margin-right:8px;
        }

        .sourcelist-passive {
            font-weight: normal !important;
            text-decoration: underline !important;
            color: #636262;
            margin-left:8px;
            margin-right:8px;
        }
    </style>
    <asp:LinkButton ID="btnHideExistsOrders" runat="server" CommandArgument="hide" OnClick="btnHideExistsOrders_Click" Visible="false"><< скрыть свои заявки</asp:LinkButton>
    <!--<div><a href="#myorders">другие мои заявки</a> <a href="#blockArh">архивные заявки</a></div>-->
    <h2 style="margin-top:0;">Самостоятельный подбор заявки</h2>

    <div id="pnlSourceSelect" runat="server" visible="true" style="padding-bottom: 10px">
        <strong>Источник списка номенклатуры </strong>
        <asp:LinkButton ID="lnkJust" CommandArgument="just" runat="server" CssClass="" OnClick="lnkSelectREG_Click">Весь каталог</asp:LinkButton>
        <asp:LinkButton ID="lnkMyFavor" CssClass="" OnClick="lnkSelectREG_Click" CommandArgument="my" runat="server">Из истории моих заказов</asp:LinkButton>
        <asp:LinkButton CommandArgument="spec" CssClass="" ID="lnkSpecial" OnClick="lnkSelectREG_Click" runat="server">Распродажа!!!</asp:LinkButton>
        &nbsp;<asp:CheckBox ID="chIncash" runat="server" Text="из наличия" AutoPostBack="True" CssClass="bold" OnCheckedChanged="chIncash_CheckedChanged" />
    </div>
    <div class="message">
        <asp:Literal ID="lbReadonly" Visible="false" runat="server" Text="Выбранную заявку нельзя редактировать на сайте. Обратитесь к своему менеджеру."></asp:Literal>
    </div>
    <table width="100%">
        <tr>
            <td style="padding: 5px; vertical-align: top;">
                <asp:HiddenField ClientIDMode="Static" ID="itemstack" runat="server" />
                <asp:HiddenField ClientIDMode="Static" ID="tkstack" runat="server" />

                <div class="message">
                    <asp:Literal ID="lbMessage" runat="server"></asp:Literal>
                </div>
                <uc1:TabControl ID="TabControl1" runat="server" />
                <div style="padding: 5px;" class="tabcontrol">
                    <asp:Panel ID="pnlStruct" runat="server">
                        <div class="italic">
                            найти номенклатуру 
                            <asp:TextBox ID="txSimSearch" MaxLength="50" runat="server" Width="141px"></asp:TextBox><asp:Button ID="btnSimSearch" runat="server" Text="найти" OnClick="btnSimSearch_Click" />
                        </div>
                        <div class="italic">
                            найти товарную категорию
                            <input class="italic" maxlength="20" type="text" id='searchtks' onkeyup="findtks()" onchange="javascript:return false;"
                                value=''>
                        </div>


                        <asp:Literal ID="struct_place" runat="server"></asp:Literal>
                    </asp:Panel>
                    <asp:Panel ID="pnlList" runat="server">
                        <div class="message small">
                            <asp:Literal ID="lbListMessage" runat="server"></asp:Literal>
                        </div>

                        <h3 class="floatLeft">
                            <asp:Literal Visible="false" ID="lbSelTN" runat="server"></asp:Literal>
                            <asp:LinkButton ID="lbtnChangeTN" CssClass="italic micro bold" runat="server" OnClick="lbtnChangeTN_Click"><img src="../simg/16/reselect.png" alt="@" title="изменить товарное направление"/></asp:LinkButton>
                        </h3>
                        <div class="clearBoth">
                        </div>
                        <table width="100%">
                            <tr>
                                <td>
                                    <div class=" orangefon padding5 ">
                                        <div class="bold padding2 white">
                                            <div id="pnlSourceSelect2" runat="server" visible="false"  style="border-bottom: 1px solid #ffffff; margin-bottom: 4px; padding-bottom: 4px;">
                                                Источник списка номенклатуры:
                                                <asp:DropDownList ID="dlSource" runat="server" AutoPostBack="True" OnSelectedIndexChanged="dlSource_SelectedIndexChanged">
                                                    <asp:ListItem Value="all">Весь каталог</asp:ListItem>
                                                    <asp:ListItem Value="my">Из истории своих заказов</asp:ListItem>
                                                    <asp:ListItem Value="spec">!!! Распродажа</asp:ListItem>
                                                </asp:DropDownList>

                                            </div>
                                            
                                            Бренд:
                            <asp:DropDownList ID="dlBrends" runat="server" Width="120px" AutoPostBack="true"
                                OnSelectedIndexChanged="dlBrends_SelectedIndexChanged">
                            </asp:DropDownList>&nbsp;&nbsp;Категория:
                            <asp:DropDownList ID="dlTK" runat="server" Width="120px" AutoPostBack="true" OnSelectedIndexChanged="dlTK_SelectedIndexChanged">
                            </asp:DropDownList>
                                            &nbsp;&nbsp;Номенклатура:
                            <asp:DropDownList ID="dlNames" runat="server" OnSelectedIndexChanged="txSearch_TextChanged"
                                Width="120px">
                            </asp:DropDownList>
                                            &nbsp;
                            

                            <br />
                                        </div>
                                        <div class="white padding2">
                                            поиск по коду, артикулу или части названия
                            <asp:TextBox ID="txSearch" runat="server" Width="250px" MaxLength="150"></asp:TextBox><asp:Button Visible="false" ID="Button1" runat="server" Text="найти" OnClick="txSearch_TextChanged" />
                                        </div>
                                    </div>
                                </td>
                                <td width="120px">
                                    <style type="text/css">
                                        .acptfltr {
                                            height: 60px;
                                            border: solid 1px #669999;
                                            background-color: #0057A8;
                                            margin-left: 3px;
                                            color: #fff;
                                        }
                                    </style>
    <div class="">
        <asp:Button ID="btnAcptFlt" CssClass="acptfltr" runat="server" OnClick="btnAcptFlt_Click" Text="применить фильтр" />
    </div>
    <div class="clearBoth"></div>
    </td>
                            </tr></table>
                        
                        на странице
                        <asp:RadioButtonList ID="rbPageSize" runat="server" AutoPostBack="true"
                            RepeatDirection="Horizontal" RepeatLayout="Flow"
                            OnSelectedIndexChanged="rbPageSize_SelectedIndexChanged">
                            <asp:ListItem Text="15" Value="15" Selected></asp:ListItem>
                            <asp:ListItem Text="30" Value="30"></asp:ListItem>
                            <asp:ListItem Text="50" Value="50"></asp:ListItem>
                        </asp:RadioButtonList>
    позиций
                        <asp:DataGrid ID="dgList" CssClass="grid" runat="server" AllowPaging="True" PageSize="15"
                            OnPageIndexChanged="dgList_PageIndexChanged" AutoGenerateColumns="False" CellPadding="4"
                            OnItemDataBound="dgList_ItemDataBound" Width="100%" OnSortCommand="dgList_SortCommand">
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
                            <PagerStyle CssClass="bold mypager" HorizontalAlign="Center"
                                Font-Size="Large" Mode="NumericPages" Position="TopAndBottom" />
                            <SelectedItemStyle BackColor="#D1DDF1" Font-Bold="True" ForeColor="#333333" />
                        </asp:DataGrid>

    </asp:Panel>
                    <asp:Panel ID="pnlOrder" runat="server">
                        <div class="message">
                            <asp:Literal ID="lbMess" runat="server"></asp:Literal>
                        </div>
                        <asp:HiddenField ClientIDMode="Static" ID="refresh" runat="server" />





                        <div class="border center padding2">
                            <table width="100%">
                                <tr>
                                    <td width="200px">
                                        <asp:Literal ID="link4prn" runat="server"></asp:Literal></td>
                                    <td>Статус заявки <span class="bigsize">
                                        <asp:Literal ID="lbStateOrder" runat="server"></asp:Literal></span></td>
                                    <td width="200px" class="right">
                                        <asp:LinkButton ID="lbtnNextStady" runat="server" CssClass="button" OnClick="lbtnNextStady_Click">-&gt;</asp:LinkButton>
                                        <asp:LinkButton ID="lbtnCancel" runat="server" CssClass="button" OnClick="lbtnDelete_Click" OnClientClick="javascript: return myConfirm('Вы подтверждаете удаление заявки?')">Отменить заявку</asp:LinkButton></td>
                                </tr>
                            </table>

                        </div>
                        <div class="border padding2">
                            <table width="100%">
                                <tr>
                                    <td width="200px">
                                        <div class="padding5">
                                            Заказчик
                                            <asp:Label ID="lbSubject" CssClass="bold" runat="server" Text=""></asp:Label>
                                        </div>
                                        <div class="padding5">
                                            Заявка № <span id="the_ord" class="bold">
                                                <asp:Literal ID="lbOrder" runat="server"></asp:Literal></span> от
                            <asp:Literal ID="lbRegDate" runat="server"></asp:Literal>
                                        </div>
                                        <div class="padding5">
                                            Код в учетной системе <span class="bold">
                                                <asp:Literal ID="lbCode" runat="server"></asp:Literal></span>
                                        </div>
                                    </td>
                                    <td width="200px" class="right">
                                        <asp:LinkButton ID="lbtnNew" runat="server" CssClass="button" OnClick="btnNewOrder_Click"><img alt="[c]" src="../simg/16/add.png"/> новая заявка</asp:LinkButton></td>
                                </tr>
                            </table>




                            <div id="divorder">

                                <div class="bold padding5">
                                    Наименование заявки
                                    <asp:TextBox ID="txNameOrder" runat="server" MaxLength="150" Width="300px"></asp:TextBox>
                                </div>


                            </div>

                            <asp:DataGrid ID="dgOrder" runat="server" AutoGenerateColumns="False" CellPadding="4"
                                CssClass="grid" OnItemDataBound="dgOrder_ItemDataBound" ShowFooter="True" OnDeleteCommand="dgOrder_DeleteCommand"
                                Width="100%">
                                <Columns>
                                    <asp:BoundColumn DataField="goodid" Visible="False">
                                        <HeaderStyle Width="16px" />
                                    </asp:BoundColumn>
                                    <asp:ButtonColumn CommandName="delete" DataTextField="" Text="&lt;img title='удалить' src='../simg/16/delete.png'&gt;"></asp:ButtonColumn>
                                    <asp:BoundColumn DataField="goodcode" Visible="true" HeaderText="код"></asp:BoundColumn>
                                    <asp:BoundColumn DataField="name" HeaderText="Наименование"></asp:BoundColumn>
                                    <asp:BoundColumn DataField="pr" HeaderText="цена" DataFormatString="{0:N}">
                                        <HeaderStyle Width="60px" />
                                        <ItemStyle Font-Bold="False" Font-Italic="False" Font-Overline="False" Font-Strikeout="False"
                                            Font-Underline="False" HorizontalAlign="Right" />
                                    </asp:BoundColumn>
                                    <asp:BoundColumn DataField="qty" HeaderText="кол-во">
                                        <HeaderStyle Width="60px" />
                                    </asp:BoundColumn>
                                    <asp:BoundColumn DataField="ed" HeaderText="">
                                        <HeaderStyle Width="20px" />
                                    </asp:BoundColumn>
                                    <asp:BoundColumn DataField="sum" HeaderText="стоимость" DataFormatString="{0:N2}">
                                        <HeaderStyle Width="60px" />
                                        <ItemStyle Font-Bold="False" Font-Italic="False" Font-Overline="False" Font-Strikeout="False"
                                            Font-Underline="False" HorizontalAlign="Right" />
                                    </asp:BoundColumn>
                                    <asp:TemplateColumn HeaderText="Примечание"></asp:TemplateColumn>
                                    <asp:BoundColumn DataField="comment" HeaderText="Остатки" ItemStyle-CssClass="small"></asp:BoundColumn>
                                </Columns>
                                <FooterStyle CssClass="mainbackcolor bold white" />
                                <HeaderStyle CssClass="mainbackcolor bold white" />
                                <PagerStyle CssClass="mypager" HorizontalAlign="Center" />
                            </asp:DataGrid>

                            <asp:Literal ID="lbDiscount" runat="server"></asp:Literal>
                            <div class="bold">Примечание</div>
                            <asp:TextBox ID="txDescr" runat="server" TextMode="MultiLine" Width="500px"></asp:TextBox>
                        </div>
                        <br />
                        <asp:LinkButton ID="lbtnSave" runat="server" CssClass="button" OnClick="btnSaveOrder_Click"
                            Visible="true"><img alt="[v]" src="../simg/16/save.png"/> сохранить заявку</asp:LinkButton>

                        &nbsp;
                        <asp:LinkButton ID="lbtnDelete" runat="server" CssClass="button" OnClick="btnClear_Click"
                            OnClientClick="javascript:if(!confirm('Вы подтверждаете очистку изменений?')) return false;"><img alt="[x]" src="../simg/16/delete.png"/> отменить изменения</asp:LinkButton>



                        <hr />
                        <uc3:CommentList ID="CommentList1" runat="server" />


                    </asp:Panel>
    </div>
            </td>
        </tr>
    </table>

    <a name="myorders"></a>
    <asp:Repeater ID="rpOrders" runat="server">
        <HeaderTemplate>
            <h4>Ранее сформированные заявки</h4>
            <ul>
        </HeaderTemplate>
        <ItemTemplate>
            <li>
                <span class='micro' id='menu_o_<%#Eval("ID") %>' style="text-align: right">
                    <%#Eval("linkchange")%>
                </span>
                <a style="padding: 5px;" title='от ' href='javascript: return false;' class="bold" onclick="openflywin('../order/detailfly.aspx?id=<%#Eval("ID") %>', 850, 800, 'Заявка <%#Eval("Name") %> от <%#Eval("RegDate") %>')">заявка №<%#Eval("ID") %>от
                    <%#Eval("RegDate") %>
                    <span class="small">
                        <%#Eval("Name") %></span></a>
                <span class="small">сумма
                    <%#Eval("SummOrder") %>р.</span> -
                <span class="small ">
                    <%#Eval("State") %></span>
            </li>
        </ItemTemplate>
        <SeparatorTemplate>
        </SeparatorTemplate>
        <FooterTemplate>
            </ul>
        </FooterTemplate>
    </asp:Repeater>
    <a name="blockArh"></a>
    <asp:Repeater ID="rpArchive" runat="server">
        <HeaderTemplate>
            <h4 id="blockArh">Архив</h4>
        </HeaderTemplate>
    </asp:Repeater>
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
