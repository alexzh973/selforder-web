<%@ Control Language="C#" AutoEventWireup="true" CodeBehind="CaptchaControl.ascx.cs" Inherits="wstcp.CaptchaControl" %>
<style type="text/css">
imgcap
{
    
}
</style>

<asp:Image CssClass="imgcap" ImageAlign="AbsMiddle"  ID="img" runat="server" ImageUrl="~/account/CaptchaHandler.ashx" />&nbsp;<asp:TextBox 
    ID="txCap" MaxLength=6 runat="server" Width="81px"></asp:TextBox>