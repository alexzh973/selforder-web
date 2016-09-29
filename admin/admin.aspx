<%@ Page Title="" Language="C#" MasterPageFile="~/admin/admin.Master" AutoEventWireup="true" CodeBehind="admin.aspx.cs" Inherits="wstcp.admin"  %>
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
	
    <div>
	<h3>проверка почты</h3>
	<p>mailserver<br/> <asp:TextBox ID="txMailServer" Width="200px" MaxLength="150" runat="server"></asp:TextBox></p>
	<p>to<br/> <asp:TextBox ID="txMailTo" Width="200px" MaxLength="150" runat="server"></asp:TextBox></p>
	<p>subj<br/> <asp:TextBox ID="txMailSubj" Width="200px" MaxLength="150" runat="server"></asp:TextBox></p>
	<p>body<br/> <asp:TextBox ID="txMailBody" Width="200px" MaxLength="150" TextMode="MultiLine" runat="server"></asp:TextBox></p>
	<p>from<br/> <asp:TextBox ID="txMailFrom" Width="200px" MaxLength="150" runat="server"></asp:TextBox></p>
	<p>
		<asp:Button ID="btnMail" runat="server" Text="send" onclick="btnMail_Click" /></p>
		<p>
			<asp:Literal ID="lbMailMess" runat="server"></asp:Literal></p>
	</div>
	<hr/>
	<div class="floatLeft">
	<h2>список предприятий</h2>
	<asp:LinkButton ID="btnNewOwner" runat="server" onclick="btnNewOwner_Click">новый</asp:LinkButton>
	<asp:DataGrid ID="dgOwners" runat="server" AutoGenerateColumns="False" 
		CellPadding="4" ForeColor="#333333" GridLines="None" 
		oncancelcommand="dgOwners_CancelCommand"         
		onitemcommand="dgOwners_ItemCommand" 
		onupdatecommand="dgOwners_UpdateCommand" 
	oneditcommand="dgOwners_EditCommand">
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
</div>
<div class="floatLeft">
<h2>список хранилищ</h2>
	<asp:DropDownList ID="dlOwners" runat="server" AutoPostBack="True" 
	onselectedindexchanged="dlOwners_SelectedIndexChanged">
	</asp:DropDownList>
	<asp:LinkButton ID="btnNewWH" runat="server" onclick="btnNewWH_Click">новый</asp:LinkButton>
	<asp:DataGrid ID="dgWarehauses" runat="server" CellPadding="4" 
		ForeColor="#333333" GridLines="None" AutoGenerateColumns="False" 
	oncancelcommand="dgWarehauses_CancelCommand" 
	oneditcommand="dgWarehauses_EditCommand" 
	onupdatecommand="dgWarehauses_UpdateCommand">
		<AlternatingItemStyle BackColor="White" ForeColor="#284775" />
		<Columns>
			<asp:BoundColumn DataField="ID" ReadOnly="True" HeaderText="ID"></asp:BoundColumn>
			<asp:BoundColumn DataField="Name" HeaderText="Название"></asp:BoundColumn>
			<asp:BoundColumn DataField="ContactInfo" HeaderText="Контактная информация"></asp:BoundColumn>
			<asp:BoundColumn DataField="OwnerCodes" HeaderText="Код"></asp:BoundColumn>
			<asp:EditCommandColumn CancelText="отмена" EditText="..." 
				UpdateText="сохранить" ButtonType="PushButton"></asp:EditCommandColumn>
		</Columns>
		<EditItemStyle BackColor="#999999" />
		<FooterStyle BackColor="#5D7B9D" Font-Bold="True" ForeColor="White" />
		<HeaderStyle BackColor="#5D7B9D" Font-Bold="True" ForeColor="White" />
		<ItemStyle BackColor="#F7F6F3" ForeColor="#333333" />
		<PagerStyle BackColor="#284775" ForeColor="White" HorizontalAlign="Center" />
		<SelectedItemStyle BackColor="#E2DED6" Font-Bold="True" ForeColor="#333333" />
	</asp:DataGrid>
 </div>  
 <div class="clearBoth"></div>
	<hr/>

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
		runat="server" Visible="False" /><asp:Button
		   ID="btnFullImport" runat="server" Text="запустить полный импорт" OnClick="full_import" />


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

