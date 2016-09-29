<%@ Control Language="C#" AutoEventWireup="true" CodeBehind="TabCtrl.ascx.cs" Inherits="TabCtrl" %>
<style type="text/css">

table.tabcontrol
{
    padding: 0;
    margin: 0;
    /*border-top: 5px solid white;*/
}
table.tabcontrol td a {
    text-decoration: none;
    /*padding-right: 20px !important;
    padding-left: 20px !important;*/
    line-height: 12px;
}
table.tabcontrol td
{
    padding-top: 8px;
    padding-left: 16px;
    padding-right: 16px;
    padding-bottom:6px;
    text-align: center;
    line-height: 100%;
vertical-align: middle;
width:200px;
}
td.tabactive
{
    color: #000000;/**/
    background-color: #FFFFFF !important;
border-radius: 8px 8px 1px 1px;
    -webkit-border-radius: 8px 8px 1px 1px;
    -moz-border-radius: 8px 8px 1px 1px;    
    border-top: 1px solid #666699;
    border-bottom: 0;
    border-left: 1px solid #666699;
    border-right: 1px solid #666699;
    
}
td.tabpassive
{
    color: #999999;
    cursor: pointer;
    background-color: #DEDEDE;
    /*border-top: 1px solid #666699;*/
    border-bottom: 1px solid #666699;
    /*border-left: 1px solid #666699;*/
    /*border-right: 1px solid #666699;*/
    /*border-radius: 8px 8px 1px 1px;*/
    -webkit-border-radius: 8px 8px 1px 1px;
    -moz-border-radius: 8px 8px 1px 1px;
}

.tabactive a {
    color: #333333;
}
.tabpassive a {
    color:#999999;
}
td.tabdivider
{
    width: 4px;
    background-color: #FFFFFF;
    border-top: 0;
    border-bottom: 1px solid #666699;
    border-left: 0px solid #666699;
    border-right: 0px solid #666699;
    padding-left: 0px !important;
    padding-right: 0px !important;
}


td.tabspace
{
    width: 98% !important;
    border-bottom: 1px solid #666699;
}
</style>
<asp:Repeater ID="Repeater1" runat="server" OnItemCommand="ItemCommand">
    <HeaderTemplate><table class="tabcontrol" cellpadding="3" cellspacing="0" ><tr></HeaderTemplate>
    <SeparatorTemplate><td class="tabdivider">&nbsp;</td></SeparatorTemplate>
    <ItemTemplate><td class="<%# getCss(Container) %> <%#((TabItem)Container.DataItem).Css %>">
        
        <asp:LinkButton ID="LinkButton1"  runat="server" 
            ToolTip="<%# ((TabItem)Container.DataItem).Tooltip %>">
            <%# ((TabItem)Container.DataItem).Title %> 
        </asp:LinkButton>
          
                  </td></ItemTemplate>
    <FooterTemplate><td class="tabspace"></td></tr></table></FooterTemplate>
</asp:Repeater>
