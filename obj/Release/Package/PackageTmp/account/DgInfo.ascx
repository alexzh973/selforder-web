<%@ Control Language="C#" AutoEventWireup="true" CodeBehind="DgInfo.ascx.cs" Inherits="wstcp.DgInfo" %>
<script type="text/javascript"></script>
<div>
    Текущее состояние по взаиморасчетам <a class="micro" title="подробно..." onclick="javascript:$('#dginfo').show('fast');" href="#"><asp:Literal ID="lbAvail" runat="server"></asp:Literal></a>
    <div id="dginfo" class="borderlight small">
    <div class="padding5">
        <div>Договор
        <asp:Literal ID="lbDG" runat="server"></asp:Literal></div>
    <div>Условия договора:</div>
    <div>&nbsp;&nbsp;срок действия
        <asp:Literal ID="lbPeriod" runat="server"></asp:Literal></div>
    <div>&nbsp;&nbsp;Лимит <span title="Сумма возможного товарного кредита">ДЗ <strong>
        <asp:Literal ID="lbLDZ" runat="server"></asp:Literal></strong></span></div>
        <div>&nbsp;&nbsp;Допуст. отсрочка  <strong>
        <asp:Literal ID="lbWait" runat="server"></asp:Literal></strong></div>
    <div>&nbsp;&nbsp;Текущий баланс <strong>
        <asp:Literal ID="lbCurrBlnc" runat="server"></asp:Literal></strong></div>
        </div>
        <div class="right grayfon"><a class="micro"  onclick="javascript:$('#dginfo').hide('fast');" href="#">^ свернуть</a></div>
</div>
</div>
<script type="text/javascript">
    javascript: $('#dginfo').hide();
</script>