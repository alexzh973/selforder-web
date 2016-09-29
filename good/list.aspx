<%@ Page Title="" Language="C#" MasterPageFile="~/common/mainpage.Master" AutoEventWireup="true" ValidateRequest="false"
    CodeBehind="list.aspx.cs" Inherits="wstcp.goodies" %>


<%@ Register Src="~/UC/TabControl.ascx" TagName="TabControl" TagPrefix="uc1" %>
<%@ Register Src="~/UC/TabCtrl.ascx" TagPrefix="uc1" TagName="TabCtrl" %>
<%@ Register Src="~/UC/ucDateInput.ascx" TagPrefix="uc1" TagName="ucDateInput" %>




<asp:Content ID="Content1" ContentPlaceHolderID="head" runat="server">
    


    <link href="../css/tooltip.css" rel="stylesheet" />
    <link href="../good/goodlist.css" rel="stylesheet" />
    

    <script src="../good/good.js"></script>
    
    

    <script type="text/javascript">
        function showIncash(event, goodid) {
            $.ajax({
                url: '../good/gdetail.ashx?sid=' + sid + '&act=incashlist&id=' + goodid,
                success: function(data) {
                    showToolTip(event, data);
                }
            });
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

            }
        }

        function shw(data) {
            $("<span class='classic'>" + data + "</span>").appendTo($("#a_" + gid));
        }


        function showangood(goodCode) {
            $("#angoodies").text("");
            var ownerid = $("#dlOwners").val();
            var tcen = $("#dlTypePr").val();
            
            showComplect(goodCode, tcen, ownerid, "komplgoodies");
            showSoput(goodCode, tcen, ownerid, "sopgoodies");
            showAnalog(goodCode, tcen, ownerid, "angoodies");

            

        }

        function setisys( goodCode, placeId) {

            var tcen = $("#dlTypePr").val();
            $.get("../good/gdetail.ashx",
                {
                    sid: sid,
                    act: "isys",
                    code: goodCode
                    
                },
                function (data) {
                    if (data && data.length > 10) {
                        $("#" + placeId).show();
                        $("#" + placeId).html("<div><h5>Инженерные системы</h5>" + data + "</div>");
                         
                    } else {
                        $("#" + placeId).html("");
                        $("#" + placeId).hide();
                    }
                }
            );
        }

        function itemInOrder(act, goodId, qty, descr) {
           
            $.ajax({
                url: '../order/orderajax.ashx?act=' + act + '&gid=' + goodId + '&qty=' + qty + '&descr=' + descr + '&sid=' + sid,
                cache: false,
                success: function (data) {
                    if ($("#cartbut") != null) {
                        $("#cartbut").removeClass("hidden"); 
                    }
                    $(".cartbut").addClass("brighttab");

                    showOrderSumm();
                    setInterval(clearmsg, 1000);

                },
                error: function (jqxhr, status, errorMsg) {
                    $('#qmsg').text(errorMsg);
                    setInterval(clearmsg, 3000);
                }
            });
        }



    </script>
    
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

