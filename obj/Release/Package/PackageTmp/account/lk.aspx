<%@ Page Title="" Language="C#" MasterPageFile="~/common/mainpage.Master" AutoEventWireup="true" CodeBehind="lk.aspx.cs" Inherits="wstcp.account.lk" %>

<%@ Register Src="DgInfo.ascx" TagName="DgInfo" TagPrefix="uc1" %>

<%@ Register Src="../UC/TabControl.ascx" TagName="TabControl" TagPrefix="uc2" %>

<asp:Content ID="Content1" ContentPlaceHolderID="head" runat="server">
    <script type="text/javascript">
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

</asp:Content>
<asp:Content ID="Content2" ContentPlaceHolderID="place_Left" runat="server">
</asp:Content>
<asp:Content ID="Content3" ContentPlaceHolderID="ContentPlaceHolder1" runat="server">
    <uc2:TabControl ID="TabControl1" runat="server" />
    <asp:Panel ID="pnlProfile" runat="server">
        <div class="tabcontrol g-row">
            <div class="g-6">
                <table width="100%">
                    <tr>
                        <td colspan="2">
                            <h4>Компания
                            </h4>
                        </td>

                    </tr>
                    <tr>
                        <td width="140px" class="right">Название</td>
                        <td>
                            <asp:Label ID="lbSubjName" runat="server" Text=""></asp:Label></td>
                    </tr>
                    <tr>
                        <td class="right">ИНН</td>
                        <td>
                            <asp:Label ID="lbSubjINN" runat="server" Text=""></asp:Label></td>
                    </tr>
                    <tr>
                        <td colspan="2">
                            <h4>Представитель</h4>
                        </td>

                    </tr>
                    <tr>
                        <td>Имя</td>
                        <td>
                            <asp:TextBox ID="txIamName" MaxLength="150" runat="server" ReadOnly="True"></asp:TextBox>
                        </td>
                    </tr>
                    <tr>
                        <td>Телефоны</td>
                        <td>
                            <asp:TextBox ID="txIamPhone" runat="server" MaxLength="100" ReadOnly="True"></asp:TextBox></td>
                    </tr>
                    <tr>
                        <td>Email</td>
                        <td>
                            <asp:TextBox ID="txIamEmail" runat="server" MaxLength="100" ReadOnly="True"></asp:TextBox>
                        </td>
                    </tr>
                    <tr>
                        <td>&nbsp;</td>
                        <td>
                            <asp:LinkButton ID="btnChange" runat="server" OnClick="btnChange_Click">изменить </asp:LinkButton>
                            <asp:Button ID="btnSave" runat="server" OnClick="btnSave_Click" Text="Сохранить" Visible="False" />
                            <div class="message">
                                <asp:Literal ID="lbMess" runat="server"></asp:Literal></div>
                        </td>
                    </tr>
                    <tr>
                        <td colspan="2">для смены пароля укажите текущий пароль и задайте новый
                            
                                
                        </td>

                    </tr>

                    <tr>
                        <td class="right">Текущий</td>
                        <td>


                            <asp:TextBox ID="txIamPswOld" runat="server" MaxLength="100"></asp:TextBox>
                        </td>
                    </tr>

                    <tr>
                        <td class="right">Новый </td>
                        <td>


                            <asp:TextBox ID="txIamPsw1" TextMode="Password" runat="server" MaxLength="100"></asp:TextBox>
                        </td>
                    </tr>

                    <tr>
                        <td class="right">Повтор нового</td>
                        <td>


                            <asp:TextBox ID="txIamPsw2" TextMode="Password" runat="server" MaxLength="100"></asp:TextBox>

                        </td>
                    </tr>

                    <tr>
                        <td>&nbsp;</td>
                        <td>

                            <asp:Button ID="btnChangePsw" runat="server" Text="сменить пароль" OnClick="btnChangePsw_Click" />
                        </td>
                    </tr>

                </table>

            </div>
            <div class="g-5 ">

                <div class="ipad">
                    <h4>Договорные условия</h4>
                    <uc1:DgInfo ID="DgInfo1" runat="server" />



                </div>
                <hr />


            </div>
        </div>
    </asp:Panel>
    <asp:Panel ID="pnlOrders" runat="server">
        <div class="tabcontrol padding5">
            
        <p>
        статус заявки
        <asp:DropDownList ID="dlState" runat="server" OnSelectedIndexChanged="dlState_SelectedIndexChanged" AutoPostBack="True"></asp:DropDownList><br />
        Найти Заявку по товару
        <asp:TextBox ID="txSearch" runat="server" MaxLength="100"></asp:TextBox>
        <asp:Button ID="btnSeach" runat="server" Text="Найти" OnClick="btnSeach_Click" Width="50px" />
            </p>
            <asp:Literal ID="lbMsg" runat="server"></asp:Literal>
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
    </div>
    </asp:Panel>
    <asp:Panel ID="pnlQueries" runat="server">
         <div class="tabcontrol padding5">
            
        <p>
        Найти Запрос по товару
        <asp:TextBox ID="txQSearch" runat="server" MaxLength="100"></asp:TextBox>
        <asp:Button ID="btnQSearch" runat="server" Text="Найти" OnClick="btnQSeach_Click" Width="50px" />
            </p>
            <asp:Literal ID="lbQMsg" runat="server"></asp:Literal>
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
    </div>

    </asp:Panel>
    

</asp:Content>
<asp:Content ID="Content4" ContentPlaceHolderID="place_Right" runat="server">
</asp:Content>
