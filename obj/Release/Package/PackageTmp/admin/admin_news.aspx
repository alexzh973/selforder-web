<%@ Page Title="" Language="C#" MasterPageFile="~/admin/admin.Master" AutoEventWireup="true"
    ValidateRequest="false" CodeBehind="admin_news.aspx.cs" Inherits="wstcp.admin_news" %>

<%@ Register Src="../UC/CommandPanel4Profile.ascx" TagName="CommandPanel4Profile"
    TagPrefix="uc2" %>
<%@ Register Src="../UC/CommentList.ascx" TagName="CommentList" TagPrefix="uc1" %>
<%@ Register Src="../UC/mediaBrowser.ascx" TagName="mediaBrowser" TagPrefix="uc4" %>
<%@ Register Src="../UC/BreadPath.ascx" TagName="BreadPath" TagPrefix="uc5" %>
<%@ Register Src="../UC/ucDateInput.ascx" TagName="ucDateInput" TagPrefix="uc6" %>
<asp:Content ID="Content1" ContentPlaceHolderID="head" runat="Server">
    <!-- TinyMCE -->
    <script src="../jscripts/tiny_mce/tiny_mce.js" type="text/javascript"></script>
    <script src="../jscripts/tiny_mce_init.js" type="text/javascript"></script>
    <style type="text/css">
        #txText_tbl
        {
            background-color: White;
        }
        #txText_tbl.p
        {
            padding: 3px 3px 5px 3px !important;
        }
    </style>
    <!-- /TinyMCE -->
</asp:Content>

