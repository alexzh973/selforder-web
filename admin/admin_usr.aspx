<%@ Page Title="" Language="C#" MasterPageFile="admin.Master" AutoEventWireup="true"
    CodeBehind="admin_usr.aspx.cs" Inherits="wstcp.admin_usr" %>

<%@ Register Src="../UC/Alphabet.ascx" TagName="Alphabet" TagPrefix="uc4" %>
<%@ Register Src="../UC/BreadPath.ascx" TagName="BreadPath" TagPrefix="uc5" %>
<%@ Register Src="../UC/CommandPanel4Profile.ascx" TagName="CommandPanel4Profile"
    TagPrefix="uc1" %>

<%@ Register src="../UC/CommandPanel4List.ascx" tagname="CommandPanel4List" tagprefix="uc2" %>



<%@ Register src="../UC/UploadThrumbPicture.ascx" tagname="UploadThrumbPicture" tagprefix="uc3" %>

<asp:Content ID="Content1" ContentPlaceHolderID="head" runat="Server">
    
</asp:Content>

<asp:Content ID="Content2" ContentPlaceHolderID="ContentPlaceHolder1" runat="Server">
   
    <h2>Пользователи</h2>    
    <asp:Label ID="lbMessage" runat="server" CssClass="message"></asp:Label>


    <asp:MultiView ID="MultiView1" runat="server">
        <asp:View ID="vDetail" runat="server">
            <asp:LinkButton ID="aBackToList" runat="server">вернуться ..</asp:LinkButton>
            <div id="detail">
            </div>
        </asp:View>
        <asp:View ID="vList" runat="server">
     <uc2:CommandPanel4List ID="CommandPanel4List1" runat="server" />
    <asp:Panel ID="pnlFilter" runat="server">
          
            <div class="_border _grayfon small">
                
                    <span class="bold">Фильтр</span>: <br/>
                Владелец: <asp:DropDownList ID="dlOrgList" runat="server" AutoPostBack="True" CssClass="small" 
                    OnSelectedIndexChanged="btnSearch_Click">
                </asp:DropDownList>&nbsp;|&nbsp;
                Активирован: <asp:DropDownList ID="dlLoginEnabled" runat="server"  AutoPostBack="True" CssClass="small"
                    OnSelectedIndexChanged="btnSearch_Click">
                    <asp:ListItem Value="Y">Вход разрешен</asp:ListItem>
                    <asp:ListItem Value="N">Вход запрещен</asp:ListItem>
                    <asp:ListItem Value="O">Все</asp:ListItem>
                </asp:DropDownList>&nbsp;|&nbsp;
                Состояние: <asp:DropDownList ID="dlState" runat="server" AutoPostBack="True"  CssClass="small"
                    OnSelectedIndexChanged="btnSearch_Click">
                    <asp:ListItem Value="Y">Активные</asp:ListItem>
                    <asp:ListItem Value="N">Удаленные</asp:ListItem>
                    <asp:ListItem Value="O">Все</asp:ListItem>
                </asp:DropDownList><br/>
                <asp:CheckBox ID="chUseGroups" runat="server" AutoPostBack="True" 
                    Checked="True" OnCheckedChanged="btnSearch_Click" Text="иерархия" />
            </div></asp:Panel>       
            
            <input id="curid" type="hidden" class="curid" runat="server" />
            <input id="curpid" type="hidden" class="curpid" runat="server" />
            <hr />
            <uc4:Alphabet ID="Alphabet1" runat="server" />
            <div>
                <div>
                    поиск
                    <asp:TextBox ID="txSearch" runat="server" CssClass="searchactive"
                        Width="200px" OnPreRender="txSearch_PreRender" ToolTip="часть фио/e-mail/телефон"
                        TabIndex="1"></asp:TextBox>
                    &nbsp;<asp:Button ID="btnSearch" runat="server" OnClick="btnSearch_Click" Text="найти"
                        TabIndex="2" /></div>
                <script type="text/javascript">
                    function search_blank(inp, g) {
                        if (g == 1) {
                            if ($('#' + inp).val() == 'часть фио/e-mail/телефон') {
                                $('#' + inp).val('');
                                $('#' + inp).toggleClass('searchactive', true);
                                $('#' + inp).focus();
                                return;
                            }
                        }
                        else {
                            if ($('#' + inp).val() == '') {
                                $('#' + inp).val('часть фио/e-mail/телефон');
                                $('#' + inp).toggleClass('searchpassive', true);
                            }
                        }
                    }       
                </script>
                <asp:Literal ID="lbParent" runat="server"></asp:Literal>
            </div>
            <asp:DataGrid ID="dgList" runat="server" AllowPaging="True" AutoGenerateColumns="False"
                OnItemDataBound="dgList_ItemDataBound" 
                OnSelectedIndexChanged="dgList_SelectedIndexChanged" CellPadding="4" 
                ForeColor="#333333" GridLines="None" OnPageIndexChanged="dgList_PageIndexChanged">
                <AlternatingItemStyle BackColor="White" ForeColor="#284775" />
                <Columns>
                    <asp:BoundColumn DataField="ID" Visible="false">
                        <ItemStyle />
                    </asp:BoundColumn>
                    <asp:TemplateColumn ItemStyle-Width="20px">
                        <ItemStyle Width="20px" />
                    </asp:TemplateColumn>
                    <asp:BoundColumn DataField="Name" ItemStyle-Width="50%">
                        <ItemStyle Width="50%" />
                    </asp:BoundColumn>
                    <asp:BoundColumn DataField="Email" ItemStyle-Width="30%">
                        <ItemStyle Width="30%" />
                    </asp:BoundColumn>
                    <asp:BoundColumn DataField="Phones" ItemStyle-Width="30%">
                        <ItemStyle Width="20px" />
                    </asp:BoundColumn>
                    <asp:TemplateColumn FooterStyle-CssClass="micro" ItemStyle-Width="40px">
                        <FooterStyle CssClass="micro" />
                        <ItemStyle Width="40px" />
                    </asp:TemplateColumn>
                    
                </Columns>
                <EditItemStyle BackColor="#999999" />
                <FooterStyle BackColor="#5D7B9D" Font-Bold="True" ForeColor="White" />
                <HeaderStyle BackColor="#5D7B9D" Font-Bold="True" ForeColor="White" />
                <ItemStyle BackColor="#F7F6F3" ForeColor="#333333" />
                <PagerStyle BackColor="#284775" ForeColor="White" HorizontalAlign="Center" Mode="NumericPages" />
                <SelectedItemStyle BackColor="#E2DED6" Font-Bold="True" ForeColor="#333333" />
            </asp:DataGrid>
        </asp:View>
        <asp:View ID="vProfile" runat="server">
            <h2>Профайл пользователя</h2>
                <table cellspacing="1" cellpadding="3" width="600px" class="profile shadow">
                    
                    <asp:PlaceHolder ID="place4admin" runat="server">
                        <tr>
                            <td class="profile-field-title">
                                ID
                            </td>
                            <td class="profile-field">
                            
                                <asp:TextBox ID="txID" runat="server" Width="80px" ReadOnly="True"></asp:TextBox>
                                <asp:CheckBox ID="chLoginEnable" runat="server" Text="разрешен ли вход в систему?" /> <asp:CheckBox ClientIDMode="Predictable" ID="chIsFolder" runat="server" AutoPostBack="True" OnCheckedChanged="chIsFolder_SelectedIndexChanged"
                                    Text="группа ли это?" />
                            </td>
                        </tr>
                        
                        
                        <tr>
                            <td class="profile-field-title">
                                входит в группу
                            </td>
                            <td class="profile-field">
                            
                                <asp:DropDownList ID="dlParent" runat="server" Width="50%">
                                </asp:DropDownList>
                            </td>
                        </tr>
                    </asp:PlaceHolder>
                    <tr>
                        <td class="profile-field-title">
                            Имя кратко
                            
                            
                        </td>
                        <td class="profile-field">
                            <asp:TextBox ID="txName" runat="server"  MaxLength="100" Width="100%"></asp:TextBox>
                        </td>
                    </tr> 
                    <tr>
                            <td class="profile-field-name" valign="top">
                                фото<br />
                                <br />
                                <asp:Literal ID="divPreview" runat="server"></asp:Literal>
                            </td>
                            <td class="profile-field">
                                <asp:FileUpload ID="FileUpload1" runat="server" /><br />
                                <asp:ImageButton ID="ImageButton1" runat="server" ImageUrl="~/SIMG/16/delete.png"
                                    OnClick="ImageButton1_Click" />
                                <asp:Button ID="btnLoadPic" runat="server" OnClick="btnLoadPic_Click" Text="загрузить" />
                                <asp:HiddenField ID="hdPhoto" runat="server" />
                            </td>
                        </tr>
                    <tr>
                        <td class="profile-field-title">
                           Внешний код                          
                        </td>
                        <td class="profile-field">
                            <asp:TextBox ID="txExtcode" runat="server" MaxLength="100" Width="50%"></asp:TextBox>
                        </td>
                    </tr>
                    <asp:PlaceHolder ID="placeNotFolder" runat="server">
                        <tr>
                            <td  class="profile-field-title">
                                набор прав
                            </td>
                            <td class="profile-field">
                                <asp:CheckBoxList ID="UserRights" runat="server">
                                <asp:ListItem Value="SADM" Text="Супер-админ"></asp:ListItem>
                                <asp:ListItem Value="UADM" Text="Аминистратор (от филиала)"></asp:ListItem>
                                <asp:ListItem Value="TP" Text="Торговый представитель"></asp:ListItem>
                               
                                </asp:CheckBoxList>
                                <asp:TextBox ID="txType" runat="server" Width="100%" ReadOnly="True"></asp:TextBox>
                                
                            </td>
                        </tr>
                        <asp:PlaceHolder ID="blockPassword" runat="server">
                            <tr>
                                <td  class="profile-field-title">
                                    старый пароль доступа
                                </td>
                                <td class="profile-field">
                                    <asp:TextBox ID="txOldPass" runat="server" TextMode="Password" Width="50%"></asp:TextBox>[<asp:Label
                                        ID="lbPsw" runat="server" Text=""></asp:Label>]
                                    &nbsp;
                                </td>
                            </tr>
                            <tr>
                                <td class="profile-field-title">
                                    новый пароль доступа
                                </td>
                                <td class="profile-field">
                                    <asp:TextBox ID="txNewPass" runat="server" TextMode="Password" Width="50%"></asp:TextBox>
                                </td>
                            </tr>
                        </asp:PlaceHolder>
                        <tr>
                            <td class="profile-field-title">
                                e-mail
                            </td>
                            <td class="profile-field">
                                <asp:TextBox ID="txEmail" runat="server" Width="100%"></asp:TextBox>
                            </td>
                        </tr>
                    <tr>
                        <td class="profile-field-title">
                           Телефоны                          
                        </td>
                        <td class="profile-field">
                            <asp:TextBox ID="txPhones" runat="server" MaxLength="250" Width="100%"></asp:TextBox>
                        </td>
                    </tr><tr>
                        <td class="profile-field-title">
                           Примечание                          
                        </td>
                        <td class="profile-field">
                            <asp:TextBox ID="txDescr" runat="server" MaxLength="500" TextMode="MultiLine" Rows="3" Width="100%"></asp:TextBox>
                        </td>
                    </tr>


                    
                    <tr><td class="profile-field-title">представитель контрагента
                        
                        
                        </td><td class="profile-field">
                            <asp:DropDownList ID="dlSubject" OnSelectedIndexChanged="dlSubject_OnSelectedIndexChanged" runat="server" Width="50%" AutoPostBack="True">
                            </asp:DropDownList>
                        
                        </td></tr>
                        
                        <tr><td class="profile-field-title">владелец
                        
                        
                        </td><td class="profile-field">
                            <asp:DropDownList ID="dlOwner" runat="server" Width="50%">
                            </asp:DropDownList>
                        
                        </td></tr>
                        </asp:PlaceHolder>
                    <tr>
                        <td class="profile-field-title">
                            последние исзменения:</td>
                        <td>
                            <asp:Label ID="lbLastCorrect" runat="server" Text="..."></asp:Label>
                        </td>
                    </tr>
                    <tr><td colspan="2" class="profile-button">
                        <uc1:CommandPanel4Profile ID="CommandPanel4Profile1" runat="server" />
                        </td></tr>
                </table>
            <asp:FormView ID="FormView1" runat="server" CellPadding="4" ForeColor="#333333">
            
                <EditRowStyle BackColor="#999999" />
                <FooterStyle BackColor="#5D7B9D" Font-Bold="True" ForeColor="White" />
                <HeaderStyle BackColor="#5D7B9D" Font-Bold="True" ForeColor="White" />
                <PagerStyle BackColor="#284775" ForeColor="White" HorizontalAlign="Center" />
                <RowStyle BackColor="#F7F6F3" ForeColor="#333333" />
            
            </asp:FormView>
                
                
            </div>
            
        </asp:View>
    </asp:MultiView>
   
    </asp:Content>
