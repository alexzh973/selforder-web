<%@ Control Language="C#" AutoEventWireup="true" CodeBehind="UploadThrumbPicture.ascx.cs" 
    Inherits="UploadThrumbPicture" %>
    <script type="text/javascript">

        $(document).ready(function () {
            $(".thrumbuploadfield").bind("mouseout", function (event) { checkthrumbuploadfield(event.target); });
        });


        function checkthrumbuploadfield(ctrl) {
            var r = ($(ctrl).attr('value') == "") ? 'нет' : 'есть';
            if (r == "есть") $("form").submit();
            return;
        }

</script>
<div>
    <div style="float: left">
        <asp:Literal ID="lbImg" runat="server"></asp:Literal><br/>
        <asp:LinkButton ID="btnDelete" runat="server" onclick="btnDelete_Click">удалить</asp:LinkButton>
        </div>
    <div style="float: left">
        <label class="small">
            выберите изображение</label><br />
        <asp:FileUpload ID="fuImg" runat="server" CssClass="thrumbuploadfield" /><br />
        <asp:Literal ID="lbResult" runat="server"></asp:Literal></div>
    <div style="clear: both">
    </div>
</div>
