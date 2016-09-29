<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="test1.aspx.cs" Inherits="wstcp.special.test1" %>

<!DOCTYPE html>

<html xmlns="http://www.w3.org/1999/xhtml">
<head runat="server">
<meta http-equiv="Content-Type" content="text/html; charset=utf-8"/>
    <title></title>
    <script src="../js/jquery.js"></script>
    <script type="text/javascript">
        $(document).ready(function () {
            $('#testbutton').click(function() {
                $('#load').load("../uc/ucDateinput.ascx");
            });
        });
    </script>
</head>
<body>
    <form id="form1" runat="server">
    <div>
        <input id="testbutton" type="button" value="testload"/>
        <div style="border:1px dotted red;" id="load"></div>
        <asp:Button ID="Button1" runat="server" OnClick="Button1_Click" Text="Button" />
    
        
    
    </div>
        <hr/>
        <asp:TextBox ID="TextBox1" runat="server" TextMode="MultiLine" Width="593px" Text="select * from Win32_LogicalDisk Where DeviceID='C'"></asp:TextBox>
        <asp:Button ID="Button2" runat="server" OnClick="Button2_Click" Text="ManagementObjSeacher" />
        <asp:GridView ID="GridView1" runat="server" >
        </asp:GridView>
        
        

<script src="../js/chosen.jquery.min.js"></script>

        <link href="../js/chosen.css" rel="stylesheet" type="text/css" />

<script type="text/javascript">

    $(function() {
        $('select').chosen({no_results_text:'Нет результатов по'});

    })

</script>

<select class="input" id="input04" data-placeholder="Укажите вариант">

<option value="name">По имени</option>

<option value="namesurname">По имени и отчеству</option>

</select>
    </form>
</body>
</html>
