<%@ Control Language="C#" AutoEventWireup="true" CodeBehind="CommandPanel4Profile.ascx.cs"  Inherits="wstcp.CommandPanel4Profile" %>

      
    <div class="command_Panel">
        <asp:LinkButton ID="lbtnOK" OnPreRender="lbtnOK_PreRender" runat="server" CssClass="button" OnClick="btnOK_Click"><img alt="[v]" src="../simg/16/tick.png"/> сохранить и закрыть</asp:LinkButton>
        <asp:LinkButton ID="lbtnSave" OnPreRender="lbtnSave_PreRender" runat="server" CssClass="button" OnClick="btnSave_Click" Visible="false"><img alt="[v]" src="../simg/16/save.png"/> сохранить</asp:LinkButton>
        <asp:LinkButton ID="lbtnCancel" runat="server" CssClass="button" OnClick="lbtnCancel_Click"><img alt="[c]" src="../simg/16/cancel.png"/> отмена</asp:LinkButton>
        <asp:LinkButton ID="lbtnDelete" runat="server" CssClass="button" OnClick="lbtnDelete_Click"><img alt="[x]" src="../simg/16/delete.png"/> удалить</asp:LinkButton>
    </div>
   

