<%@ Control Language="C#" AutoEventWireup="true" CodeBehind="ucOrdersTable.ascx.cs" Inherits="wstcp.order.ucOrdersTable" %>
<h3 runat="server" id="h3">
    <asp:Literal ID="lbTitleList" runat="server"></asp:Literal></h3>
<div id="list" runat="server">

<asp:DataGrid ID="dgList" runat="server" AutoGenerateColumns="False" CssClass="table table-striped table-tabs-default" CellPadding="6" OnItemDataBound="dgList_ItemDataBound" AllowPaging="True" OnPageIndexChanged="dgList_PageIndexChanged" ForeColor="#333333" GridLines="None" CellSpacing="-1" Width="100%" OnItemCreated="dgList_ItemCreated">
    <Columns>
         <asp:TemplateColumn Visible="false"><HeaderStyle Width="10px" /></asp:TemplateColumn>
        <asp:BoundColumn DataField="id" Visible="true" DataFormatString="№{0}" HeaderText="№/код">
            <ItemStyle Width="80px" />
         </asp:BoundColumn>
        <asp:BoundColumn DataField="RegDate" DataFormatString="{0:d}" HeaderText="Дата" >
            
            <ItemStyle Width="80px" />
            
         </asp:BoundColumn>
        <asp:BoundColumn DataField="Name" HeaderText="Заявка" >
            
        </asp:BoundColumn>
        <asp:BoundColumn DataField="SummOrder" ItemStyle-CssClass="left" DataFormatString="{0:n}р." HeaderText="Сумма">
            <ItemStyle Width="80px" CssClass="left"/>

         </asp:BoundColumn>
        <asp:TemplateColumn  HeaderText="Состояние">
            
<ItemStyle  Width="130px"></ItemStyle>
         </asp:TemplateColumn>
        <asp:TemplateColumn HeaderText="Возможные">
            <ItemStyle CssClass="icons-act" Width="80px" />

        </asp:TemplateColumn>
        
       
         <asp:TemplateColumn HeaderText="действия">
             <ItemStyle CssClass="icons-act" />
         </asp:TemplateColumn>
        
       
    </Columns>
    <EditItemStyle BackColor="#999999" />
    <FooterStyle BackColor="#4598c7" Font-Bold="True" ForeColor="White" />
    <HeaderStyle Font-Bold="True"  Font-Italic="False" Font-Overline="False" Font-Strikeout="False" Font-Underline="False" />
    <ItemStyle CssClass="selectablehover" BackColor="#F7F6F3" ForeColor="#333333" />
    <AlternatingItemStyle CssClass="selectablehover" BackColor="White" ForeColor="#284775" />
   
     <PagerStyle NextPageText=" следующие 10 &amp;gt;" Position="TopAndBottom" PrevPageText="&amp;lt; предыдущие 10 "  ForeColor="#003366" CssClass="mypager center" HorizontalAlign="Center" Mode="NumericPages" />
    <SelectedItemStyle BackColor="#E2DED6" Font-Bold="True" ForeColor="#333333" />
</asp:DataGrid>
    
    </div>