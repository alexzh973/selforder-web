<%@ Page Title="" Language="C#" MasterPageFile="~/common/Preview.Master" AutoEventWireup="true" CodeBehind="card.aspx.cs" Inherits="wstcp.good.card" %>

<%@ Register src="../UC/ucDateInput.ascx" tagname="ucDateInput" tagprefix="uc1" %>

<asp:Content ID="Content1" ContentPlaceHolderID="head" runat="server">
</asp:Content>
<asp:Content ID="Content2" ContentPlaceHolderID="ContentPlaceHolder1" runat="server">

    <div class="f-bwi">
        <asp:Image CssClass="f-bwi-pic" ID="imgGood" runat="server" />
        <div class="f-bwi-text">
        <h2>
            <asp:Literal ID="lbName" runat="server"></asp:Literal></h2>
            <p><asp:Literal ID="lbTNTK" runat="server"></asp:Literal></p>
           <p>цена <asp:Literal ID="lbPrice" runat="server"></asp:Literal></p>
           <h4>Свободные остатки </h4>
            <asp:Literal ID="tblIncash" runat="server"></asp:Literal>
        </div>
        <!-- f-bwi-text -->
    </div>
    <!-- f-bwi -->
    <fieldset><legend>Участие в акциях</legend>
        <div class="floatLeft"><asp:DataGrid ID="dgAcc" runat="server" ShowHeader="False" AutoGenerateColumns="False">
            <Columns>
                <asp:BoundColumn DataField="AccName"></asp:BoundColumn>
                <asp:BoundColumn DataField="FirstDate" DataFormatString="{0:d}"></asp:BoundColumn>
                <asp:BoundColumn DataField="LastDate" DataFormatString="{0:d}"></asp:BoundColumn>
            </Columns>
            </asp:DataGrid></div>
        
        
        <div class="clearBoth"></div>
    </fieldset>
</asp:Content>
