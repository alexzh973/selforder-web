<%@ Page Title="" Language="C#" MasterPageFile="common/mainpage.Master" AutoEventWireup="true"
    CodeBehind="nsigoodies.aspx.cs" Inherits="wstcp.nsigoodies" %>

<asp:Content ID="Content1" ContentPlaceHolderID="head" runat="server">
    <link href="tooltip.css" rel="stylesheet" type="text/css" />
    <script type="text/javascript">

        function setword(word) {
            $(".searchfield").val(word);
            $(".searchbtn").click();
        }
        function set_word(param, word) {
            $("input[ID*='searchparam']").val(param);

            $(".searchfield").val(word);
            if ($(".searchbtn"))
                $(".searchbtn").click();

            //$("#waitcursor").show();
            //$("#content1").html("<div style='padding:200px'><img src='../clock.gif'></div>");

        }

        var gid = 0;
        function set4detail(goodid, enscode, s_id) {
            gid = goodid;
            $("div[id*='goodinfodiv']").hide();

            if ($("#goodinfodiv" + goodid).length > 0)
                $("#goodinfodiv" + goodid).show();
            else {
                $.get("../good/gdetail.ashx",
                        {
                            act: "incash",
                            id: goodid,
                            sid: s_id
                        },
                        shw
                     );

                //$("<div id='goodinfodiv" + goodid + "'><iframe frameborder=0 src='../good/gdetail.aspx?v=small&id=" + goodid + "' width='400px' height='120px' /></div>").appendTo($("#td_" + goodid));
            }
        }

        function shw(data) {
            $("<div style='float:right;border:1px dotted #AAAAAA;'  id='goodinfodiv" + gid + "'>" + data + "</div><div style='clear:both;'></div>").appendTo($("#td_" + gid));
        }

        $(document).ready(function () {
            $("tr[id*='row_']").mouseover(function (e) { $(this).addClass('itemover'); });
            $("tr[id*='row_']").mouseout(function (e) { $(this).removeClass('itemover'); });
        });

    </script>
    <style type="text/css">
        .searchfield
        {
        }
        .itemover
        {
            background-color: #eef;
        }
    </style>
</asp:Content>
<asp:Content ID="Content3" ContentPlaceHolderID="place_Left" runat="server">
     <div class="border padding5">
        <div>
            <strong>В подразделении:</strong>
            <asp:DropDownList ID="dlOwners" runat="server" AutoPostBack="True" OnSelectedIndexChanged="dlOwners_SelectedIndexChanged"
                TabIndex="3">
            </asp:DropDownList>
        </div>
        <asp:RadioButtonList ID="rbQty" runat="server" AutoPostBack="True" CssClass="slow small"
            OnCheckedChanged="btnSearch_Click" RepeatDirection="Vertical" RepeatLayout="Flow"
            OnSelectedIndexChanged="chZn_SelectedIndexChanged">
            <asp:ListItem Value="0" Selected="True" Text="ВСЕ"></asp:ListItem>
            <asp:ListItem Value="1" Text="Только в наличии"></asp:ListItem>
            <asp:ListItem Value="2" Text="Только в наличии в выбранном подр"></asp:ListItem>
            <asp:ListItem Value="3" Text="Отсутств в выбр., но есть в других"></asp:ListItem>
        </asp:RadioButtonList>
        <br />
        <asp:CheckBoxList ID="chZn" runat="server" AutoPostBack="True" CssClass="slow small" OnCheckedChanged="btnSearch_Click"
            RepeatDirection="Vertical" RepeatLayout="Flow" OnSelectedIndexChanged="chZn_SelectedIndexChanged">
            <asp:ListItem Value="NL">НЕЛИКВИДЫ</asp:ListItem>
            <asp:ListItem Value="P2">ПЕРЕЗАПАС</asp:ListItem>
        </asp:CheckBoxList>
<br />
        <asp:LinkButton CssClass="slow" ID="lbtnMakeFile" runat="server" OnClick="lbtnMakeFile_Click">выгрузить файл</asp:LinkButton>
        <div>
            <asp:Literal ID="lbFile" runat="server"></asp:Literal></div>


    </div>
    <hr />
   
    <div class="small ">
        <strong>Товарное направление</strong><br />
        <asp:Literal ID="divCloud" runat="server" Visible="true"></asp:Literal>
    </div>
    <hr/>
    <div class="small">
    
    <asp:Repeater ID="Repeater1" runat="server">
                    <HeaderTemplate>
                        <strong>Данные обновлены:</strong>
                    </HeaderTemplate>
                    <ItemTemplate>
                       <div title='<%#Eval("lastrep") %>'> <%#Eval("Name") %>:&nbsp;<%#Eval("lastupd", "{0:U}")%></div>
                    </ItemTemplate>
                    <SeparatorTemplate>
                        --------------</SeparatorTemplate>
                </asp:Repeater>
    </div>
