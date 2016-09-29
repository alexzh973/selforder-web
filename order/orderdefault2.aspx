<%@ Page Title="" Language="C#" MasterPageFile="~/common/workpage.Master" AutoEventWireup="true"
    CodeBehind="orderdefault2.aspx.cs" Inherits="wstcp.orderdefault2" %>

<%@ Register Src="~/UC/TabControl.ascx" TagName="TabControl" TagPrefix="uc1" %>
<%@ Register Src="../UC/CommandPanel4Profile.ascx" TagName="CommandPanel4Profile" TagPrefix="uc2" %>
<%@ Register Src="../UC/CommentList.ascx" TagName="CommentList" TagPrefix="uc3" %>
<asp:Content ID="Content1" ContentPlaceHolderID="head" runat="server">
    <link href="../tooltip.css" rel="stylesheet" type="text/css" />
    <link href="ord.css" rel="stylesheet"  type="text/css"/>
    <script src="o.js" type="text/javascript"></script>
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
    </style>

   <script type="text/javascript">
       
       function showStruct() {
           $("#structWindow").hide();
           return;
           if ($("#structWindow").css("display")=="none")
               $("#structWindow").show("slow");
           else {
               $("#structWindow").hide();
           }
    }
       </script> 

           </asp:Content>

       <asp:Content ID="Content3" ContentPlaceHolderID="ContentPlaceHolder1" runat="server">
   
<%--    <asp:Panel ID="pnlForSubj" runat="server">
    </asp:Panel>--%>

