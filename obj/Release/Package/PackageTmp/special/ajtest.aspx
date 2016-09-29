<%@ Page Title="" Language="C#" MasterPageFile="~/common/workpage.Master" AutoEventWireup="true" CodeBehind="ajtest.aspx.cs" Inherits="wstcp.special.ajtest" EnableEventValidation="false" %>
<asp:Content ID="Content1" ContentPlaceHolderID="head" runat="server">
    <script type="text/javascript">
        $(document).ready(function () {
            $("#dlBrend").change(selbyBrend);
            $("#dlCat").change(selbyCat);
            $("#dlName").change(selbyName);
        });

        function selbyBrend() {
            $.ajax({
                url: 'ajfilter.ashx?sid='+sid+'&src=brend&trg=cat&sel=' + $("#dlBrend").val(),
                success: function (data) {
                    $('#dlCat').html(data);
                }
            });
            $.ajax({
                url: 'ajfilter.ashx?sid=' + sid + '&src=brend&trg=name&sel=' + $("#dlBrend").val(),
                success: function (data) {
                    $('#dlName').html(data);
                }
            });
        }
        function selbyCat() {
            $.ajax({
                url: 'ajfilter.ashx?sid=' + sid + '&src=cat&trg=brend&sel=' + $("#dlCat").val(),
                success: function (data) {
                    $('#dlBrend').html(data);
                }
            });
            $.ajax({
                url: 'ajfilter.ashx?sid=' + sid + '&src=cat&trg=name&sel=' + $("#dlCat").val(),
                success: function (data) {
                    $('#dlName').html(data);
                }
            });
        }
        function selbyName() {
            $.ajax({
                url: 'ajfilter.ashx?sid=' + sid + '&src=name&trg=brend&sel=' + $("#dlName").val(),
                success: function (data) {
                    $('#dlBrend').html(data);
                }
            });
            $.ajax({
                url: 'ajfilter.ashx?sid=' + sid + '&src=name&trg=cat&sel=' + $("#dlName").val(),
                success: function (data) {
                    $('#dlCat').html(data);
                }
            });
        }
    </script>
</asp:Content>
<asp:Content ID="Content2" ContentPlaceHolderID="ContentPlaceHolder1"  runat="server">
    <div id="sss"></div>
   <div>Бренд <asp:DropDownList ID="dlBrend" ClientIDMode="Static" runat="server"></asp:DropDownList></div>
    <div>Категория<asp:DropDownList ID="dlCat"  ClientIDMode="Static" runat="server" OnLoad="dlCat_Load"></asp:DropDownList></div>
    <div>Наименование<asp:DropDownList ID="dlName" ClientIDMode="Static" runat="server"></asp:DropDownList>
        <asp:Button ID="Button1" runat="server" OnClick="Button1_Click" Text="Button" />
    </div>

</asp:Content>
