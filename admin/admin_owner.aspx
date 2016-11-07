<%@ Page Title="" Language="C#" MasterPageFile="~/admin/admin.Master" AutoEventWireup="true" CodeBehind="admin_owner.aspx.cs" Inherits="wstcp.admin_owner" ValidateRequest="false" %>

<%@ Register Src="../UC/mediaBrowser.ascx" TagName="mediaBrowser" TagPrefix="uc1" %>
<asp:Content ID="Content1" ContentPlaceHolderID="head" runat="server">
    <!-- TinyMCE -->
    
    <script src="../jscripts/tiny_mce/tiny_mce.js" type="text/javascript"></script>
    <script type="text/javascript">
        tinyMCE.init({
            mode: "exact",
            elements: "txText,txInvoiceHead,txInvoiceFoot",

            theme: "advanced",
            plugins: "pagebreak,style,layer,table,save,advhr,advimage,advlink,emotions,iespell,insertdatetime,preview,media,searchreplace,print,contextmenu,paste,directionality,fullscreen,noneditable,visualchars,nonbreaking,xhtmlxtras,template,inlinepopups,autosave",

            // Theme options
            theme_advanced_buttons1: "bold,italic,underline,strikethrough,|,fontselect,fontsizeselect,|,sub,sup,|,forecolor,backcolor,|,removeformat,code,undo,redo,|",
            theme_advanced_buttons2: "bullist,numlist,|,justifyleft,justifycenter,justifyright,justifyfull,|,outdent,indent,|,link,unlink,cleanup,|,hr,charmap,emotions,|",
            theme_advanced_buttons3: "",
            theme_advanced_toolbar_location: "top",
            theme_advanced_toolbar_align: "left",
            theme_advanced_statusbar_location: "none",
            theme_advanced_resizing: true,

            // Example word content CSS (should be your site CSS) this one removes paragraph margins
            //content_css: false,
            content_css: "../jscripts/tiny_mce/css/word.css",
            template_external_list_url: "../jscripts/tiny_mce/lists/template_list.js",
            external_link_list_url: "../jscripts/tiny_mce/lists/link_list.js",
            external_image_list_url: "../jscripts/tiny_mce/lists/image_list.js",
            media_external_list_url: "../jscripts/tiny_mce/lists/media_list.js",

            // Replace values for the template plugin
            template_replace_values: {
                username: "Some User",
                staffid: "991234"
            }
        });
    </script>
    <style type="text/css">
        #txText_tbl {
            background-color: White;
        }

            #txText_tbl.p {
                padding: 3px 3px 5px 3px !important;
            }
    </style>
    <!-- /TinyMCE -->
</asp:Content>

