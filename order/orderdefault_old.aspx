<%@ Page Title="" Language="C#" MasterPageFile="~/common/mainpage.Master" AutoEventWireup="true"
    CodeBehind="orderdefault_old.aspx.cs" Inherits="wstcp.orderdefault" %>


<%@ Register Src="~/UC/TabControl.ascx" TagName="TabControl" TagPrefix="uc1" %>
<%@ Register Src="../UC/CommandPanel4Profile.ascx" TagName="CommandPanel4Profile" TagPrefix="uc2" %>
<%@ Register Src="../UC/CommentList.ascx" TagName="CommentList" TagPrefix="uc3" %>
<%@ Register Src="../account/DgInfo.ascx" TagName="DgInfo" TagPrefix="uc4" %>
<%@ Register TagPrefix="uc2" TagName="ucDateInput" Src="~/UC/ucDateInput.ascx" %>



<asp:Content ID="Content1" ContentPlaceHolderID="head" runat="server">

    <link href="../css/tooltip.css" rel="stylesheet" type="text/css" />
    <link href="../order/ord.css" rel="stylesheet" type="text/css" />
    <script src="../order/ord.js" type="text/javascript"></script>
    <style type="text/css">
        .printSelected div {
            display: none;
        }

            .printSelected div.printSelection {
                display: block;
            }

                .printSelected div.printSelection div {
                    display: block;
                }

        table.plusminus {
            line-height: 16px;
        }

        .plusminus {
            cursor: pointer;
            padding: 0;
            margin: 0;
            border: 0;
        }
    </style>
    <script src="../good/glist.js"></script>
    <script type="text/javascript">

        $(function () {
            $(".inputqty").keypress(function (event) {
                if (event.which == '13') {
                    event.preventDefault();
                    $(this).blur();
                }
            });
        });


        function showStruct() {
            $("#structWindow").hide();
            return;
            if ($("#structWindow").css("display") == "none")
                $("#structWindow").show("slow");
            else {
                $("#structWindow").hide();
            }
        }

        function showan(goodCode, ownerId, priceType) {
            showComplect(goodCode, priceType, ownerId, "komplgoodies");
            showSoput(goodCode, priceType, ownerId, "sopgoodies");
            showAnalog(goodCode, priceType, ownerId, "angoodies");
        }


        function plusq(goodId) {
            var inpqty = $("#qch_" + goodId);
            inpqty.val("" + (myParseFloat(inpqty.val()) + 1));
            changeQty(goodId);
        }
        function minusq(goodId) {
            var inpqty = $("#qch_" + goodId);
            var nq = myParseFloat(inpqty.val()) - 1;
            if (nq < 0) {
                inpqty.val("");

            } else {
                inpqty.val("" + nq);

            }
            changeQty(goodId);
        }
        function plusqo(goodId) {
            var inpqty = $("#q_" + goodId);
            inpqty.val("" + (myParseFloat(inpqty.val()) + 1));
            recount(goodId);
        }
        function minusqo(goodId) {
            var inpqty = $("#q_" + goodId);
            var nq = myParseFloat(inpqty.val()) - 1;
            if (nq < 0) {
                inpqty.val("");

            } else {
                inpqty.val("" + nq);

            }
            recount(goodId);
        }
    </script>

</asp:Content>

