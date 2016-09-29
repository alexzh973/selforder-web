<%@ Page Title="" Language="C#" MasterPageFile="~/common/mainpage.Master" AutoEventWireup="true" CodeBehind="newreg.aspx.cs" Inherits="wstcp.account.newreg" %>
<asp:Content ID="Content1" ContentPlaceHolderID="head" runat="server">
</asp:Content>
<asp:Content ID="Content2" ContentPlaceHolderID="place_Left" runat="server" >
</asp:Content>
<asp:Content ID="Content3" ContentPlaceHolderID="ContentPlaceHolder1" runat="server">
    <table>
        <tr>
           
            <td class="profile-field"><p><strong>ИНН</strong><br/>
                <asp:TextBox ID="txINN" runat="server" MaxLength="11"></asp:TextBox></p></td>
        </tr>
        <tr>
            
            <td class="profile-field"><p><strong>Наименование компании</strong><br/><asp:TextBox ID="txName" runat="server" MaxLength="250" Width="100%"></asp:TextBox></p></td>
        </tr>
        <tr>
                  
                    <td class="profile-field"><p><strong>Имя представителя</strong><br/>
                        <asp:TextBox ID="txNamePers" runat="server" MaxLength="250" Width="100%"></asp:TextBox></p>
                    </td>
                </tr>
                <tr>
                    
                    <td class="profile-field"><p><strong>e-mail представителя</strong><br/>
                        <asp:TextBox ID="txEmailPers" runat="server" MaxLength="250" Width="100%"></asp:TextBox></p>
                    </td>
                </tr>
        
        <tr>
            
            <td>
                <asp:Button ID="btnOk" runat="server" Text="зарегистрирвать" OnClick="btnOk_Click" /> </td>
        </tr>

    </table>
</asp:Content>
<asp:Content ID="Content4" ContentPlaceHolderID="place_Right" runat="server">
</asp:Content>
