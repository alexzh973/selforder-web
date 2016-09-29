<%@ Control Language="C#" AutoEventWireup="true" CodeBehind="CommentList.ascx.cs" Inherits="wstcp.CommentList" %>

<asp:PlaceHolder ID="PlaceHolder1" runat="server">
<h3>Комментарии&nbsp; 
    <span class="bold">(<asp:Label ID="lbQty" runat="server" Text=""></asp:Label>)</span> <asp:PlaceHolder ID="plknSH" runat="server">
    <span class="link small" id="knSH"  onclick="javascript: if ($('#knSH').html()=='показать') {$('#knSH').html('скрыть');$('#commentlist').show(); }else{$('#knSH').html('показать');$('#commentlist').hide();}">показать</span></asp:PlaceHolder>
<asp:LinkButton ID="btnAddComment" runat="server" CssClass="small" onclick="btnAddComment_Click"><img src='../simg/16/comment_add.png' alt='[+]' title='добавить комментарий' > добавить</asp:LinkButton>
    
    </h3>
    <div id='commentlist'>
<asp:Repeater ID="Repeater1" runat="server">
<HeaderTemplate>


</HeaderTemplate>
<ItemTemplate>
<div class="comment">
    <div class="micro bold"><%#Eval("Regdate","{0:d}") %> &nbsp;<a title="" onclick="openflywin('../admin/card.aspx?id=<%#Eval("AuthorID") %>',500,500,'Пользователь')" href="#"><%#Eval("AuthorName") %></a>:</div>
    <%#Eval("Descr") %>
</div>
</ItemTemplate>
<FooterTemplate>
    

</FooterTemplate>
</asp:Repeater>
<script type="text/javascript">
    //$("#commentlist").hide();
</script>
</div>

    <asp:Panel ID="pnlAddComment" runat="server">
       <table class="pro_file" width="100%" >
       <tr><td>добавить комментарий
           </td></tr>
       <tr><td><asp:HiddenField ID="hdObjectType" runat="server" />
               <asp:HiddenField ID="hdObjectID" runat="server" />
               <asp:TextBox ID="txComment" runat="server" Rows="3" TextMode="MultiLine" Width="100%" ></asp:TextBox>
       </td></tr>
       <tr><td>
           <asp:Button ID="Button1" runat="server" Text="ОК" Width="80px" onclick="Button1_Click" 
               /></td></tr>
       </table>       

   </asp:Panel>
  
</asp:PlaceHolder>