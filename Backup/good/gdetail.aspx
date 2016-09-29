<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="gdetail.aspx.cs" Inherits="wstcp.gdetail" %>

<!DOCTYPE html PUBLIC "-//W3C//DTD XHTML 1.0 Transitional//EN" "http://www.w3.org/TR/xhtml1/DTD/xhtml1-transitional.dtd">
<html xmlns="http://www.w3.org/1999/xhtml">
<head runat="server">
    <title></title>
    <link href="../custom.css" rel="stylesheet" type="text/css" />
</head>
<body>
    <form id="form1" runat="server">
    <div>
        <h2>
            <asp:Literal ID="lbSelectedGood" runat="server"></asp:Literal></h2>
        <asp:Literal ID="lbDetail" runat="server"></asp:Literal>
        <asp:DataGrid ID="dgIncash" runat="server" 
            OnItemDataBound="dgIncash_ItemDataBound" AutoGenerateColumns="False" 
            CellPadding="5">
            <Columns>
                <asp:BoundColumn DataField="OwnerName" HeaderText="Подразд"></asp:BoundColumn>
                <asp:BoundColumn DataField="zn" HeaderText="зн"></asp:BoundColumn>
                <asp:BoundColumn DataField="zn_z" HeaderText="зн. ост"></asp:BoundColumn>
                <asp:BoundColumn DataField="qty" HeaderText="кол-во" DataFormatString="{0:N0}"></asp:BoundColumn>
                <asp:BoundColumn DataField="suppricewnds" HeaderText="с с н" DataFormatString="{0:F2}"></asp:BoundColumn>
                <asp:BoundColumn DataField="lcd" HeaderText="обн"></asp:BoundColumn>
            </Columns>
            <HeaderStyle BackColor="#666699" Font-Bold="True" ForeColor="White" />
        </asp:DataGrid>
    </div>
    </form>
</body>
</html>
