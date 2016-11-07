<%@ Page Title="" Language="C#" MasterPageFile="~/admin/admin.Master" ValidateRequest="false"
    AutoEventWireup="true" CodeBehind="admin_partner.aspx.cs" Inherits="wstcp.admin_partner" %>

<%@ Register Src="../UC/CommandPanel4Profile.ascx" TagName="CommandPanel4Profile"
    TagPrefix="uc1" %>
<asp:Content ID="Content1" ContentPlaceHolderID="head" runat="Server">
   
</asp:Content>

<asp:Content ID="Content2" ContentPlaceHolderID="ContentPlaceHolder1" runat="Server">
<div>
        <a href='admin_partner.aspx?act=new&id=0' class="button bold">Новый контрагент</a><br />
        <a href="#" onclick="printBlock('divtasklist')" class="button">печать</a>
    </div>    
    <h2>
        Список контрагентов</h2>

    <asp:LinkButton ID="btnReport" runat="server" OnClick="btnReport_Click">отчет по контрагентам/заявкам</asp:LinkButton> | <asp:LinkButton ID="btnList" runat="server">Список контрагентов</asp:LinkButton>
    <asp:MultiView ID="MultiView1" runat="server">
        <asp:View ID="vReport" runat="server">


            <asp:DataGrid ID="dgReport" runat="server">
            </asp:DataGrid>


        </asp:View>
        <asp:View ID="vList" runat="server">
            <asp:DropDownList ID="dlSortlist" runat="server" AutoPostBack="True" OnSelectedIndexChanged="dlSortlist_SelectedIndexChanged">
                <asp:ListItem Value="name">По наименованию</asp:ListItem>
                <asp:ListItem Value="qty">По кол-ву заказов</asp:ListItem>
                <asp:ListItem Value="regdate">По недавности заказа</asp:ListItem>
            </asp:DropDownList>

            <asp:Literal ID="litOverview" runat="server"></asp:Literal>
            <div class='small grayfon'>
                <div>
                    по ответственному менеджеру <asp:DropDownList ID="dlTA" runat="server"></asp:DropDownList> 
                </div>
                <div>
                    <asp:TextBox ID="txSearch" runat="server" Visible="true"></asp:TextBox>
                    <asp:Button ID="btnSearch" runat="server" Visible="true" Text="найти" OnClick="btnSearch_Click" /></div>
                <div>
                    <asp:Label ID="lbFilter" runat="server" CssClass="small" Visible="false" Text=""></asp:Label></div>
            </div>
            <div id="divtasklist">
                <asp:DataGrid ID="dgList" runat="server" AutoGenerateColumns="False" OnItemCommand="dgList_ItemCommand"
                    OnPageIndexChanged="dgList_PageIndexChanged" OnItemDataBound="dgList_ItemDataBound"
                    Width="98%" ShowFooter="True" GridLines="None" CellPadding="4" 
                    ForeColor="#333333" AllowPaging="True" PageSize="7" >
                    <AlternatingItemStyle BackColor="White" ForeColor="#284775" />
                    <Columns>
                        <asp:BoundColumn DataField="ID" Visible="False"></asp:BoundColumn>
                        <asp:BoundColumn DataField="Name" HeaderText="Название">
                            <HeaderStyle Width="80%" />
                        </asp:BoundColumn>
                        
                        <asp:TemplateColumn HeaderText="действия">
                            <HeaderStyle Width="20px" />
                        </asp:TemplateColumn>
                        <asp:ButtonColumn CommandName="select" Text="заказы"></asp:ButtonColumn>
                    </Columns>
                    <EditItemStyle BackColor="#999999" />
                    <FooterStyle BackColor="#5D7B9D" Font-Bold="True" ForeColor="White" />
                    <HeaderStyle BackColor="#5D7B9D" Font-Bold="True" ForeColor="White" Font-Italic="False"
                        Font-Overline="False" Font-Size="Smaller" Font-Strikeout="False" Font-Underline="False" />
                    <ItemStyle BackColor="#F7F6F3" ForeColor="#333333" />
                    <PagerStyle BackColor="#284775" ForeColor="White" HorizontalAlign="Center" CssClass="mypager" Mode="NumericPages" />
                    <SelectedItemStyle BackColor="#E2DED6" Font-Bold="True" ForeColor="#333333" />
                </asp:DataGrid>

                <hr />
                <asp:DataGrid ID="dgOrders" runat="server" AutoGenerateColumns="False" 
                    onitemdatabound="dgOrders_ItemDataBound">
                    <Columns>
                        <asp:BoundColumn DataField="id" HeaderText="Id"></asp:BoundColumn>
                        <asp:BoundColumn DataField="regdate" HeaderText="Создан"></asp:BoundColumn>
                        <asp:BoundColumn DataField="name" HeaderText="Наименование"></asp:BoundColumn>
                        <asp:BoundColumn DataField="summorder" HeaderText="Сумма"></asp:BoundColumn>
                        <asp:BoundColumn DataField="State" HeaderText="Статус"></asp:BoundColumn>
                        <asp:BoundColumn DataField="lcd" HeaderText="коррект."></asp:BoundColumn>
                    </Columns>
                    <HeaderStyle CssClass="bold small" />
                </asp:DataGrid>
            </div>
        </asp:View>
        <asp:View ID="vDetail" runat="server">
        </asp:View>
        <asp:View ID="vProfile" runat="server">
            <asp:Label ID="lbMessage" runat="server" Text=""></asp:Label>
            <h2>
                <asp:Literal ID="lbTitle" runat="server"></asp:Literal>
            </h2>
            <table class="profile shadow" cellpadding="3" width="500">
                <tr>
                    <td class="profile-button" colspan="2">
                        <uc1:CommandPanel4Profile ID="CommandPanel4Profile1" runat="server" />
                    </td>
                </tr>
                <tr>
                    <td class="profile-field-title">
                        
                    </td>
                    <td class="profile-field">
                        ID <asp:TextBox ID="txID" runat="server" Width="50px" ReadOnly="True"></asp:TextBox>&nbsp;&nbsp;ИНН <asp:TextBox ID="txINN" runat="server" MaxLength="50" Width="100"></asp:TextBox>&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;
