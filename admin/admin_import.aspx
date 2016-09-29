<%@ Page Title="" Language="C#" MasterPageFile="~/admin/admin.Master" AutoEventWireup="true" CodeBehind="admin_import.aspx.cs" Inherits="wstcp.admin_import"  %>
<asp:Content ID="Content1" ContentPlaceHolderID="head" runat="server">
    <script type="text/javascript">
	$(document).ready(function () {
		setInterval(check_import, 1000);   
	});

	function check_import() {
		$.ajax({
			url: '../import.ashx?act=fi',
			success: function (data) {
				if ($("#blimportinfo1")) {
					$('#blimportinfo1').text(data);                    
				}
			}
		});
		$.ajax({
		    url: '../import.ashx?act=in',
		    success: function (data) {
		        if ($("#blimportinfo2")) {
		            $('#blimportinfo2').text(data);
		        }
		    }
		});
	}
	
	</script>
</asp:Content>

<asp:Content ID="Content2" ContentPlaceHolderID="ContentPlaceHolder1" runat="server">
    
    <div id="blimportinfo1"></div>
<div id="blimportinfo2"></div>

	

	<asp:PlaceHolder ID="placeContent" Visible="false" runat="server"></asp:PlaceHolder>
    <asp:LinkButton ID="lbtnImpTas" runat="server" OnClick="lbtnImpTas_Click">импорт ТА</asp:LinkButton> | 


	<asp:LinkButton ID="btnImportAnalog" runat="server" OnClick="btnImportAnalog_Click">импорт аналогов</asp:LinkButton> | 


	<asp:LinkButton ID="btnUpdateInvoices" runat="server" OnClick="btnUpdateInvoices_Click">импорт счетов на оплату</asp:LinkButton> | <asp:LinkButton ID="btnUpdateTNTK" runat="server" OnClick="btnUpdateTNTK_Click">апдейт TN и TK</asp:LinkButton>
    <div>

	<asp:Label ID="lbMess" runat="server" Text=""></asp:Label>
        </div>

	<h2>История импорта</h2>
	<asp:DropDownList ID="dlTypeELG" runat="server" AutoPostBack="True" 
		onselectedindexchanged="dlTypeELG_SelectedIndexChanged">
		<asp:ListItem>import</asp:ListItem>
		<asp:ListItem>fullimport</asp:ListItem>
	</asp:DropDownList>
	<asp:DataGrid ID="dgELG" runat="server" AllowPaging="True" CellPadding="4" Width="98%"
		ForeColor="#333333" GridLines="None" 
		onpageindexchanged="dgELG_PageIndexChanged">
		<AlternatingItemStyle BackColor="White" ForeColor="#284775" />
		<EditItemStyle BackColor="#999999" />
		<FooterStyle BackColor="#5D7B9D" Font-Bold="True" ForeColor="White" />
		<HeaderStyle BackColor="#5D7B9D" Font-Bold="True" ForeColor="White" />
		<ItemStyle BackColor="#F7F6F3" ForeColor="#333333" />
		<PagerStyle BackColor="#284775" ForeColor="White" HorizontalAlign="Center" 
			Position="TopAndBottom" Mode="NumericPages" />
		<SelectedItemStyle BackColor="#E2DED6" Font-Bold="True" ForeColor="#333333" />
	</asp:DataGrid>
	<hr />
	<h2>Сервисные функции</h2>
	<asp:Button ID="brnStopImport" runat="server" onclick="brnStopImport_Click" 
		Text="стоп импорт" />&nbsp;
		<asp:Button ID="btnStartWatch" runat="server" Text="запустить импорт" 
		onclick="btnStartWatch_Click" />
	
		<asp:Button
		   ID="btnFullImport" runat="server" Text="запустить полный импорт" OnClick="full_import" />


		<hr/>
	<asp:Button ID="btnCheckDoubles" runat="server" 
		Text="Проверить наличие дубликатов" onclick="btnCheckDoubles_Click" />
	<asp:Button ID="btnClearDoubles" runat="server" Text="очистка дубликатов" 
		onclick="btnClearDoubles_Click" />
	<asp:DataGrid ID="dgDbl1" runat="server" AllowPaging="True" CellPadding="4" 
		ForeColor="#333333" GridLines="None">
		<AlternatingItemStyle BackColor="White" />
		<EditItemStyle BackColor="#2461BF" />
		<FooterStyle BackColor="#507CD1" Font-Bold="True" ForeColor="White" />
		<HeaderStyle BackColor="#507CD1" Font-Bold="True" ForeColor="White" />
		<ItemStyle BackColor="#EFF3FB" />
		<PagerStyle BackColor="#2461BF" ForeColor="White" HorizontalAlign="Center" 
			Mode="NumericPages" />
		<SelectedItemStyle BackColor="#D1DDF1" Font-Bold="True" ForeColor="#333333" />
	</asp:DataGrid>
	<asp:DataGrid ID="dgDbl2" runat="server" AllowPaging="True" CellPadding="4" 
		ForeColor="#333333" GridLines="None">
		<AlternatingItemStyle BackColor="White" />
		<EditItemStyle BackColor="#2461BF" />
		<FooterStyle BackColor="#507CD1" Font-Bold="True" ForeColor="White" />
		<HeaderStyle BackColor="#507CD1" Font-Bold="True" ForeColor="White" />
		<ItemStyle BackColor="#EFF3FB" />
		<PagerStyle BackColor="#2461BF" ForeColor="White" HorizontalAlign="Center" 
			Mode="NumericPages" />
		<SelectedItemStyle BackColor="#D1DDF1" Font-Bold="True" ForeColor="#333333" />
	</asp:DataGrid>
	<asp:DataGrid ID="dgDbl3" runat="server" AllowPaging="True" CellPadding="4" 
		ForeColor="#333333" GridLines="None">
		<AlternatingItemStyle BackColor="White" />
		<EditItemStyle BackColor="#2461BF" />
		<FooterStyle BackColor="#507CD1" Font-Bold="True" ForeColor="White" />
		<HeaderStyle BackColor="#507CD1" Font-Bold="True" ForeColor="White" />
		<ItemStyle BackColor="#EFF3FB" />
		<PagerStyle BackColor="#2461BF" ForeColor="White" HorizontalAlign="Center" 
			Mode="NumericPages" />
		<SelectedItemStyle BackColor="#D1DDF1" Font-Bold="True" ForeColor="#333333" />
	</asp:DataGrid>


		<hr/>
	   импорт справочника номенклатуры <asp:FileUpload ID="fileFullImport" 
		runat="server" Visible="False" />


	<div class="message">
		   <asp:Literal ID="lbMessageimport" runat="server"></asp:Literal></div>
	<asp:DataGrid ID="dgImpNow" runat="server">
	</asp:DataGrid>



		   <hr/>
		   проверка файла 
		<asp:TextBox ID="txNamefiletest" runat="server"></asp:TextBox><asp:Button ID="btnTestfile"
			runat="server" Text="file" OnClick="testFile" />
		<asp:Literal ID="lbTest" runat="server"></asp:Literal><asp:Button ID="btnTestDigi" runat="server"
			Text="Digi"  OnClick="testDigi"/>

	<hr/>
	<asp:Button ID="loadPictures" runat="server" Text="загрузить картинки на распродажу неликвидов" OnClick="loadPictures_Click" />
    <asp:TextBox ID="txQtyImgs" runat="server"></asp:TextBox>
    <asp:Button ID="Button1" runat="server" OnClick="Button1_Click" Text="прописать img" />
    <asp:Button ID="btnSetAllViewArticle" runat="server" Text="Button" OnClick="btnSetAllViewArticle_Click" />
</asp:Content>