<asp:Content ID="Content3" ContentPlaceHolderID="ContentPlaceHolder1" runat="server">

    <div class="floatLeft">
        <h2 onclick="showStruct()">Самостоятельный подбор заявки
            <asp:Literal ID="lbForSubj" runat="server"></asp:Literal></h2>
    </div>
    <div class="floatRight">
        <asp:LinkButton ID="lbtnGotoCart" Visible="false" runat="server" OnClick="lbtnGotoCart_Click">Перейти в заявку</asp:LinkButton>
        <asp:LinkButton ID="lbtnNew" Visible="false" runat="server" CssClass="button drg" OnClick="btnNewOrder_Click"><img alt="[c]" src="../simg/16/add.png"/> новая заявка</asp:LinkButton>
    </div>
    <div class="floatRight">
        <div id="pnlSourceSelect" runat="server" visible="true" style="padding-bottom: 10px">
            <strong>Источник списка номенклатуры </strong>: 
        <asp:LinkButton ID="lnkJust" CommandArgument="just" runat="server" ClientIDMode="Static" CssClass="" OnClick="lnkSelectREG_Click">Весь каталог</asp:LinkButton>
            <asp:LinkButton ID="lnkMyFavor" CssClass="" ClientIDMode="Static" OnClick="lnkSelectREG_Click" CommandArgument="my" runat="server">Из истории моих заказов</asp:LinkButton>
            <asp:LinkButton CommandArgument="spec" CssClass="" ClientIDMode="Static" ID="lnkSpecial" OnClick="lnkSelectREG_Click" runat="server">Распродажа!!!</asp:LinkButton>
            &nbsp;<asp:CheckBox ID="chIncash" ClientIDMode="Static" runat="server" Text="из наличия" AutoPostBack="True" CssClass="bold" OnCheckedChanged="chIncash_CheckedChanged" />
        </div>
    </div>
    <div class="clearBoth"></div>



    <div class="message">
        <asp:Literal ID="lbReadonly" Visible="false" runat="server" Text="Выбранную заявку нельзя редактировать на сайте. Обратитесь к своему менеджеру."></asp:Literal>
    </div>
    <div id="msgcart" name="msgcart"></div>

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

                        <div class="grayfon floatLeft">
                            Режим выбора товарных категорий
                            <asp:RadioButtonList ID="rbRegselect" runat="server" OnSelectedIndexChanged="rbRegselect_SelectedIndexChanged" AutoPostBack="true" RepeatDirection="Horizontal" RepeatLayout="Flow">
                                <asp:ListItem Selected="True" Value="single">Одинарный</asp:ListItem>
                                <asp:ListItem Value="multy">Множественный</asp:ListItem>
                            </asp:RadioButtonList>
                            &nbsp;&nbsp;&nbsp;Вид списка категорий
                                    <asp:RadioButtonList ID="rbVidcat" runat="server" OnSelectedIndexChanged="rbVidcat_SelectedIndexChanged" AutoPostBack="true" RepeatDirection="Horizontal" RepeatLayout="Flow">
                                        <asp:ListItem Selected="True" Value="min">сворачиваемый</asp:ListItem>
                                        <asp:ListItem Value="max">раскрытый</asp:ListItem>
                                    </asp:RadioButtonList>
                        </div>
                        <div class="floatRight">
                            <asp:LinkButton ID="lbtnClearsearchcat" runat="server" OnClick="lbtnClearsearchcat_Click">очистить выбор категории</asp:LinkButton>
                        </div>
                        <div class="clearBoth">
                        </div>

                        <div class="message">
                            <asp:Label ID="lbMessSource" runat="server" Text="Label"></asp:Label>
                        </div>
                        <asp:PlaceHolder ID="PlaceHolder1" runat="server" Visible="false">
                            <div class="bold">
                                найти номенклатуру 
                            <asp:TextBox ID="txSimSearch" MaxLength="50" runat="server" Width="400px"></asp:TextBox><asp:Button ID="btnSimSearch" runat="server" Text="найти" OnClick="btnSimSearch_Click" />
                            </div>
                            <div class="right">
                                найти товарную категорию
                            <input class="italic" maxlength="20" type="text" id='searchtks' onkeypress="findtks()" onchange="javas                    cript:return false;"
                                value='' />
                            </div>
                        </asp:PlaceHolder>
                        <div style="overflow: auto; height: 500px">
                            <asp:Literal ID="struct_place" runat="server"></asp:Literal>
                        </div>
                    </asp:Panel>
                    <asp:Panel ID="pnlList" runat="server">
                        <div class="message small">
                            <asp:Literal ID="lbListMessage" runat="server"></asp:Literal>
                        </div>

                        <%--<i><span class="red bold border"> * </span> - <span class="small">звездочка перед ценой означает заказную позицию и цена ориентировочная. Её нужно уточнить у своего менеджера. Двойной клик на цене - послать запрос по этой позиции.</span></i>--%>
                        <div class="floatLeft big bold">
                            <asp:LinkButton ID="lbtnChangeTN" CssClass="italic micro bold" runat="server" OnClick="lbtnChangeTN_Click"><img src="../simg/16/arrow-left.png" alt="<=" title="К выбору Товарное направление/категории"/></asp:LinkButton>
                            <asp:Literal Visible="false" ID="lbSelTN" runat="server"></asp:Literal>

                        </div>
                        <div class="floatLeft-10" id="catTitle">&nbsp;</div>
                        <div class="clearBoth">
                        </div>
                        <table width="100%" class=" orangefon">
                            <tr>
                                <td>
                                    <div class=" orangefon padding5 ">
                                        <div class="bold padding2 white">
                                            <div id="pnlSourceSelect2" runat="server" visible="false" style="border-bottom: 1px solid #ffffff; margin-bottom: 4px; padding-bottom: 4px;">
                                                Источник списка номенклатуры:
                                                <asp:DropDownList ID="dlSource" runat="server" AutoPostBack="True" OnSelectedIndexChanged="dlSource_SelectedIndexChanged">
                                                    <asp:ListItem Value="all">Весь каталог</asp:ListItem>
                                                    <asp:ListItem Value="my">Из истории своих заказов</asp:ListItem>
                                                    <asp:ListItem Value="spec">!!! Распродажа</asp:ListItem>
                                                </asp:DropDownList>

                                            </div>

                                            Бренд:
                            <asp:DropDownList ID="dlBrends" runat="server" Width="120px" AutoPostBack="true"
                                OnSelectedIndexChanged="dlBrends_SelectedIndexChanged" ClientIDMode="Static">
                            </asp:DropDownList>&nbsp;&nbsp;Категория:
                            <asp:DropDownList ID="dlTK" runat="server" Width="120px" AutoPostBack="true" OnSelectedIndexChanged="dlTK_SelectedIndexChanged" ClientIDMode="Static">
                            </asp:DropDownList>
                                            &nbsp;&nbsp;Номенклатура:
                            <asp:DropDownList ID="dlNames" runat="server" OnSelectedIndexChanged="txSearch_TextChanged"
                                Width="120px" ClientIDMode="Static" AutoPostBack="True">
                            </asp:DropDownList>
                                            &nbsp;
                            

                            <br />
                                        </div>
                                        <div class="white padding2 bold">
                                            поиск по коду, артикулу или части названия
                            <asp:TextBox ID="txSearch" runat="server" Width="250px" MaxLength="150">&nbsp;</asp:TextBox><asp:Button Visible="true" ClientIDMode="Static" ID="btnSearchGood" runat="server" CssClass="f-bu" Text="найти" OnClick="txSearch_TextChanged" />
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
                                        <asp:LinkButton ID="btnClearFilter" ClientIDMode="Static" CssClass="small white clear-filter " runat="server" OnClick="btnClearFilter_Click">сбросить фильтр</asp:LinkButton>
                                        <asp:Button ID="btnAcptFlt" CssClass="acptfltr" runat="server" OnClick="btnAcptFlt_Click" Text="применить фильтр" Visible="false" />
                                    </div>
                                    <div class="clearBoth"></div>
                                </td>
                            </tr>
                        </table>


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
                            OnItemDataBound="dgList_ItemDataBound" Width="100%" AllowSorting="True" OnSortCommand="dgList_SortCommand">
                            <Columns>
                                <asp:BoundColumn DataField="id" Visible="False"></asp:BoundColumn>
                                <asp:TemplateColumn HeaderText="выбор" ItemStyle-Width="70px">
                                    <ItemStyle Width="70px" />
                                </asp:TemplateColumn>
                                <asp:TemplateColumn HeaderText="" ItemStyle-Width="20px">
                                    <ItemStyle Width="20px" Font-Bold="False" Font-Italic="False" Font-Overline="False" Font-Strikeout="False" Font-Underline="False" />
                                </asp:TemplateColumn>
                                <asp:BoundColumn DataField="goodcode" HeaderText="код 1С">
                                    <ItemStyle Font-Bold="False" Font-Italic="False" Font-Overline="False" Font-Size="8pt" Font-Strikeout="False" Font-Underline="False" />
                                </asp:BoundColumn>
                                <asp:BoundColumn DataField="name" HeaderText="наименование" SortExpression="name"></asp:BoundColumn>
                                <asp:TemplateColumn HeaderText="наличие"></asp:TemplateColumn>
                                <asp:TemplateColumn HeaderText="цена, (руб.)" SortExpression="price">
                                    <ItemStyle Font-Bold="False" Font-Italic="False" Font-Overline="False" Font-Strikeout="False" Font-Underline="False" HorizontalAlign="Right" />
                                </asp:TemplateColumn>
                                <asp:TemplateColumn HeaderText="">
                                    <ItemStyle Font-Bold="False" Font-Italic="False" Font-Overline="False" Font-Strikeout="False" Font-Underline="False" Wrap="False" />
                                </asp:TemplateColumn>
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



                        <div id="pnlFormOrder" runat="server">

                            <div class="border center padding2">
                                <table width="100%">
                                    <tr>
                                        <td width="200px">
                                            <asp:Literal ID="link4prn" runat="server"></asp:Literal></td>
                                        <td><span class="bigsize">
                                            <asp:Literal ID="lbStateOrder" runat="server"></asp:Literal></span>

                                        </td>
                                        <td width="200px" class="right">
                                            <asp:LinkButton ID="lbtnNextStady" runat="server" OnClick="lbtnNextStady_Click"><img src="../simg/iconext.png" title="Согласовано, Далее"/></asp:LinkButton>
                                            <asp:LinkButton ID="lbtnCancel" runat="server" OnClick="lbtnDelete_Click" OnClientClick="javascript: return myConfirm('Вы подтвержд                    аете удаление заявки?')"><img src="../simg/icodel.png" title="Отменить заявку"/></asp:LinkButton></td>
                                    </tr>
                                </table>

                            </div>
                            <div class="border padding2">
                                <table width="100%">
                                    <tr>
                                        <td width="50%">
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
                                        <td width="50%">



                                            <uc4:DgInfo ID="DgInfo1" runat="server" />



                                        </td>
                                    </tr>
                                </table>




                                <div id="divorder">

                                    <div class="bold padding5">
                                        Наименование заявки
                                    <asp:TextBox ID="txNameOrder" ClientIDMode="Static" runat="server" MaxLength="150" Width="300px" OnPreRender="txNameOrder_PreRender"></asp:TextBox>
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
                                        <asp:BoundColumn DataField="goodcode" Visible="true" HeaderText="код">
                                            <ItemStyle Font-Bold="False" Font-Italic="False" Font-Overline="False" Font-Size="X-Small" Font-Strikeout="False" Font-Underline="False" />
                                        </asp:BoundColumn>
                                        <asp:BoundColumn DataField="name" HeaderText="Наименование"></asp:BoundColumn>
                                        <asp:BoundColumn DataField="price" HeaderText="цена" DataFormatString="{0:N}">
                                            <HeaderStyle Width="60px" />
                                            <ItemStyle Font-Bold="False" Font-Italic="False" Font-Overline="False" Font-Strikeout="False"
                                                Font-Underline="False" HorizontalAlign="Right" />
                                        </asp:BoundColumn>
                                        <asp:BoundColumn DataField="qty" HeaderText="кол-во">
                                            <HeaderStyle Width="60px" />
                                        </asp:BoundColumn>
                                        <asp:TemplateColumn HeaderText=""></asp:TemplateColumn>
                                        <asp:BoundColumn DataField="ed" HeaderText="">
                                            <HeaderStyle Width="20px" />
                                        </asp:BoundColumn>
                                        <asp:BoundColumn DataField="summ" HeaderText="стоимость" DataFormatString="{0:N2}">
                                            <HeaderStyle Width="60px" />
                                            <ItemStyle Font-Bold="False" Font-Italic="False" Font-Overline="False" Font-Strikeout="False"
                                                Font-Underline="False" HorizontalAlign="Right" />
                                        </asp:BoundColumn>
                                        <asp:TemplateColumn HeaderText="Примечание"></asp:TemplateColumn>
                                        <asp:BoundColumn DataField="comment" HeaderText="Остатки" ItemStyle-CssClass="small">
                                            <HeaderStyle Width="60px" />
                                            <ItemStyle CssClass="small" />
                                        </asp:BoundColumn>
                                    </Columns>
                                    <FooterStyle CssClass="mainbackcolor bold white" />
                                    <HeaderStyle CssClass="mainbackcolor bold white" />
                                    <PagerStyle CssClass="mypager" HorizontalAlign="Center" />
                                </asp:DataGrid>

                                <asp:Literal ID="lbDiscount" runat="server"></asp:Literal>
                                <div class="bold">Примечание</div>
                                <asp:TextBox ID="txDescr" runat="server" ClientIDMode="Static" TextMode="MultiLine" Width="500px" OnPreRender="txDescr_PreRender"></asp:TextBox>
                            </div>
                            <br />
                            <div class="center">

                                <asp:LinkButton ID="lbtnSave" runat="server" CssClass="button" OnClick="btnSaveOrder_Click"
                                    Visible="true"><img alt="[v]" src="../simg/16/save.png"/> сохранить заявку</asp:LinkButton>
                                &nbsp;  
                                <asp:LinkButton ID="btnClear" runat="server" CssClass="button" OnClick="btnClear_Click"
                                    OnClientClick="javascript:if(!confirm('Вы подтверждаете очистку изменений?')) return false;"><img alt="[x]" src="../simg/16/delete.png"/> отменить изменения</asp:LinkButton>

                                &nbsp;&nbsp;&nbsp;<asp:LinkButton ID="lbtnStoreQFile" runat="server" Visible="false" CssClass="button" OnClick="btnStoreQFile_Click"><img alt="[@]" src="../simg/16/detail.png"/> сохранить как набор для прайс-листа</asp:LinkButton>


                            </div>

                            <hr />
                            <uc3:CommentList ID="CommentList1" runat="server" />

                        </div>
                    </asp:Panel>
                    <asp:Panel ID="pnlResult" Visible="false" runat="server">
                        <p>Заявка успешно сохранена. №<asp:Literal ID="lbResultID" runat="server"></asp:Literal></p>
                        <p>В течение 5 минут она будет зарегистрирована в Сантехкомплекте.</p>
                        <p>
                            <asp:Literal ID="lbResultInvoice" runat="server"></asp:Literal>
                        </p>
                        <p>Отслеживать состояние заявки можно на <a href="../default.aspx">Главной странице</a></p>
                        <br />
                        <div>
                            <asp:Literal ID="lbMsgResult" runat="server"></asp:Literal>
                        </div>
                        <asp:Panel ID="pnlWishDate" runat="server" Visible="False" CssClass="border shadow left ">
                            <p class="bold">Ваш баланс позволяет получить товар в ближайшее время</p>
                            <span class="bigsize bold">Если готовы получить товар, то выберите дату и способ получения</span>
                            <div>
                                Желаемая дата получения
                                <br />
                                <uc2:ucDateInput ID="ucWishDate" runat="server" />
                            </div>
                            <div>
                                <asp:CheckBox ID="chNeedTrans" runat="server" Text="Нужна ли доставка" />
                            </div>
                            <div>
                                <asp:Button ID="btnGetOrder" runat="server" CssClass="f-bu" Text="отправить запрос" OnClick="btnGetOrder_Click" />
                                <asp:Button ID="btnCancelGetOrder" CssClass="f-bu" runat="server" Text="отставить" OnClick="btnCancelGetOrder_Click" />
                            </div>
                        </asp:Panel>


                        <div>
                            <a href="../default.aspx">Перейти на главную страницу</a> |
                            <asp:LinkButton ID="lbtnResNew" runat="server" OnClick="btnNewOrder_Click">Сделать новую заявку</asp:LinkButton>
                        </div>
                    </asp:Panel>
                    
                    <asp:Panel ID="pnlResultQ" Visible="false" runat="server">
                        <p>Набор для прайс-листа успешно сохранен. <asp:Literal ID="lbIDQ" runat="server"></asp:Literal></p>
                        
                        <p>Скачивать актуальный прайс можно в <a href="../account/lk.aspx">Личном кабинете</a></p>
                        <br />
                        
                        


                        <div>
                            <a href="../default.aspx">Перейти на главную страницу</a> |
                            <asp:LinkButton ID="lbtnNewO" runat="server" OnClick="btnNewOrder_Click">Сделать новую заявку</asp:LinkButton> | 
                            <asp:LinkButton ID="lbtnNewQ" runat="server" OnClick="btnNewQ_Click">Сделать новый набор для прайс-листа</asp:LinkButton> | 
                            <asp:LinkButton ID="btnCurrentQ" runat="server" OnClick="btnCurrentQ_Click">Продолжить работу с текущим набором</asp:LinkButton>
                        </div>
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


        }

    </script>
</asp:Content>
