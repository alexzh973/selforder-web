<%@ Page Title="" Language="C#" MasterPageFile="~/common/mainpage.Master" AutoEventWireup="true"
    CodeBehind="cart.aspx.cs" Inherits="wstcp.cart" %>


<%@ Register Src="~/UC/TabControl.ascx" TagName="TabControl" TagPrefix="uc1" %>
<%@ Register Src="../UC/CommandPanel4Profile.ascx" TagName="CommandPanel4Profile" TagPrefix="uc2" %>
<%@ Register Src="../UC/CommentList.ascx" TagName="CommentList" TagPrefix="uc3" %>
<%@ Register Src="../account/DgInfo.ascx" TagName="DgInfo" TagPrefix="uc4" %>
<%@ Register TagPrefix="uc2" TagName="ucDateInput" Src="~/UC/ucDateInput.ascx" %>



<%@ Register Src="../UC/BreadPath.ascx" TagName="BreadPath" TagPrefix="uc5" %>



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



            recountcart(goodId);
        }



        function minusqo(goodId) {
            var inpqty = $("#q_" + goodId);
            var nq = myParseFloat(inpqty.val()) - 1;
            if (nq < 0) {
                inpqty.val("");

            } else {
                inpqty.val("" + nq);

            }
            recountcart(goodId);
        }

        function recountcart(goodid) {
            var q = $("#q_" + goodid).val();
            var pr = $("#pr_" + goodid).text();
            
            itemInOrdercart('a', goodid, q, '');
            
            $.ajax({
                url: '../order/orderajax.ashx?act=countitemsumm&pr=' + pr + '&q=' + q + '&sid=' + sid,
                cache: false,
                success: function (data) {
                    $("#sm_" + goodid).text(data);

                },
                error: function (jqxhr, status, errorMsg) {
                    $('#qmsg').text(errorMsg);
                    setInterval(clearmsg, 3000);
                }
            });
        }
        function itemInOrdercart(act, goodId, qty, descr) {
            $.ajax({
                url: '../order/orderajax.ashx?act=' + act + '&gid=' + goodId + '&qty=' + qty + '&descr=' + descr + '&sid=' + sid,
                cache: false,
                success: function (data) {
                    
                 //   $("td[id*='_TabControl12']").addClass("brighttab");
                 //   $("span[id*='_TabControl12']").addClass("brighttab");
                    showOrderSummcart();
                    setInterval(clearmsg, 1000);

                },
                error: function (jqxhr, status, errorMsg) {
                    $('#qmsg').text(errorMsg);
                    setInterval(clearmsg, 3000);
                }
            });
        }

        function showOrderSummcart() {
            $.ajax({
                url: '../order/orderajax.ashx?act=getsumm&sid=' + sid,
                cache: false,
                success: function (data) {

                    $('#lbSumm').text(data);
                },
                error: function (jqxhr, status, errorMsg) {
                    $('#qmsg').text(errorMsg);
                    setInterval(clearmsg, 7000);
                }
            });
            $.ajax({
                url: '../order/orderajax.ashx?act=getdscnt&sid=' + sid,
                cache: false,
                success: function (data) {
                    
                    $('#lbDscnt').text(data);
                },
                error: function (jqxhr, status, errorMsg) {
                    $('#qmsg').text(errorMsg);
                    setInterval(clearmsg, 7000);
                }
            });
        }
    </script>

</asp:Content>

