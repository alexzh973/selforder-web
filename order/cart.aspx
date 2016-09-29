<%@ Page Title="" Language="C#" MasterPageFile="~/common/mainpage.Master" AutoEventWireup="true"
    CodeBehind="cart.aspx.cs" Inherits="wstcp._cart" %>


<%@ Register Src="~/UC/TabControl.ascx" TagName="TabControl" TagPrefix="uc1" %>
<%@ Register Src="../UC/CommandPanel4Profile.ascx" TagName="CommandPanel4Profile" TagPrefix="uc2" %>
<%@ Register Src="../UC/CommentList.ascx" TagName="CommentList" TagPrefix="uc3" %>
<%@ Register Src="../account/DgInfo.ascx" TagName="DgInfo" TagPrefix="uc4" %>
<%@ Register TagPrefix="uc2" TagName="ucDateInput" Src="~/UC/ucDateInput.ascx" %>



<asp:Content ID="Content1" ContentPlaceHolderID="head" runat="server">

    <link href="../css/tooltip.css" rel="stylesheet" type="text/css" />
    <link href="../order/ord01.css" rel="stylesheet" type="text/css" />
    
    <style type="text/css">
        

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
    

</asp:Content>

<asp:Content ID="scr1" ContentPlaceHolderID="placeScripts" runat="server">
    <script src="../order/ord01.js" type="text/javascript"></script>
    <script src="../good/good.js"></script>
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


        function plusqo(goodId, qty) {
            var inpqty = $("#q_" + goodId);
            checkNumeric("q_" + goodId, qty);
            inpqty.val(+inpqty.val() + +qty);
            recount(goodId,qty);
        }
        function minusqo(goodId, qty) {
            var inpqty = $("#q_" + goodId);
            checkNumeric("q_" + goodId, qty);
            var nq = inpqty.val() - qty;
            if (nq < 0) {
                inpqty.val("");

            } else {
                inpqty.val(nq);

            }
            recount(goodId,qty);
        }
   


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


<asp:Content ID="Content3" ContentPlaceHolderID="ContentPlaceHolder1" runat="server">

    <div class="floatLeft">
        <h2 onclick="showStruct()">Самостоятельный подбор заявки
            <asp:Literal ID="lbForSubj" runat="server"></asp:Literal></h2>
    </div>
    <div class="floatRight">
       
        <asp:LinkButton ID="lbtnNew" Visible="false" runat="server" CssClass="button slowfunc drg " OnClick="btnNewOrder_Click"><img alt="[c]" src="../simg/16/add.png"/> новая заявка</asp:LinkButton>
    </div>
    <div class="floatRight">
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
                <uc1:TabControl ID="TabControl1" runat="server" />
                <div style="padding: 5px;" class="tabcontrol">
                    <asp:Panel ID="pnlStruct" runat="server">
                    </asp:Panel>
                    <asp:Panel ID="pnlList" runat="server">
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
                                            <asp:LinkButton ID="lbtnNextStady" runat="server" CssClass="slowfunc" OnClick="lbtnNextStady_Click"><img src="../simg/iconext.png" title="Согласовано, Далее"/></asp:LinkButton>
                                            <asp:LinkButton ID="lbtnCancel" runat="server"  OnClick="lbtnDelete_Click" OnClientClick="javascript: return myConfirm('Вы подтвержд                    аете удаление заявки?')"><img src="../simg/icodel.png" title="Отменить заявку"/></asp:LinkButton></td>
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

                                <asp:LinkButton ID="lbtnSave" runat="server" CssClass="button slowfunc" OnClick="btnSaveOrder_Click"
                                    Visible="true"><img alt="[v]" src="../simg/16/save.png"/> сохранить заявку</asp:LinkButton>
                                &nbsp;  
                                <asp:LinkButton ID="btnClear" runat="server" CssClass="button slowfunc" OnClick="btnClear_Click"
                                    OnClientClick="javascript:if(!confirm('Вы подтверждаете очистку изменений?')) return false;"><img alt="[x]" src="../simg/16/delete.png"/> отменить изменения</asp:LinkButton>
                                <asp:LinkButton ID="btnNew" CssClass="button slowfunc"  runat="server" OnClientClick="javascript:if(!confirm('Вы подтверждаете очистку изменений?')) return false;" OnClick="btnNewOrder_Click"><img alt="[x]" src="../simg/16/add.png"/> Новая заявка</asp:LinkButton>
                                &nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;<asp:LinkButton ID="lbtnStoreQFile" OnPreRender="lbtnStoreQFile_PreRender" runat="server" CssClass="linkbutton slowfunc" OnClick="btnStoreQFile_Click"><img alt="[@]" src="../simg/16/detail.png"/> сохранить набор для прайс-листа</asp:LinkButton>


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
                        <p>Отслеживать состояние заявки можно на <a class="slowfunc" href="../default.aspx">Главной странице</a></p>
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
                                <asp:Button ID="btnGetOrder" runat="server" CssClass="f-bu slowfunc" Text="отправить запрос" OnClick="btnGetOrder_Click" />
                                <asp:Button ID="btnCancelGetOrder" CssClass="f-bu slowfunc" runat="server" Text="отставить" OnClick="btnCancelGetOrder_Click" />
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
                            <asp:LinkButton ID="btnCurrentQ" runat="server" OnClick="btnCurrentQ_Click">Продолжить работу с текущим прайсом</asp:LinkButton>
                        </div>
                    </asp:Panel>
                </div>
            </td>
        </tr>
    </table>


    
</asp:Content>
