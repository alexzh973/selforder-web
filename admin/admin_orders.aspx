<%@ Page Title="" Language="C#" MasterPageFile="~/admin/admin.Master" AutoEventWireup="true" CodeBehind="admin_orders.aspx.cs" Inherits="wstcp.admin_orders" %>
<asp:Content ID="Content1" ContentPlaceHolderID="head" runat="server">
</asp:Content>
<asp:Content ID="Content2" ContentPlaceHolderID="ContentPlaceHolder1" runat="server">
    <br/>
    
    <div class="f-row">

    <label>Клиент</label> <asp:DropDownList ID="dlSubject" runat="server" AutoPostBack="True" OnSelectedIndexChanged="btnSearch_Click"></asp:DropDownList></div>
    <div class="f-row"><label>Торговый агент</label> <asp:DropDownList ID="dlTa" runat="server" AutoPostBack="True" OnSelectedIndexChanged="btnSearch_Click"></asp:DropDownList></div>
    <div class="f-row"><label>найти по содержанию</label> <asp:TextBox ID="txSearch" runat="server"></asp:TextBox>
    <asp:Button ID="btnSearch" runat="server" Text="найти" OnClick="btnSearch_Click" CssClass="f-bu" /></div>
    <div class="f-row"><label>кто формировал заявку</label><asp:RadioButtonList ID="rbSelf" runat="server" AutoPostBack="True" OnSelectedIndexChanged="btnSearch_Click" RepeatDirection="Horizontal" RepeatLayout="Flow">
        <asp:ListItem Selected="True" Value="all">все вместе</asp:ListItem>
        <asp:ListItem Value="s">клиент самостоятельно</asp:ListItem>
        <asp:ListItem Value="1c">менеджер в 1С</asp:ListItem>
        </asp:RadioButtonList></div>
        
        <hr/>   
                        <asp:DataGrid ID="dgList" CssClass="grid" runat="server" AllowPaging="True" PageSize="10"
                            OnPageIndexChanged="dgList_PageIndexChanged" CellPadding="4"
                            OnItemDataBound="dgList_ItemDataBound" Width="100%" AutoGenerateColumns="False">
                            <HeaderStyle CssClass="mainbackcolor small bold white" />
                            <Columns>
                                <asp:BoundColumn DataField="id" HeaderText="№/код 1С">
                                    <HeaderStyle Width="100px" />
                                </asp:BoundColumn>
                                <asp:TemplateColumn HeaderText="Клиент/договор">
                                    <HeaderStyle Width="300px" />
                                </asp:TemplateColumn>
                                <asp:TemplateColumn HeaderText="Заявка">
                                    <HeaderStyle Width="300px" />
                                </asp:TemplateColumn>
                                <asp:BoundColumn DataField="summorder" HeaderText="Сумма, руб.">
                                    <HeaderStyle Width="80px" />
                                    <ItemStyle CssClass="right"></ItemStyle>
                                </asp:BoundColumn>
                                <asp:TemplateColumn HeaderText="состояние">
                                    <HeaderStyle Width="200px" />
                                </asp:TemplateColumn>
                                <asp:TemplateColumn HeaderText="последн. корр.">
                                    <HeaderStyle Width="120px" />
                                </asp:TemplateColumn>
                            </Columns>
                            <FooterStyle CssClass="mainbackcolor small bold white" />
                            <PagerStyle CssClass="bold mypager" HorizontalAlign="Center"
                                Font-Size="Large" Mode="NumericPages" Position="TopAndBottom" />
                            <SelectedItemStyle BackColor="#D1DDF1" Font-Bold="True" ForeColor="#333333" />
                        </asp:DataGrid>
    <asp:LinkButton ID="lbtnUpdateInvoice" runat="server" OnClick="lbtnUpdateInvoice_Click">сервисная кнопка</asp:LinkButton>
                    </asp:Content>
