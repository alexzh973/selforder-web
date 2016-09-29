<%@ Page Title="" Language="C#" MasterPageFile="~/common/mainpage.Master" AutoEventWireup="true" CodeBehind="testtesttest.aspx.cs" Inherits="wstcp.testtesttest" %>
<%@ Register src="UC/TabCtrl.ascx" tagname="TabCtrl" tagprefix="uc1" %>
<%@ Register Src="~/UC/ucDateInput.ascx" TagPrefix="uc1" TagName="ucDateInput" %>

<asp:Content ID="Content1" ContentPlaceHolderID="head" runat="server">
</asp:Content>
<asp:Content ID="Content2" ContentPlaceHolderID="place_Left" runat="server">
</asp:Content>
<asp:Content ID="Content3" ContentPlaceHolderID="ContentPlaceHolder1" runat="server">
    <uc1:TabCtrl ID="TabCtrl1" OnSelectionChanged="Tab_OnSelectionChanged" runat="server" />
    <uc1:ucDateInput runat="server" OnSelectionChanged="myDate_SelectionChanged" ID="myDate" />
    <hr>
    <asp:Label ID="Label1" runat="server" Text="Label"></asp:Label>
</asp:Content>
<asp:Content ID="Content4" ContentPlaceHolderID="place_Right" runat="server">
</asp:Content>
