<%@ Page Title="" Language="C#" MasterPageFile="~/admin/admin.Master" 
    AutoEventWireup="true" CodeBehind="admin_syssetting.aspx.cs" Inherits="wstcp.admin_syssetting" %>

<%@ Register Src="../UC/CommandPanel4List.ascx" TagName="CommandPanel4List" TagPrefix="uc1" %>
<%@ Register Src="../UC/CommandPanel4Profile.ascx" TagName="CommandPanel4Profile" TagPrefix="uc2" %>

<asp:Content ID="Content1" ContentPlaceHolderID="head" runat="Server">
    <script language="javascript" type="text/javascript">
        function show_detail(obj_id) {
            if (obj_id == '' && $("#preview")) {
            }
            else {
                $("#preview").load("?act=view&id=" + obj_id);
            }

            return;
        }
        function openByParent(new_url) {
            document.location = new_url;
        }
    </script>
    </asp:Content>



<asp:Content ID="Content2" ContentPlaceHolderID="ContentPlaceHolder1" runat="Server">
    <asp:Literal ID="lbMessage" runat="server"></asp:Literal>
    <div class="just">
        
        <asp:MultiView ID="MultiView1" runat="server">
            <asp:View ID="vList" runat="server">
                <uc1:CommandPanel4List ID="CommandPanel4List1" runat="server" />
                <asp:DataGrid ID="dgList" runat="server" AutoGenerateColumns="False" CellPadding="4"
                    ForeColor="#333333" GridLines="None" OnSelectedIndexChanged="dgList_SelectedIndexChanged1">
                    <AlternatingItemStyle BackColor="White" ForeColor="#284775" />
                    <Columns>
                        <asp:BoundColumn DataField="Var" HeaderText="Ключ"></asp:BoundColumn>
                        
                        <asp:BoundColumn DataField="Value" HeaderText="Значение"></asp:BoundColumn>
                        <asp:ButtonColumn CommandName="Select" Text="..."></asp:ButtonColumn>
                    </Columns>
                    <EditItemStyle BackColor="#999999" />
                    <FooterStyle BackColor="#5D7B9D" Font-Bold="True" ForeColor="White" />
                    <HeaderStyle BackColor="#5D7B9D" Font-Bold="True" ForeColor="White" />
                    <ItemStyle BackColor="#F7F6F3" ForeColor="#333333" />
                    <PagerStyle BackColor="#284775" ForeColor="White" HorizontalAlign="Center" />
                    <SelectedItemStyle BackColor="#E2DED6" Font-Bold="True" ForeColor="#333333" />
                </asp:DataGrid>
            </asp:View>
            <asp:View ID="vOrgProfile" runat="server">
                <table class="profile" cellpadding="3">
                    <tr>
                        <td class="toolbar-top" colspan="2">
                            <uc2:CommandPanel4Profile ID="cmdProfileOrg" runat="server" />
                        </td>
                    </tr>
                    
                    <tr>
                        <td class="profile-field-name">
                            Код
                        </td>
                        <td class="profile-field">
                            <asp:TextBox ID="txName" runat="server" MaxLength="100" Width="100%"></asp:TextBox>
                        </td>
                    </tr>
                    
                    <tr>
                        <td class="profile-field-name">
                            Значение
                        </td>
                        <td class="profile-field">
                            <asp:TextBox ID="txDescr" runat="server" MaxLength="500" Rows="4" TextMode="MultiLine"
                                Width="100%"></asp:TextBox>
                        </td>
                    </tr>
                </table>
            </asp:View>
        </asp:MultiView>
    </div>
    <div id="detail">
    </div>
</asp:Content>