<asp:Content ID="Content3" ContentPlaceHolderID="ContentPlaceHolder1" runat="server">
    <asp:HiddenField ClientIDMode="Static" ID="refresh" runat="server" />
    <uc5:BreadPath ID="BreadPath1" runat="server" />
    <div class="floatLeft">
        <h2>Самостоятельный подбор заявки
            <asp:Literal ID="lbForSubj" runat="server"></asp:Literal></h2>
    </div>
    <div class="floatRight">
        <asp:LinkButton ID="lbtnNew" runat="server" CssClass="button drg" OnClick="btnNewOrder_Click"><img alt="[c]" src="../simg/16/add.png"/> новая заявка</asp:LinkButton>
    </div>

    <div class="clearBoth"></div>



    <div class="message">
        <asp:Literal ID="lbReadonly" Visible="false" runat="server" Text="Выбранную заявку нельзя редактировать на сайте. Обратитесь к своему менеджеру."></asp:Literal>
    </div>
    <div id="msgcart" name="msgcart"></div>

    <table width="100%">
        <tr>
            <td style="padding: 5px; vertical-align: top;">

                <div class="message">
                    <asp:Literal ID="lbMessage" runat="server"></asp:Literal>
                </div>
                <div class="message">
                            <asp:Literal ID="lbMess" runat="server"></asp:Literal>
                        </div>
                
                    
                    
                    
                        <asp:MultiView ID="mvForm" runat="server">
                            <asp:View ID="vProfile" runat="server">
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

                                                    <asp:LinkButton ID="lbtnCancel" runat="server" OnClick="lbtnDelete_Click" OnClientClick="javascript: return myConfirm('Вы подтверждаете удаление заявки?');"><img src="../simg/icodel.png" title="Отменить заявку"/></asp:LinkButton></td>
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
                                        <div class="center"><a href="selectgood.aspx" class="linkbutton small bold">+ добавить номенклатуру</a></div>
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
                                                <asp:BoundColumn DataField="comment" HeaderText="Остатки" ItemStyle-CssClass="small"></asp:BoundColumn>
                                            </Columns>
                                            <FooterStyle CssClass="mainbackcolor bold white" />
                                            <HeaderStyle CssClass="mainbackcolor bold white" />
                                            <PagerStyle CssClass="mypager" HorizontalAlign="Center" />
                                        </asp:DataGrid>
                                        <div id="divSumm">Сумма заявки <asp:Label ClientIDMode="Static" ID="lbSumm" runat="server" Text=""></asp:Label>руб.</div>
                                        <div id="divDiscount">скидка <asp:Label ClientIDMode="Static" ID="lbDscnt" runat="server" Text=""></asp:Label>%</div>
                                        <asp:Literal ID="lbDiscount" ClientIDMode="Static" runat="server"></asp:Literal>
                                        <div class="bold">Примечание</div>
                                        <asp:TextBox ID="txDescr" runat="server" ClientIDMode="Static" TextMode="MultiLine" Width="500px" OnPreRender="txDescr_PreRender"></asp:TextBox>
                                    </div>
                                    <br />
                                    <div class="center">
                                        <%--<div class="floatLeft">--%>
                                        <asp:LinkButton ID="lbtnSave" runat="server" CssClass="button" OnClick="btnSaveOrder_Click"
                                            Visible="true"><img alt="[v]" src="../simg/16/save.png"/> сохранить заявку</asp:LinkButton>
                                        &nbsp;  
                                <asp:LinkButton ID="btnClear" runat="server" CssClass="button" OnClick="btnClear_Click"
                                    OnClientClick="javascript:if(!confirm('Вы подтверждаете очистку изменений?')) return false;"><img alt="[x]" src="../simg/16/delete.png"/> отменить изменения</asp:LinkButton>
                                        <%--</div>--%>

                                        <%--<div class="clearBoth"></div>--%>
                                    </div>

                                    <hr />
                                    <uc3:CommentList ID="CommentList1" runat="server" />

                                </div>
                            </asp:View>
                            <asp:View ID="vResult" runat="server">
                                <div style="padding: 30px;">
                                    <asp:Literal ID="lbResult" runat="server"></asp:Literal>

                                </div>
                            </asp:View>
                            <asp:View ID="vDelete" runat="server">
                                <div style="padding: 30px;">
                                    <p>Вы подтверждаете удаление этой заявки?</p>
                                    <p>Заявка не удаляется полностью из системы, она будет доступна Вам в <a href="../account/lk.aspx">Личном кабинете</a></p>
                                    <p>
                                        <asp:Button ID="btnDel2" runat="server" Text="Удалить" /><asp:Button ID="btnDelCancel" runat="server" Text="вернуться" />
                                    </p>

                                </div>
                            </asp:View>
                        </asp:MultiView>
                        





                    
                    <asp:Panel ID="pnlResult" Visible="false" runat="server">
                        <p>Заявка успешно сохранена.</p>
                        <p>Заявка будет автоматически передана в Сантехкомплект для регистрации (передача займет ~ 5 минут).</p>
                        <p>После этого к заявке будет прикреплен счет на оплату</p>
                        <p>Отслеживать состояние заявки можно на <a href="../default.aspx">Главной странице</a></p>
                        <br />
                        <div>
                            <asp:Literal ID="lbMsgResult" runat="server"></asp:Literal>
                        </div>



                        <div>
                            <a href="../default.aspx">Перейти на главную страницу</a> |
                            <asp:LinkButton ID="lbtnResNew" runat="server" OnClick="btnNewOrder_Click">Сделать новую заявку</asp:LinkButton>
                        </div>
                    </asp:Panel>
                
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





        

    </script>
</asp:Content>