Код 1С <asp:TextBox ID="txCode" Width="100" MaxLength="11" runat="server"></asp:TextBox>&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;
                    </td>
                </tr>
                <tr>
                    <td class="profile-field-title">
                        Название
                    </td>
                    <td class="profile-field">
                        <asp:TextBox ID="txName" runat="server" MaxLength="100" Width="100%"></asp:TextBox>
                    </td>
                </tr>
                <tr>
                    <td class="profile-field-title">
                        Владелец</td>
                    <td class="profile-field">
                        <asp:DropDownList ID="dlOwner" runat="server">
                        </asp:DropDownList>
                    </td>
                </tr>
                <tr>
                    <td class="profile-field-title">Это группа </td>
                    <td class="profile-field">
                        <asp:CheckBox ID="chIsFolder" runat="server" Text="" />
                    </td>
                </tr>
                <tr>
                    <td class="profile-field-title">
                        Входит в группу
                    </td>
                    <td class="profile-field">
                        <asp:DropDownList ID="dlParent" runat="server" Width="50%">
                        </asp:DropDownList>
                    </td>
                </tr>
                <tr>
                    <td class="profile-field-title">
                        &nbsp;</td>
                    <td class="profile-field">
                        <asp:CheckBox ID="chUseSmsAuthorization" runat="server" Text="Режим авторизации через код по SMS по умолчанию для новых клиентов." />
                    </td>
                </tr>
                
                
                
                <tr>
                    <td class="profile-field-title"></td>
                    <td class="profile-field">Код Договора <asp:TextBox ID="txCodeAgr" Width="100" MaxLength="11" runat="server"></asp:TextBox>&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;
                       Тип цен по договору <asp:DropDownList ID="dlPriceType" runat="server">
                            <asp:ListItem Value="pr_b">Базовый уровень</asp:ListItem>
                            <asp:ListItem Value="pr_opt">Оптовая</asp:ListItem>
                            <asp:ListItem Value="pr_kropt">Крупный опт</asp:ListItem>
                            <asp:ListItem Value="pr_spec">Специальная</asp:ListItem>
                            <asp:ListItem Value="pr_vip">VIP</asp:ListItem>
                            <asp:ListItem Value="pr_ngc">НГЦ</asp:ListItem>
                        </asp:DropDownList>
                    </td>
                </tr>
                <tr>
                    <td class="profile-field-title">
                        email'ы ответственного менеджера (ов)
                    </td>
                    <td class="profile-field">
                        <asp:TextBox ID="txEmailTAs" Width="100%" MaxLength="240" runat="server"></asp:TextBox><br />
                        <span class="italic small">можно написать несколько через запятую</span>
                    </td>
                </tr>
                <tr>
                    <td class="grayfon bold" colspan="2">
                        Представители контрагента
                        <asp:DataGrid ID="dgPersons" runat="server" AutoGenerateColumns="False" OnSelectedIndexChanged="dgPersons_SelectedIndexChanged" ShowHeader="False">
                            <Columns>
                                <asp:BoundColumn DataField="ID"></asp:BoundColumn>
                                <asp:BoundColumn DataField="Name"></asp:BoundColumn>
                                <asp:BoundColumn DataField="Email"></asp:BoundColumn>
                                <asp:BoundColumn DataField="phones"></asp:BoundColumn>
                                <asp:ButtonColumn CommandName="Select" Text="[...]"></asp:ButtonColumn>
                            </Columns>
                        </asp:DataGrid><asp:LinkButton ID="btnAddPerson" runat="server" OnClick="btnAddPerson_Click">Добавить представителя</asp:LinkButton>
                    </td>
                </tr>
                <tr>
                    <td class="profile-field-title">
                        &nbsp;
                    </td>
                    <td class="profile-field">
                        <asp:CheckBox ID="chLoginEnable" runat="server" Text="Доступ разрешен" TextAlign="Right" />
                    </td>
                </tr>
                <tr>
                    <td class="profile-field-title">
                        ID
                    </td>
                    <td class="profile-field">
                        <asp:TextBox ID="txIDPers" runat="server" ReadOnly="True" Width="100px"></asp:TextBox>
                    </td>
                </tr>
                <tr>
                    <td class="profile-field-title">
                        Имя
                    </td>
                    <td class="profile-field">
                        <asp:TextBox ID="txNamePers" runat="server" Width="100%"></asp:TextBox>
                    </td>
                </tr>
                <tr>
                    <td class="profile-field-title">
                        e-mail
                    </td>
                    <td class="profile-field">
                        <asp:TextBox ID="txEmailPers" runat="server" Width="100%"></asp:TextBox>
                    </td>
                </tr>
                <tr>
                    <td class="profile-field-title">
                        моб. тел.
                    </td>
                    <td class="profile-field">
                        <asp:TextBox ID="txPhone" runat="server" Width="100%"></asp:TextBox>
                    </td>
                </tr>
                <tr>
                    <td class="profile-field-title">
                        пароль доступа
                    </td>
                    <td class="profile-field">
                        <asp:TextBox ID="txPassPers" runat="server" Width="50%"></asp:TextBox>
                    </td>
                </tr>
                <tr>
                    <td class="profile-field-title">
                        последние исзменения:
                    </td>
                    <td class="profile-field">
                        <asp:Label ID="lbLastCorrect" runat="server" Text="..."></asp:Label>
                    </td>
                </tr>
            </table>
        </asp:View>
    </asp:MultiView>
</asp:Content>