</asp:Content>
<asp:Content ID="Content2" ContentPlaceHolderID="ContentPlaceHolder1" runat="server">
    <div class="grayfon padding5  small">        
        <strong>Товарная категория:</strong>
        <asp:Literal ID="blockCategory" runat="server"></asp:Literal>
        <br /><strong>Бренд: </strong>
        <asp:DropDownList ID="dlBrend" runat="server" AutoPostBack="True" OnSelectedIndexChanged="dlBrend_SelectedIndexChanged">
        </asp:DropDownList>
        <br />
        <input type="hidden" id="schparam" value="" />
        <asp:HiddenField ID="searchparam" runat="server" />
        поиск:
        <asp:TextBox CssClass="searchfield" ID="txSearch" MaxLength="150" runat="server"
            Width="400px"></asp:TextBox>
        <asp:Button ID="btnSearch" CssClass="searchbtn waiting" runat="server" OnClick="btnSearch_Click"
            TabIndex="1" Text="найти" />
    </div>
   <h2>
            <asp:Literal ID="selCatBrend" runat="server"></asp:Literal></h2>
    
    <table width="100%" border="0">
       
        <tr>
            <td valign="top">
                <div id="content1">
                </div>
                <asp:DataGrid ID="dgGoodies" CssClass="grid small" runat="server" CellPadding="4"
                    AllowPaging="True" AutoGenerateColumns="False" OnPageIndexChanged="dgGoodies_PageIndexChanged" PagerStyle-Position="TopAndBottom"
                    PageSize="20" OnItemDataBound="dgGoodies_ItemDataBound" EnableViewState="true">
                    <Columns>
                        <asp:BoundColumn DataField="ID" HeaderText="ID" Visible="false"></asp:BoundColumn>
                        <asp:BoundColumn DataField="Article" HeaderText="Артикул" Visible="False"></asp:BoundColumn>
                        <asp:BoundColumn DataField="GoodCode" HeaderText="Код"></asp:BoundColumn>
                        <asp:BoundColumn DataField="Name" HeaderText="Наименование"></asp:BoundColumn>
                        <asp:BoundColumn DataField="Qty" HeaderText="Кол-во"></asp:BoundColumn>
                        <asp:BoundColumn DataField="qtyother" HeaderText="Кол-во *"></asp:BoundColumn>
                        <asp:TemplateColumn ></asp:TemplateColumn>
                        <asp:BoundColumn DataField="zn_z" HeaderText="Зн зпс"></asp:BoundColumn>
                        <asp:BoundColumn DataField="pr_spr" HeaderText="Супбн" DataFormatString="{0:F2}">
                        </asp:BoundColumn>
                        <asp:BoundColumn DataField="qtylcd" HeaderText="посл. изм."></asp:BoundColumn>
                        <asp:TemplateColumn></asp:TemplateColumn>
                    </Columns>
                    <FooterStyle CssClass="mainbackcolor white bold" />
                    <HeaderStyle CssClass="mainbackcolor white bold" />
                    <PagerStyle CssClass="mypager mainbackcolor" Mode="NumericPages" 
                        Position="TopAndBottom" />
                </asp:DataGrid>
                <div>
                    <strong>кол-во</strong> - это количество в выбранном подразделении</div>
                <div>
                    <strong>кол-во *</strong> - это количество в всех подразделениях исключая выбранное</div>
            </td>
            <td valign="top" width="1%">
                <div>
                </div>
                <div>
                    <!--<iframe id="ENS" name="ENS" width="100%" height="400px" frameborder="1" src="empty.htm">
                    </iframe>-->
                </div>
            </td>
        </tr>
        <tr>
            <td colspan="3" class="grayfon small">
                
                <asp:LinkButton ID="LinkButton1" runat="server" OnClick="LinkButton1_Click">_</asp:LinkButton>
                <asp:Button ID="Button1" runat="server" Text="Button" OnClick="Button1_Click" Visible="false" />
            </td>
        </tr>
    </table>
</asp:Content>
<asp:Content ID="Content4" ContentPlaceHolderID="place_Right" runat="server">
   
</asp:Content>
