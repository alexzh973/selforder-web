<%@ Control Language="C#" AutoEventWireup="true" CodeBehind="CommandPanel4List.ascx.cs"
    Inherits="wstcp.CommandPanel4List" %>
    


    <table class="toolbar">
        <tr>
            <td>
                <asp:Label ID="lbTitle" runat="server" Text="" CssClass="toolbar" Visible="false"></asp:Label>
            </td>
            <td align="center" valign="middle" nowrap>
                <asp:LinkButton ID="btnNew" OnClick="bntNew_Click" runat="server" CssClass="button"><img src='../simg/16/add.png'>&nbsp;создать</asp:LinkButton>
            </td>
            <td align="center" valign="middle" nowrap>
                <asp:LinkButton ID="btnEdit" runat="server" OnClick="bntEdit_Click" CssClass="button"><img src='../simg/16/document-edit.png'>&nbsp;изменить</asp:LinkButton>
            </td>
            <td align="center" valign="middle" nowrap>
                <asp:LinkButton ID="btnCopy" runat="server" OnClick="bntCopy_Click" CssClass="button"><img src='../simg/16/page_copy.png'>&nbsp;копировать</asp:LinkButton>
            </td>
            <td align="center" valign="middle" nowrap>
                <asp:LinkButton ID="btnDelete" runat="server" OnClick="bntDelete_Click" CssClass="button"
                    OnPreRender="btnDelete_PreRender" Visible="false"><img src='../simg/delete.png'>&nbsp;удалить</asp:LinkButton>
            </td>
        </tr>
    </table>
