<%@ Page Title="" Language="C#" MasterPageFile="~/admin/admin.Master" AutoEventWireup="true" CodeBehind="admin_sysmsg.aspx.cs" Inherits="wstcp.admin_sysmsg" %>
<asp:Content ID="Content1" ContentPlaceHolderID="head" runat="server">
</asp:Content>

<asp:Content ID="Content3" ContentPlaceHolderID="ContentPlaceHolder1" runat="server">
    <asp:TextBox ID="txSysmsg" runat="server" TextMode=MultiLine Rows="7" MaxLength="499" Width="80%"></asp:TextBox><br/>
    <asp:Button ID="btnSend" runat="server" Text="сохранить сообщение" 
        onclick="btnSend_Click" />
<hr/>
    <asp:DataGrid ID="dgList" runat="server" AutoGenerateColumns="False" 
        ondeletecommand="dgList_DeleteCommand">
        <Columns>
            <asp:BoundColumn DataField="id" HeaderText="id"></asp:BoundColumn>
            <asp:BoundColumn DataField="msgtxt" HeaderText="сообщение"></asp:BoundColumn>
            <asp:BoundColumn DataField="state" HeaderText="статус"></asp:BoundColumn>
            <asp:BoundColumn DataField="lc" HeaderText="автор"></asp:BoundColumn>
            <asp:BoundColumn DataField="lcd" HeaderText="дата"></asp:BoundColumn>
            <asp:ButtonColumn CommandName="Delete" Text="Удалить"></asp:ButtonColumn>
        </Columns>
    </asp:DataGrid>
</asp:Content>

