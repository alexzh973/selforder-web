<%@ Page Title="" Language="C#" MasterPageFile="~/common/workpage.Master" AutoEventWireup="true" CodeBehind="view.aspx.cs" Inherits="wstcp.news.view" %>
<asp:Content ID="Content1" ContentPlaceHolderID="head" runat="server">
</asp:Content>

<asp:Content ID="Content3" ContentPlaceHolderID="ContentPlaceHolder1" runat="server">
   <div class="commandPanel">
        <a href="#" onclick="printit()" class="button">
            <img src="../simg/16/printer.png">распечатать</a></div>
    <h2>
        <asp:Literal ID="lbTitleRecord" runat="server"></asp:Literal></h2>
    <div class="attributes">
        <table>
            <tr>
                <td>
                    <a href='#' class="button">
                        <asp:Image ID="Image1" runat="server" /></a>
                </td>
                <td width="98%">
                    <asp:Label ID="lbDescr" runat="server" Text="" CssClass="descrrecord"></asp:Label>
                </td>
            </tr>
        </table>
    </div>
    <asp:Label ID="lbText" runat="server" Text=""></asp:Label>
    <hr />
    <div class="micro" style="background-color: #EEEEEE;">
        <asp:Label ID="lbAttributes" runat="server" Text=""></asp:Label></div>
    <div class="micro" style="background-color: #EEEEEE;">
        
        <asp:Label ID="lbOrgsview" runat="server" Text="" CssClass="attributes small"></asp:Label></div>
</asp:Content>
