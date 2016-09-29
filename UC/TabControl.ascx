<%@ Control Language="C#" AutoEventWireup="true" CodeBehind="TabControl.ascx.cs"  Inherits="TabControl" %>

<style type="text/css">

table.tabcontrol
{
    padding: 0;
    margin: 0;
    /*border-top: 5px solid white;*/
}
table.tabcontrol td
{
    padding-top: 8px;
    padding-left: 6px;
    padding-right: 6px;
    padding-bottom:6px;
    text-align: center;
}
td.tabactive
{
    color: #000000;
    background-color: #FFFFFF !important;
    border-top: 1px solid #666699;
    border-bottom: 0;
    border-left: 1px solid #666699;
    border-right: 1px solid #666699;
   /* border-radius: 8px 8px 1px 1px;
    -webkit-border-radius: 8px 8px 1px 1px;
    -moz-border-radius: 8px 8px 1px 1px;*/
}
td.tabpassive
{
    color: #999999;
    cursor: pointer;
    background-color: #DEDEDE;
    border-top: 1px solid #666699;
    border-bottom: 1px solid #666699;
    border-left: 1px solid #666699;
    border-right: 1px solid #666699;
    /*border-radius: 8px 8px 1px 1px;
    -webkit-border-radius: 8px 8px 1px 1px;
    -moz-border-radius: 8px 8px 1px 1px;*/
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
}
</style>



<input id="hdCurTabID" type="hidden" runat="server"/>
<asp:Literal ID="lit" runat="server" OnPreRender="_Container_PreRender"></asp:Literal>