<asp:Content ID="Content2" ContentPlaceHolderID="place_Left" runat="server">
    <div class="ipad grayfon">
        <h4>Фильтр</h4>
        <div>
            <strong>В подразделении:</strong>
            <asp:DropDownList ID="dlOwners" ClientIDMode="Static" runat="server" AutoPostBack="True" OnSelectedIndexChanged="dlOwners_SelectedIndexChanged"
                TabIndex="3">
            </asp:DropDownList>
                 <hr/>
            <span class="bold">показывать тип цен</span>
            <asp:DropDownList  ClientIDMode="Static" ID="dlTypePr" runat="server" AutoPostBack="True" OnSelectedIndexChanged="dlTypePr_SelectedIndexChanged">
                <asp:ListItem Value="pr_b" Text="Базовая"></asp:ListItem>
                <asp:ListItem Value="pr_opt" Text="Оптовая"></asp:ListItem>
                <asp:ListItem Value="pr_kropt" Text="Кр оптовая"></asp:ListItem>
                <asp:ListItem Value="pr_spec" Text="Спец"></asp:ListItem>
                <asp:ListItem Value="pr_vip" Text="VIP"></asp:ListItem>
                <asp:ListItem Value="pr_ngc" Text="НГЦ"></asp:ListItem>
                <asp:ListItem Value="pr_spr" Text="Суппрайс с НДС"></asp:ListItem>

            </asp:DropDownList>
        </div>
             <hr/>
        <div>
            <asp:RadioButtonList ID="rbQty" runat="server" AutoPostBack="True" CssClass="slow small"
                OnCheckedChanged="btnSearch_Click" RepeatDirection="Vertical" RepeatLayout="Flow"
                OnSelectedIndexChanged="chZn_SelectedIndexChanged">
                <asp:ListItem Value="0" Selected="True" Text="ВСЕ"></asp:ListItem>
                <asp:ListItem Value="1" Text="Только в наличии"></asp:ListItem>
                <asp:ListItem Value="2" Text="Только в наличии в выбранном подр"></asp:ListItem>
                <asp:ListItem Value="3" Text="Отсутств в выбр., но есть в других"></asp:ListItem>
            </asp:RadioButtonList>
                 <hr/>
            <asp:CheckBoxList ID="chZn" runat="server" AutoPostBack="True" CssClass="slow small" OnCheckedChanged="btnSearch_Click"
                RepeatDirection="Vertical" RepeatLayout="Flow" OnSelectedIndexChanged="chZn_SelectedIndexChanged">
                <asp:ListItem Value="NL">НЕЛИКВИДЫ</asp:ListItem>
                <asp:ListItem Value="P2">ПЕРЕЗАПАС</asp:ListItem>
            </asp:CheckBoxList>
        </div>
        <hr/>
<div>
    на странице<br/>
                        <asp:RadioButtonList ID="rbPageSize" runat="server" AutoPostBack="true"
                            RepeatDirection="Horizontal" RepeatLayout="Flow"
                            OnSelectedIndexChanged="rbPageSize_SelectedIndexChanged">
                            <asp:ListItem Text="15" Value="15" Selected></asp:ListItem>
                            <asp:ListItem Text="30" Value="30"></asp:ListItem>
                            <asp:ListItem Text="50" Value="50"></asp:ListItem>
                        </asp:RadioButtonList>
                        позиций</div>

    </div>
    <asp:Repeater ID="rpAccs" runat="server">
        <HeaderTemplate><h4>Акции</h4></HeaderTemplate>
        <ItemTemplate>
            <div class="ipad"><a href="list.aspx?id=<%#Eval("ID") %>"><%#Eval("Name") %></a>
                <div>с <%#cDate.cToString(Eval("startdate")) %> по <%#cDate.cToString(Eval("Finishdate")) %></div>
                      </div>
        </ItemTemplate>
    </asp:Repeater>

     <div id="isys" ></div>
     <div id="komplgoodies" ></div>
    
    <div id="sopgoodies" ></div>

    <div id="angoodies" ></div>
    
   
</asp:Content>

