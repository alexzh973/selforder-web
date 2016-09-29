<%@ Page Title="" Language="C#" MasterPageFile="~/admin/admin.Master" AutoEventWireup="true" CodeBehind="admin_mail.aspx.cs" Inherits="wstcp.admin_mail" %>
<asp:Content ID="Content1" ContentPlaceHolderID="head" runat="server">
</asp:Content>

<asp:Content ID="Content3" ContentPlaceHolderID="ContentPlaceHolder1" runat="server">
    <div><asp:TextBox ID="txBcc" runat="server" Width="100%"></asp:TextBox></div>
    <div><asp:TextBox ID="txTitle" runat="server" Width="100%"></asp:TextBox></div>
    <div><asp:TextBox ID="txMsg" runat="server" TextMode="MultiLine" Rows="5" Width="100%"></asp:TextBox></div>
    <div><asp:Button ID="btnSend" runat="server" OnClick="btnSend_Click" Text="отправить" /></div>
</asp:Content>