<asp:Content ID="Content2" ContentPlaceHolderID="ContentPlaceHolder1" runat="Server">
    <asp:Label ID="lbMessage" runat="server" Text=""></asp:Label>
    <uc5:BreadPath ID="BreadPath1" runat="server" />
    <h2>
        
        <asp:Literal ID="lbTitle" runat="server"></asp:Literal>
    </h2>
 <asp:MultiView ID="MultiView1" runat="server">
     <asp:View ID="vList" runat="server">
         <div class="grayfon padding5">
             состояние <asp:DropDownList ID="dlState" runat="server" AutoPostBack="True" OnSelectedIndexChanged="btnSearch_Click">
                 <asp:ListItem Value="">Актуальные</asp:ListItem>
                 <asp:ListItem Value="A">Архивные</asp:ListItem>
                 <asp:ListItem Value="D">Удаленные</asp:ListItem>
             </asp:DropDownList><br/>
                    поиск
                    <asp:TextBox ID="txSearch" runat="server" CssClass="searchactive"
                        Width="200px" 
                        TabIndex="1"></asp:TextBox>
                    &nbsp;<asp:Button ID="btnSearch" runat="server" OnClick="btnSearch_Click" Text="найти"
                        TabIndex="2" /></div>
                     <asp:DataGrid ID="dgList" runat="server" AllowPaging="True" AutoGenerateColumns="False"
                OnItemDataBound="dgList_ItemDataBound" CellPadding="4" 
                ForeColor="#333333" GridLines="None" OnPageIndexChanged="dgList_PageIndexChanged">
                <AlternatingItemStyle BackColor="White" ForeColor="#284775" />
                <Columns>
                    <asp:BoundColumn DataField="ID" Visible="false">
                        <ItemStyle />
                    </asp:BoundColumn>
                    <asp:BoundColumn DataField="RegDate" ItemStyle-CssClass="small" HeaderText="Создание"></asp:BoundColumn>
                    <asp:BoundColumn DataField="EndDate"  DataFormatString="{0:d}"  HeaderText="Актуально до"></asp:BoundColumn>
                    <asp:TemplateColumn ItemStyle-Width="50%" HeaderText="Материал">
                        <ItemStyle Width="50%" />
                    </asp:TemplateColumn>
                    
                    <asp:TemplateColumn ItemStyle-Width="20px" HeaderText="Автор">
                        <ItemStyle Width="20px" />
                    </asp:TemplateColumn>
                    <asp:ButtonColumn CommandName="Select" Text="[изменить]"></asp:ButtonColumn>
                    
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
    <asp:PlaceHolder ID="blockShortAttributes" runat="server"></asp:PlaceHolder>
    <table class="profile" cellpadding="3" width="100%">
        <tr>
            <td class="profile-button" colspan="2">
                <uc2:CommandPanel4Profile ID="CommandPanel4Profile1" runat="server" />
            </td>
        </tr>
        <tr>
            <td class="profile-field-title">
                ID
            </td>
            <td class="profile-field">
                
                <asp:TextBox ID="txID" runat="server" Width="50px" ReadOnly="True"></asp:TextBox>&nbsp;&nbsp;<asp:CheckBox ID="chPublished" runat="server" Text="Опубликовано" TextAlign="Right" />&nbsp;&nbsp;|&nbsp;&nbsp;<asp:CheckBox
                    ID="chCommentPossible" runat="server" Text="можно комментировать" TextAlign="Right" />
                <asp:ImageButton ID="btnUnlock" ImageUrl="../simg/16/lock_o.png" runat="server" 
                    onclick="btnUnlock_Click" ToolTip="Разблокировать" />
            </td>
        </tr>
        <tr>
            <td class="profile-field-title bold">
                Заголовок
            </td>
            <td class="profile-field">
                <asp:TextBox ID="txName" runat="server" MaxLength="150" Width="80%"></asp:TextBox>
            </td>
        </tr>
        
        <tr>
            <td class="profile-field-title bold">
                Актуально до
            </td>
            <td class="profile-field">
                <uc6:ucDateInput ID="ucEndDate" runat="server" />
                <span class='micro'>(дата, до которой новость будет считаться актуальной, после этого
                    - в архив)</span>
            </td>
        </tr>
        <tr>
            <td class="profile-field-title bold">
                
            </td>
            <td class="profile-field">
                &nbsp;</td>
        </tr>
        <tr>
            <td class="profile-field-title">
                маленькая картинка (эмблема)<br />
                <asp:Image ID="imgPreview" runat="server" /><asp:ImageButton ID="ImageButton1" runat="server"
                    ImageUrl="~/SIMG/16/delete.png" OnClick="ImageButton1_Click" />
            </td>
            <td class="profile-field">
                <asp:FileUpload ID="FileUpload1" runat="server" />
                <asp:Button ID="btnLoadPic" runat="server" OnClick="btnLoadPic_Click" Text="загрузить" /><br />
                <asp:HiddenField ID="hdImgSmall" runat="server" />
            </td>
        </tr>
        <tr>
            <td class="profile-field-title bold">
                Анонс
            </td>
            <td class="profile-field">
                <asp:TextBox ID="txDescr" ClientIDMode="Static" runat="server" MaxLength="500" Rows="3"
                    Text="" TextMode="MultiLine" Width="100%"></asp:TextBox>
            </td>
        </tr>
        <tr>
            <td class="profile-field-title bold">
                Полный текст
                <div><a href="#" onclick="openflywin('../special/imgsel.aspx', 600, 500, 'выбор изображения')">выбрать картинку</a></div>
            </td>
            <td>
                <uc4:mediaBrowser ID="mediaBrowser1" runat="server" />
                <asp:TextBox ID="txText" TextMode="MultiLine" ClientIDMode="Static" Rows="20" Height="300px"
                    runat="server" Width="100%" OnPreRender="txText_PreRender"></asp:TextBox>
            </td>
        </tr>
        
        <tr>
            <td class="profile-field-title">
                доступность для клиентов подразделений
            </td>
            <td class="profile-field">
                <asp:CheckBoxList ID="chAccessOrg" runat="server" RepeatColumns="3">
                </asp:CheckBoxList>
            </td>
        </tr>
        <tr>
            <td class="profile-field-title">
                последние исзменения:
            </td>
            <td class="profile-field">
                <asp:Label ID="lbLastCorrect" runat="server" Text="..."></asp:Label>
            </td>
        </tr>
    </table></asp:View>
   </asp:MultiView>
</asp:Content>