<asp:Content ID="Content2" ContentPlaceHolderID="ContentPlaceHolder1" runat="server">






    



    <asp:MultiView ID="mv" runat="server">
        <asp:View ID="vList" runat="server">
         <h2>список предприятий</h2>
    <asp:LinkButton ID="btnNewOwner" runat="server" OnClick="btnNewOwner_Click">новый</asp:LinkButton>

    <asp:DataGrid ID="dgOwners" runat="server" AutoGenerateColumns="False"
        CellPadding="4" ForeColor="#333333" GridLines="None"
        OnEditCommand="dgOwners_EditCommand">
        <AlternatingItemStyle BackColor="White" ForeColor="#284775" />
        <Columns>
            <asp:BoundColumn DataField="ID" ReadOnly="True" HeaderText="ID"></asp:BoundColumn>
            <asp:BoundColumn DataField="Name" HeaderText="Название"></asp:BoundColumn>
            <asp:BoundColumn DataField="Adrs" HeaderText="ip"></asp:BoundColumn>
            <asp:BoundColumn DataField="State" ReadOnly="True" HeaderText="S"></asp:BoundColumn>
            <asp:EditCommandColumn
                CancelText="отмена"
                EditText="..."
                UpdateText="сохранить"></asp:EditCommandColumn>
            <asp:ButtonColumn CommandName="Delete" Text="[x]"></asp:ButtonColumn>
        </Columns>
        <EditItemStyle BackColor="#999999" />
        <FooterStyle BackColor="#5D7B9D" Font-Bold="True" ForeColor="White" />
        <HeaderStyle BackColor="#5D7B9D" Font-Bold="True" ForeColor="White" />
        <ItemStyle BackColor="#F7F6F3" ForeColor="#333333" />
        <PagerStyle BackColor="#284775" ForeColor="White" HorizontalAlign="Center" />
        <SelectedItemStyle BackColor="#E2DED6" Font-Bold="True" ForeColor="#333333" />
    </asp:DataGrid>   
            

        </asp:View>
        <asp:View ID="vProfile" runat="server">
            <table class="profile" cellpadding="3" width="100%">
                <tr>
                    <td class="toolbar-top" colspan="2">
                        <asp:Button ID="btnOK1" runat="server" Text="Сохранить" OnClick="btnOK1_Click" />
                        <asp:Button ID="btnCancel1" runat="server" Text="Cancel" OnClick="btnCancel1_Click" />
                    </td>
                </tr>
                <tr>
                    <td class="profile-field-name">ID
                    </td>
                    <td class="profile-field">

                        <asp:TextBox ID="txID" runat="server" Width="80px" ReadOnly="True"></asp:TextBox>

                    </td>
                </tr>
                <tr>
                    <td class="profile-field-name bold">Название
                    </td>
                    <td class="profile-field">
                        <asp:TextBox ID="txName" runat="server" MaxLength="100" Width="80%"></asp:TextBox>
                    </td>
                </tr>




                <tr>
                    <td class="profile-field-name bold">Фактический Адрес
                    </td>
                    <td class="profile-field">
                        <asp:TextBox ID="txAddress" ClientIDMode="Static" runat="server" MaxLength="500" Rows="3"
                            Text="" TextMode="MultiLine" Width="100%"></asp:TextBox>
                    </td>
                </tr>
                <tr>
                    <td class="profile-field-name bold">Телефоны
                    </td>
                    <td class="profile-field">
                        <asp:TextBox ID="txPhones" ClientIDMode="Static" runat="server" MaxLength="250"
                            Text="" TextMode="SingleLine" Width="100%"></asp:TextBox>
                    </td>
                </tr>
                <tr>
                    <td class="profile-field-name bold">Режим работы 
                    </td>
                    <td class="profile-field">
                        <asp:TextBox ID="txWorktime" ClientIDMode="Static" runat="server" MaxLength="250"
                            Text="" TextMode="SingleLine" Width="100%"></asp:TextBox>
                    </td>
                </tr>
                <tr>
                    <td class="profile-field-name rightalign">
                        
                    </td>
                    <td class="profile-field"><asp:CheckBox ID="chShowIncash" runat="server" Text="Показывать абсолютную величину остатка на складе"/>
                        </td>
                </tr>
                <tr>
                    <td class="profile-field-name rightalign">
                        &nbsp;</td>
                    <td class="profile-field">
                        <asp:CheckBox ID="chAutoActivate" runat="server" Text="автоматически активировать нового пользователя" />
                    </td>
                </tr>
                <tr>
                    <td class="profile-field-name rightalign">
                        &nbsp;</td>
                    <td class="profile-field">
                        <asp:CheckBox ID="chUseSmsAuthorization" runat="server" Text="Режим авторизации через код по SMS по умолчанию для новых клиентов." />
                    </td>
                </tr>
                <tr>
                    <td class="profile-field-name rightalign">
                        &nbsp;</td>
                    <td class="profile-field">
                        <asp:CheckBox ID="chShowInvDiscount" runat="server" Text="Показывать скидку от базовой цены в заявке" />
                    </td>
                </tr>
                <tr>
                    <td class="profile-field-name bold">Email торгового агента по умолчанию </td>
                    <td class="profile-field">
                        <asp:TextBox ID="txDefaultTA" runat="server" ClientIDMode="Static" MaxLength="250" Text="" TextMode="SingleLine" Width="100%"></asp:TextBox>
                    </td>
                </tr>
                <tr>
                    <td class="profile-field-name bold">Описательная, статическая информация
                    </td>
                    <td>
                        <uc1:mediaBrowser ID="mediaBrowser1" runat="server" />
                        <asp:TextBox ID="txText" TextMode="MultiLine" CssClass="richtxt" ClientIDMode="Static" Rows="20" Height="300px"
                            runat="server" Width="100%" OnPreRender="txText_PreRender"></asp:TextBox>
                    </td>
                </tr>
                <tr>
                    
                    <td colspan="2">
                        <h6>Шапка счета на оплату</h6>
                        <uc1:mediaBrowser ID="mediaBrowser2" runat="server" />
                        <asp:TextBox ID="txInvoiceHead" TextMode="MultiLine" CssClass="richtxt" ClientIDMode="Static" Rows="20" Height="200px"
                            runat="server" Width="100%" OnPreRender="txInvoiceHead_PreRender"></asp:TextBox>
                        <h6>Подвал счета на оплату</h6>
                        <uc1:mediaBrowser ID="mediaBrowser3" runat="server" />
                        <asp:TextBox ID="txInvoiceFoot" TextMode="MultiLine" CssClass="richtxt" ClientIDMode="Static" Rows="20" Height="300px"
                            runat="server" Width="100%" OnPreRender="txInvoiceFoot_PreRender"></asp:TextBox>
                        <h6>Строка поставщик в счете на оплату</h6>
                        
                        <asp:TextBox ID="txInvoiceRekv" TextMode="MultiLine" ClientIDMode="Static" Rows="20" Height="300px"
                            runat="server" Width="100%" ></asp:TextBox>
                    </td>
                </tr>

                <tr>
                    <td class="profile-field-name">последние изменения:
                    </td>
                    <td class="profile-field">
                        <asp:Label ID="lbLastCorrect" runat="server" Text="..."></asp:Label>
                    </td>
                </tr>
            </table>

        </asp:View>
    </asp:MultiView>



    <div class="message">
        <asp:Literal ID="lbMessageimport" runat="server"></asp:Literal>
    </div>
</asp:Content>