<asp:Content ID="Content3" ContentPlaceHolderID="ContentPlaceHolder1" runat="server">
   
        <h2 >Справочник номенклатуры<asp:Label ID="lbTitle" runat="server" Text="Справочник номенклатуры"></asp:Label></h2>
        <asp:LinkButton CssClass="slow" ID="lbtnMakeFile" runat="server" OnClick="lbtnMakeFile_Click">выгрузить файл</asp:LinkButton>
        <div>
                            <asp:Literal ID="lbFile" runat="server"></asp:Literal>
                        </div>
   
   


    <div class="message">
        <asp:Literal ID="lbMessage" runat="server"></asp:Literal>
    </div>




    <table width="100%">
        <tr>
            <td style="padding: 5px; vertical-align: top;">
                <uc1:TabCtrl runat="server" ID="TabCtrl" OnSelectionChanged="TabChanged" />    
                <%--<uc1:TabControl ID="TabControl1" runat="server" />--%>
                <div style="padding: 5px;" class="tabcontrol">
                  
                
                    <asp:MultiView ID="MultiView1" runat="server">
                        <asp:View ID="vStruct" runat="server">
                            <asp:Panel ID="pnlStruct" runat="server">
                        <div class="bold">
                            найти номенклатуру 
                            <asp:TextBox ID="txSimSearch" MaxLength="50" runat="server" Width="400px"></asp:TextBox><asp:Button ID="btnSimSearch" runat="server" Text="найти" OnClick="btnSimSearch_Click" />
                        </div>
                            <asp:Menu ID="menuSys" runat="server" ItemWrap="True" Orientation="Horizontal" CssClass="menuSys" OnMenuItemClick="menuSys_MenuItemClick" Visible="False">
                                <Items>
                                    <asp:MenuItem ImageUrl="~/simg/sysVoda.png" Text="Водоснабжение" Value="Водоснабжение"></asp:MenuItem>
                                    <asp:MenuItem ImageUrl="~/simg/sysKanal.png" Text="Канализация" Value="Водоотведение"></asp:MenuItem>
                                    <asp:MenuItem ImageUrl="~/simg/sysSanit.png" Text="Сантехника" Value="Сантехника"></asp:MenuItem>
                                    <asp:MenuItem ImageUrl="~/simg/sysPozhar.png" Text="Пожарка" Value="Пожаротушение"></asp:MenuItem>
                                     <asp:MenuItem ImageUrl="~/simg/sysOtop.png" Text="Отопление" Value="Отопление"></asp:MenuItem>
                                     <asp:MenuItem ImageUrl="~/simg/sysItp.png" Text="Тепловые пункты" Value="Тепловые пункты"></asp:MenuItem>
                                    <asp:MenuItem ImageUrl="~/simg/sysInstr.png" Text="Инструмент" Value="Инструмент"></asp:MenuItem>
                                </Items>
                            </asp:Menu>
                        
                        <div class="right" style="display: none;">
                            найти товарную категорию
                            <input class="italic" maxlength="20" type="text" id='searchtks' onkeypress="findtks()" onchange="javascript:return false;"
                                value='' />
                        </div>


                        <asp:Literal ID="struct_place" runat="server"></asp:Literal>
                    </asp:Panel>
                        </asp:View>
                        <asp:View ID="vList" runat="server">    <asp:Panel ID="pnlList" runat="server">
                        <div class="message small">
                            <asp:Literal ID="lbListMessage" runat="server"></asp:Literal>
                        </div>

                        <%--<i><span class="red bold border"> * </span> - <span class="small">звездочка перед ценой означает заказную позицию и цена ориентировочная. Её нужно уточнить у своего менеджера. Двойной клик на цене - послать запрос по этой позиции.</span></i>--%>
                        
                        
                        <div class="big bold">
                            <asp:LinkButton ID="lbtnChangeTN" CssClass="italic micro bold" runat="server" OnClick="lbtnChangeTN_Click"><img src="../simg/16/arrow-left.png" alt="<=" title="К выбору Товарное направление/категории"/></asp:LinkButton>
                            <asp:Literal Visible="false" ID="lbSelTN" runat="server"></asp:Literal>

                        </div>
                        
                        <table width="100%" class="grayfon">
                            <tr>
                                <td>
                                        <div class="padding2 ">
                                            Бренд:
                            <asp:DropDownList ID="dlBrends" runat="server" Width="120px" AutoPostBack="true"
                                OnSelectedIndexChanged="dlBrends_SelectedIndexChanged" ClientIDMode="Static">
                            </asp:DropDownList>&nbsp;&nbsp;Категория:
                            <asp:DropDownList ID="dlTK" runat="server" Width="120px" AutoPostBack="true" OnSelectedIndexChanged="dlTK_SelectedIndexChanged" ClientIDMode="Static">
                            </asp:DropDownList>
                                            &nbsp;&nbsp;Номенклатура:
                            <asp:DropDownList ID="dlNames" runat="server" OnSelectedIndexChanged="txSearch_TextChanged"
                                Width="120px" ClientIDMode="Static" AutoPostBack="True">
                            </asp:DropDownList>
                                            
                                        </div>
                                        <div class="padding2">
                                            поиск по коду, артикулу или части названия
                            <asp:TextBox ID="txSearch" runat="server" Width="250px" MaxLength="150">&nbsp;</asp:TextBox><asp:Button Visible="true" ClientIDMode="Static" ID="btnSearchGood" runat="server" CssClass="f-bu" Text="найти" OnClick="txSearch_TextChanged" />
                            <asp:LinkButton ID="btnClearFilter" ClientIDMode="Static" CssClass="small clear-filter " runat="server" OnClick="btnClearFilter_Click">сбросить фильтр</asp:LinkButton>  

                                        </div>
                                    
                                </td>
                                
                            </tr>
                        </table>

                        
                        <asp:DataGrid ID="dgList" CssClass="grid" runat="server" AllowPaging="True" PageSize="15"
                            OnPageIndexChanged="dgList_PageIndexChanged" AutoGenerateColumns="False" CellPadding="4"
                            OnItemDataBound="dgList_ItemDataBound" Width="100%">

                            <Columns>
                                <asp:BoundColumn DataField="ID" HeaderText="ID" Visible="false"></asp:BoundColumn>
                                <asp:BoundColumn DataField="Article" HeaderText="Артикул" Visible="False"></asp:BoundColumn>
                                <asp:BoundColumn DataField="GoodCode" HeaderText="Код"></asp:BoundColumn>
                                <asp:BoundColumn DataField="Name" HeaderText="Наименование"></asp:BoundColumn>
                                <asp:BoundColumn DataField="Qty" HeaderText="Кол-во"></asp:BoundColumn>
                                <asp:BoundColumn DataField="qtyother" HeaderText="Кол-во *"></asp:BoundColumn>
                                <asp:TemplateColumn></asp:TemplateColumn>
                                <asp:BoundColumn DataField="zn_z" HeaderText="Зн зпс"></asp:BoundColumn>
                                <asp:BoundColumn DataField="pr_spr" HeaderText="Цена, руб" DataFormatString="{0:F2}"></asp:BoundColumn>
                                <asp:BoundColumn DataField="lcd" HeaderText="посл. изм." Visible="False"></asp:BoundColumn>
                                <asp:TemplateColumn></asp:TemplateColumn>
                            </Columns>

                            
                            <HeaderStyle CssClass="mainbackcolor small bold white" />
                            <FooterStyle CssClass="mainbackcolor small bold white" />
                            <PagerStyle CssClass="bold mypager" HorizontalAlign="Center"
                                Font-Size="Large" Mode="NumericPages" Position="TopAndBottom" />
                            <SelectedItemStyle BackColor="#D1DDF1" Font-Bold="True" ForeColor="#333333" />
                        </asp:DataGrid>

                    </asp:Panel></asp:View>
                        <asp:View ID="vAcc" runat="server">
                            
                            <%--<p>ID акции <asp:TextBox ID="txIDacc" ReadOnly="True" runat="server"></asp:TextBox>    </p>--%>
                            <div class="floatLeft">
                            <p><strong>Название акции</strong><br/>
                            <asp:TextBox ID="txNameAcc" runat="server" Width="400px"></asp:TextBox>    </p>
                            </div>
                            <div class="floatRight">
                            <p><strong>Период действия</strong> <br/>
                                с <uc1:ucDateInput runat="server" ID="StartDate" /> по 
                                <uc1:ucDateInput runat="server" ID="FinishDate" />
                            </p>
                                </div>
                            <div class="clearBoth"></div>
                            <p>
                                <strong>Подробное описание</strong><br/>
                                <asp:TextBox ID="txText" ClientIDMode="Static" runat="server" Width="100%" Rows="4" TextMode="MultiLine"></asp:TextBox>
                                </p>
                            <p><strong>Баннер акции (max 100x100 px)</strong><br/>
                                <asp:FileUpload ID="ufBanner" runat="server" />
                             </p>
                            <h4>Состав акции</h4>
                            <asp:DataGrid ID="dgOrder" runat="server" AutoGenerateColumns="False" CellPadding="4"
                                    CssClass="grid" OnItemDataBound="dgAcc_ItemDataBound" ShowFooter="True" OnDeleteCommand="dgAcc_DeleteCommand"
                                    Width="100%">
                                    <Columns>
                                        <asp:BoundColumn DataField="goodid" Visible="False">
                                            <HeaderStyle Width="16px" />
                                        </asp:BoundColumn>
                                        <asp:ButtonColumn CommandName="delete" DataTextField="" Text="&lt;img title='удалить' src='../simg/16/delete.png'&gt;"></asp:ButtonColumn>
                                        <asp:BoundColumn DataField="goodcode" Visible="true" HeaderText="код">
                                            <ItemStyle Font-Bold="False" Font-Italic="False" Font-Overline="False" Font-Size="X-Small" Font-Strikeout="False" Font-Underline="False" />
                                        </asp:BoundColumn>
                                        <asp:BoundColumn DataField="nameGood" HeaderText="Наименование"></asp:BoundColumn>
                                        <asp:BoundColumn DataField="price" HeaderText="цена" DataFormatString="{0:N}">
                                            <HeaderStyle Width="60px" />
                                            <ItemStyle Font-Bold="False" Font-Italic="False" Font-Overline="False" Font-Strikeout="False"
                                                Font-Underline="False" HorizontalAlign="Right" />
                                        </asp:BoundColumn>
                                        <asp:BoundColumn DataField="qtyincash" HeaderText="кол-во">
                                            <HeaderStyle Width="60px" />
                                        </asp:BoundColumn>
                                        <asp:TemplateColumn HeaderText=""></asp:TemplateColumn>
                                        
                                        
                                    </Columns>
                                    <FooterStyle CssClass="mainbackcolor bold white" />
                                    <HeaderStyle CssClass="mainbackcolor bold white" />
                                    <PagerStyle CssClass="mypager" HorizontalAlign="Center" />
                                </asp:DataGrid>
                            <asp:Button ID="btnSaveAcc" runat="server" Text="Сохранить" OnClick="btnSaveAcc_Click" />
                        </asp:View>
                    </asp:MultiView>
                    <div class="message">
                        <asp:Literal ID="lbMess" runat="server"></asp:Literal>
                    </div>
                    <asp:HiddenField ClientIDMode="Static" ID="refresh" runat="server" />











                </div>
            </td>
        </tr>
    </table>


    <script type="text/javascript">
        function addToAcc(accId, goodId) {
            
            $.ajax({
                url: '../good/acc.ashx?sid=' + sid + '&act=add&accid='+accId+'&gid=' + goodId,
                success: function (data) { $("#kn_" + goodId).attr("src", "../simg/minus.png"); $("#kn_" + goodId).attr("onclick", "removeFromAcc('"+accId+"','"+goodId+"')"); }
            });
        }

        function removeFromAcc(accId,goodId) {
            $.ajax({
                url: '../good/acc.ashx?sid=' + sid + '&act=rem&accid='+accId+'&gid=' + goodId,
                success: function (data) { $('#kn_' + goodId).attr('src', '../simg/plus.png'); $("#kn_" + goodId).attr("onclick", "addToAcc('" + accId + "','" + goodId + "')"); }
            });
        }
    </script>



            <script type="text/javascript">


                $("label").bind("click", selchks);
                $("input[name='ch_tks']").bind("click", selchks);

                $(".microimg").hide();

                $(".microimg").bind({
                    "mouseout": function () {
                        $(".microimg").hide();
                    }
                });


                $(".eye").mouseover(function (e) {
                    $(".microimg").hide();

                    var img = $("#img" + $(this).attr("id"));

                    if (img.css("display") != "none") { img.hide(); return; }
                    var offset = $(this).offset();
                    var relX = (e.pageX - offset.left);
                    var relY = (e.pageY - offset.top);

                    //alert(e.pageY + ' : ' + offset.top);
                    img.css("top", offset.top - 70);
                    img.css("left", offset.left - 70);

                    img.show('fast');
                    img.addClass("border");
                    img.addClass("shadow");
                });



                function selchks() {

                    $("input[name='ch_tks']").each(function () {
                        var a = '#' + this.id;
                        if ($(a).is(":checked")) {
                            $("label[for='" + this.id + "']").removeClass('uncheck');
                            $("label[for='" + this.id + "']").addClass('check');

                        }
                        else {
                            $("label[for='" + this.id + "']").removeClass('check');
                            $("label[for='" + this.id + "']").addClass('uncheck');

                        }
                    });


                }

            </script>
  
</asp:Content>
