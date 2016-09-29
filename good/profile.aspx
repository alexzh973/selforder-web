<%@ Page Title="" Language="C#" MasterPageFile="~/common/mainpage.Master" AutoEventWireup="true" CodeBehind="profile.aspx.cs" Inherits="wstcp.good.profile" %>

<%@ Register Src="../UC/ucDateInput.ascx" TagName="ucDateInput" TagPrefix="uc1" %>

<%@ Register src="../UC/BreadPath.ascx" tagname="BreadPath" tagprefix="uc2" %>

<asp:Content ID="Content1" ContentPlaceHolderID="head" runat="server">
</asp:Content>
<asp:Content ID="Content2" ContentPlaceHolderID="place_Left" runat="server">
</asp:Content>
<asp:Content ID="Content3" ContentPlaceHolderID="ContentPlaceHolder1" runat="server">
<uc2:BreadPath ID="BreadPath1" runat="server" />
    <div class="f-bwi">
        <asp:Image CssClass="f-bwi-pic" ID="imgGood" runat="server" />
        <asp:FileUpload ID="FileUpload1" runat="server" /><asp:Button ID="btnUploadImg" runat="server" Text="Загрузить" OnClick="btnUploadImg_Click" />
        
        <div class="f-bwi-text">
            <h2>
                <asp:Literal ID="lbName" runat="server"></asp:Literal></h2>
            <p>
                <asp:Literal ID="lbTNTK" runat="server"></asp:Literal></p>
            <p>цена
                <asp:Literal ID="lbPrice" runat="server"></asp:Literal></p>
            <h4>Свободные остатки </h4>
            <asp:Literal ID="tblIncash" runat="server"></asp:Literal>
        </div>
        <!-- f-bwi-text -->
    </div>
    <!-- f-bwi -->
    <h4>Участие в акциях</h4>
    <%--<div>
        <div class="floatLeft">
            <asp:CheckBoxList ID="chlAcc" runat="server"></asp:CheckBoxList>

        </div>
        <div class="floatLeft border padding5">
            новая акция<br />
            <asp:TextBox ID="txNewAcc" runat="server" Width="277px"></asp:TextBox>
            <br />
            старт
            <uc1:ucDateInput ID="ucFirstDay" runat="server" />
            - финиш
            <uc1:ucDateInput ID="ucLastDay" runat="server" />
            <br />


        </div>

        <div class="clearBoth"></div>
        <asp:Button ID="btnSaveNewAcc" runat="server" Text="ok" OnClick="btnSaveNewAcc_Click" Width="50px" />

    </div>--%>

    <div>
        <asp:DataGrid ID="dgAcc" runat="server" CellPadding="4" ForeColor="#333333" GridLines="None" AutoGenerateColumns="true">
            <AlternatingItemStyle BackColor="White" ForeColor="#284775" />
            <EditItemStyle BackColor="#999999" />
            <FooterStyle BackColor="#5D7B9D" Font-Bold="True" ForeColor="White" />
            <HeaderStyle BackColor="#5D7B9D" Font-Bold="True" ForeColor="White" />
            <ItemStyle BackColor="#F7F6F3" ForeColor="#333333" />
            <PagerStyle BackColor="#284775" ForeColor="White" HorizontalAlign="Center" />
            <SelectedItemStyle BackColor="#E2DED6" Font-Bold="True" ForeColor="#333333" />
        </asp:DataGrid>
    </div>



</asp:Content>
<asp:Content ID="Content4" ContentPlaceHolderID="place_Right" runat="server">
</asp:Content>
