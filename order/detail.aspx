﻿<%@ Page Title="" Language="C#" MasterPageFile="~/common/mainpage.Master" AutoEventWireup="true"
    CodeBehind="detail.aspx.cs" Inherits="wstcp.order.detail" %>

<%@ Register Src="../UC/CommentList.ascx" TagName="CommentList" TagPrefix="uc1" %>

<%@ Register Src="../UC/ucDateInput.ascx" TagName="ucDateInput" TagPrefix="uc2" %>

<%@ Register Src="../account/DgInfo.ascx" TagName="DgInfo" TagPrefix="uc3" %>


<asp:Content ID="Content1" ContentPlaceHolderID="head" runat="server">


    
   
    
</asp:Content>
<asp:Content runat="server" ID="center" ContentPlaceHolderID="ContentPlaceHolder1">
    <div class="message">
        <asp:Literal ID="lbMess" runat="server"></asp:Literal></div>
        <div class="g">
            <div class="g-row">
                <div class="g-6">
                   <h2>
                        <asp:Literal ID="lbTitle" runat="server"></asp:Literal></h2>
                    <div>
                        Заказчик
                <strong>
                    <asp:Literal ID="lbSubject" runat="server"></asp:Literal></strong>
                    </div>
                    <div>
                        Заявка
                <asp:Literal ID="lbAttr" runat="server"></asp:Literal>
                    </div>

                    <div>
                        Код в учетной системе <span class="bold">
                            <asp:Literal ID="lbCode" runat="server"></asp:Literal></span>
                    </div>
                    <div>
                        Ответственный менеджер: <strong>
                            <asp:Literal ID="lbTAs" runat="server"></asp:Literal></strong>
                    </div>
                    <asp:Literal ID="lbWishdate" runat="server"></asp:Literal>
                    <div>
                    </div>
                    <div>
                        Примечание:<br />
                        <asp:Literal ID="lbDescr" runat="server"></asp:Literal>
                    </div>
                    <uc3:DgInfo ID="DgInfo1" runat="server" /> 
                </div>
                <div class="g-6">
                    <div class="borderlight padding5">
                        <span><asp:Literal ID="linkInvoice" runat="server"></asp:Literal></span>
                        <img src="../simg/ord.png"  onclick="printBlock('divorder')" style="cursor:pointer;" title="Распечатать заявку"/><br/>
                    
                        <%--<a href="#" onclick="printBlock('divorder')" class="button"><img src="../simg/16/printer.png" /> печать заявки</a>--%>
                        <asp:LinkButton ID="lbtnRequestInvoice" CssClass="micro bold" runat="server" OnClick="lbtnRequestInvoice_Click">Прошу перевыставить<br/> счет на оплату</asp:LinkButton>
                    </div>
                    <br/>
                    <div class="borderlight">
                    Текущий статус:
                            <asp:LinkButton ID="btnRefresh" runat="server" ToolTip="проверить  статус (обновить информацию)" OnClick="btnRefresh_Click" CssClass="slowfunc">обновить</asp:LinkButton>
                            <br />
                            <strong class="bigsize">
                                <asp:Literal ID="lbState" runat="server"></asp:Literal></strong>
                
                    </div>
                
                    <div class="borderlight">
                        <p class="padding5 left">
                                <asp:Label ID="lbLock" CssClass="small bold" runat="server" Text=""></asp:Label>
                            </p>
                            <asp:MultiView ID="mvCmd" runat="server">
                                <asp:View ID="vBtns" runat="server">
                                    <div class="bold small">

                                    
                                        <asp:HyperLink ID="linkToCart" runat="server">изменить заявку</asp:HyperLink>
                                    
                                        <asp:LinkButton ID="lbtnNextStady" runat="server" OnClick="lbtnNextStady_Click" CssClass="button slowfunc"></asp:LinkButton>
                                        <asp:LinkButton ID="lbtnCancel" runat="server" OnClick="lbtnCancel_Click" ToolTip="Отменить заявку" CssClass="button slowfunc"></asp:LinkButton>
                                    </div>
                                    <div>
                                        <asp:LinkButton ID="lbtnChangeWishDate" Visible="false" CssClass="small bold" runat="server" OnClick="lbtnChangeWishDate_Click" ToolTip="Дату/Способ/Адрес">изменить параметры получения</asp:LinkButton>
                                    </div>
                                </asp:View>
                                <asp:View ID="vQuestDate" runat="server">
                                    <div class="border shadow left">
                                        <span class="bigsize bold">Запрос на отгрузку</span>
                                        <div>
                                            <label>Желаемая дата получения</label>
                                            <br />
                                            <uc2:ucDateInput ID="ucWishDate" OnSelectionChanged="ucWishDate_SelectionChanged" runat="server" />
                                            <p class="small italic">выбирайте пожалуйста рабочие дни</p>
                                            <p class="message">
                                                <asp:Label ID="lbMessDate" runat="server" Text=""></asp:Label>
                                            </p>
                                        </div>
                                        <div>
                                            <label>Вариант получения</label>
                                            <br />
                                            <asp:RadioButtonList ID="rbTeoVariant" runat="server" AutoPostBack="True" OnSelectedIndexChanged="chNeedTrans_CheckedChanged">
                                                <asp:ListItem Selected="True" Value="self">Самовывоз</asp:ListItem>
                                                <asp:ListItem Value="teo">Доставка</asp:ListItem>
                                            </asp:RadioButtonList>
                                            <%--<asp:CheckBox ID="chNeedTrans" runat="server" Text="Нужна ли доставка" AutoPostBack="true" OnCheckedChanged="chNeedTrans_CheckedChanged" />--%>
                                        </div>
                                        <div runat="server" id="teoaddress">
                                            <label for="txTEOAddress">Адрес доставки</label><br />
                                            <asp:TextBox ID="txTEOAddress" ClientIDMode="Static" MaxLength="499" TextMode="MultiLine" runat="server" Text=""></asp:TextBox>

                                        </div>
                                        <div>
                                            <asp:Button ID="btnSendCmd" runat="server" CssClass="f-bu-success slowfunc" Text="отправить запрос" OnClick="btnSendCmd_Click" /><asp:Button ID="btnCancelCmd" runat="server" Text="отставить" OnClick="btnCancelCmd_Click" />
                                        </div>
                                    </div>
                                </asp:View>
                                <asp:View ID="vMsgSuccess" runat="server">

                                    <div class="message-success">
                                        <asp:Literal ID="lbSuccess" runat="server"></asp:Literal>
                                    </div>
                                </asp:View>
                            </asp:MultiView>
                        
                    </div>
                    
                     <asp:Panel ID="pnlTrans" runat="server" CssClass="borderlight">
                        <h4>Параметры доставки</h4>
                        <p>
                            <asp:Label ID="lbTeoDate" runat="server" Text=""></asp:Label></p>
                        <p>
                            <asp:Label ID="lbTEOTrans" runat="server" Text=""></asp:Label></p>
                        <p>
                            <asp:Label ID="lbTeoAddress" runat="server" Text="Label"></asp:Label></p>
                    </asp:Panel>

                </div>
            </div>    
                <%--<table width="100%">
                    

                    <tr>
                        <td width="120px">
                            <div></div><br/>

                            
                        </td>
                        <td>
                            <div>
                                
                        <div>
                            
                           
                        </div>
                            </div>

                        </td>
                        <td class="right" width="250px">
                            
                            


                        </td>

                    </tr>
                </table>--%>
            

            <div id="divorder">
                <div class="floatLeft">
                    
                </div>
                <div class="floatRight-30">
                   
                </div>
                <div class="clearBoth"></div>
                <script type="text/javascript">
                    function hidereal() {
                        $(".fullreal").hide("fast");
                    }
                    function showall() {
                        $(".fullreal").show("fast");
                    }
                </script>
                <div class="small"><a href="javascript: return 0;" onclick="hidereal()" class="linkbutton">показать только неотгруженые позиции</a>&nbsp;&nbsp;&nbsp;&nbsp; <a href="javascript: return 0;"  class="linkbutton" onclick="showall()">показать все</a></div>
                <asp:DataGrid ID="DataGrid1" runat="server" CellPadding="4" ForeColor="#333333" OnItemDataBound="DataGrid1_ItemDataBound"
                    AutoGenerateColumns="False" ShowFooter="True">
                    <AlternatingItemStyle BackColor="White" ForeColor="#284775" />
                    <Columns>
                        <asp:BoundColumn DataField="GoodCode" HeaderText="Код"></asp:BoundColumn>
                        <asp:BoundColumn DataField="Name" HeaderText="Наименование"></asp:BoundColumn>
                        <asp:BoundColumn DataField="Price" HeaderText="Цена"></asp:BoundColumn>
                        <asp:BoundColumn DataField="Qty" HeaderText="Кол-во"></asp:BoundColumn>
                        <asp:BoundColumn DataField="Summ" HeaderText="Стоимость"></asp:BoundColumn>
                        <asp:BoundColumn DataField="Descr" HeaderText="комментарий заказчика"></asp:BoundColumn>
                        <asp:BoundColumn DataField="Comment" ItemStyle-CssClass="small" HeaderText="наличие">
                            <ItemStyle CssClass="small"></ItemStyle>
                        </asp:BoundColumn>
                        <asp:BoundColumn DataField="Booking" HeaderText="Резерв" ItemStyle-CssClass="small"></asp:BoundColumn>
                        <asp:BoundColumn DataField="Realized" HeaderText="Отгружено" ItemStyle-CssClass="small"></asp:BoundColumn>
                    </Columns>
                    <EditItemStyle BackColor="#999999" />
                    <FooterStyle BackColor="#5D7B9D" Font-Bold="True" ForeColor="White" />
                    <HeaderStyle BackColor="#5D7B9D" Font-Bold="True" CssClass="small" ForeColor="White" />
                    <ItemStyle BackColor="#F7F6F3" ForeColor="#333333" />
                    <PagerStyle BackColor="#284775" ForeColor="White" HorizontalAlign="Center" />
                    <SelectedItemStyle BackColor="#E2DED6" Font-Bold="True" ForeColor="#333333" />
                </asp:DataGrid>

            </div>
        </div>
        <asp:Literal ID="lbRecount" runat="server"></asp:Literal>
        <uc1:CommentList ID="CommentList1" runat="server" />
</asp:Content>