<%--    <div id="structWindow" style="position: absolute; z-index: 973; left:20px; top:200px; width:600px; height:800px; border:3px solid #777777; overflow: auto;" runat="server">
        <h2>Каталог номенклатуры</h2>
        <asp:Literal ID="strWin" runat="server"></asp:Literal>
    </div>--%>
           <div class="floatLeft"><h2 onclick="showStruct()">Самостоятельный подбор заявки</h2></div>
           <div class="floatRight"><div id="pnlSourceSelect" runat="server" visible="true" style="padding-bottom: 10px">
                                       <strong>Источник списка номенклатуры </strong><br/>
        <asp:LinkButton ID="lnkJust" CommandArgument="just" runat="server" ClientIDMode="Static" CssClass="" OnClick="lnkSelectREG_Click">Весь каталог</asp:LinkButton>
        <asp:LinkButton ID="lnkMyFavor" CssClass="" ClientIDMode="Static" OnClick="lnkSelectREG_Click" CommandArgument="my" runat="server">Из истории моих заказов</asp:LinkButton>
        <asp:LinkButton CommandArgument="spec" CssClass="" ClientIDMode="Static"  ID="lnkSpecial" OnClick="lnkSelectREG_Click" runat="server">Распродажа!!!</asp:LinkButton>
        &nbsp;<asp:CheckBox ID="chIncash" ClientIDMode="Static" runat="server" Text="из наличия" AutoPostBack="True" CssClass="bold" OnCheckedChanged="chIncash_CheckedChanged" />
    </div></div>
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
                        <div class="bold">
                            найти номенклатуру 
                            <asp:TextBox ID="txSimSearch" MaxLength="50" runat="server" Width="400px"></asp:TextBox><asp:Button ID="btnSimSearch" runat="server" Text="найти" OnClick="btnSimSearch_Click" />
                        </div>
                        <div class="italic">
                            найти товарную категорию
                            <input class="italic" maxlength="20" type="text" id='searchtks' onkeyup="f   indtks()" onchange="javascript:return false;"
                                value=''>
                        </div>


                        <asp:Literal ID="struct_place" runat="server"></asp:Literal>
                    </asp:Panel>
                    <asp:Panel ID="pnlList" runat="server">
                        <div class="message small">
                            <asp:Literal ID="lbListMessage" runat="server"></asp:Literal>
                        </div>

                        <%--<i><span class="red bold border"> * </span> - <span class="small">звездочка перед ценой означает заказную позицию и цена ориентировочная. Её нужно уточнить у своего менеджера. Двойной клик на цене - послать запрос по этой позиции.</span></i>--%>
                        <h3 class="floatLeft">
                            <asp:Literal Visible="false" ID="lbSelTN" runat="server"></asp:Literal>
                            <asp:LinkButton ID="lbtnChangeTN" CssClass="italic micro bold" runat="server" OnClick="lbtnChangeTN_Click"><img src="../simg/16/reselect.png" alt="@" title="изменить товарное направление"/></asp:LinkButton>
                        </h3>
                        <div class="floatLeft-10" id="catTitle">&nbsp;</div>
                        <div class="clearBoth">
                        </div>
                        <table width="100%" class=" orangefon">
                            <tr>
                                <td>
                                    <div class=" orangefon padding5 ">
                                        <asp:Panel ID="pnlDinflt" runat="server"></asp:Panel>
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
                                        <div class="white padding2">
                                            поиск по коду, артикулу или части названия
                            <asp:TextBox ID="txSearch" runat="server" Width="250px" MaxLength="150"></asp:TextBox><asp:Button Visible="true" ClientIDMode="Static" ID="btnSearchGood" runat="server" Text="найти" OnClick="txSearch_TextChanged" />
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
                            OnItemDataBound="dgList_ItemDataBound" Width="100%">
                            <Columns>
                                <asp:BoundColumn DataField="id" Visible="False"></asp:BoundColumn>
                                <asp:TemplateColumn HeaderText="выбор" ItemStyle-Width="70px">
                                    <ItemStyle Width="70px" />
                                </asp:TemplateColumn>
                                <asp:BoundColumn DataField="goodcode" HeaderText="код 1С"></asp:BoundColumn>
                                <asp:BoundColumn DataField="name" HeaderText="наименование"></asp:BoundColumn>
                                <asp:TemplateColumn HeaderText="наличие"></asp:TemplateColumn>
                                <asp:TemplateColumn HeaderText="цена, (руб.)">
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





                        <div class="border center padding2">
                            <table width="100%">
                                <tr>
                                    <td width="200px">
                                        <asp:Literal ID="link4prn" runat="server"></asp:Literal></td>
                                    <td>Статус заявки <span class="bigsize">
                                        <asp:Literal ID="lbStateOrder" runat="server"></asp:Literal></span>
                                        <div>Баланс по взаимосрасчетам:
                                            <asp:Literal ID="lbBalance" runat="server"></asp:Literal></div>

                                    </td>
                                    <td width="200px" class="right">
                                        <asp:LinkButton ID="lbtnNextStady" runat="server" OnClick="lbtnNextStady_Click"><img src="../simg/iconext.png" title="Согласовано, Далее"/></asp:LinkButton>
                                        <asp:LinkButton ID="lbtnCancel" runat="server" OnClick="lbtnDelete_Click" OnClientClick="javascript: return myConfirm('Вы подтверждаете удаление заявки?')"><img src="../simg/icodel.png" title="Отменить заявку"/></asp:LinkButton></td>
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
                                        
                                        

                                    </td>
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
                        <div>
                            <div class="floatLeft"><asp:LinkButton ID="lbtnSave" runat="server" CssClass="button" OnClick="btnSaveOrder_Click"
                            Visible="true"><img alt="[v]" src="../simg/16/save.png"/> сохранить заявку</asp:LinkButton>
                              &nbsp;  
                                <asp:LinkButton ID="btnClear" runat="server" CssClass="button" OnClick="btnClear_Click"
                            OnClientClick="javascript:if(!confirm('Вы подтверждаете очистку изменений?')) return false;"><img alt="[x]" src="../simg/16/delete.png"/> отменить изменения</asp:LinkButton></div>
                            <div class="floatRight"><asp:LinkButton ID="lbtnNew" runat="server" CssClass="button drg" OnClick="btnNewOrder_Click"><img alt="[c]" src="../simg/16/add.png"/> новая заявка</asp:LinkButton></div>
                            <div class="clearBoth"></div>
                            

                        </div>
                        

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


        }
    
</script>
</asp:Content>
