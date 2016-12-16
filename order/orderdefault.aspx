<%@ Page Title="" Language="C#" MasterPageFile="~/common/mainpage.Master" AutoEventWireup="true"
    CodeBehind="orderdefault.aspx.cs" Inherits="wstcp._orderdefault" %>


<%@ Register Src="~/UC/TabControl.ascx" TagName="TabControl" TagPrefix="uc1" %>


<asp:Content ID="Content1" ContentPlaceHolderID="head" runat="server">

    <link href="../css/tooltip.css" rel="stylesheet" type="text/css" />
    <link href="../order/ord01.css" rel="stylesheet" type="text/css" />

    <style type="text/css">
        

        table.plusminus {
            line-height: 16px;
        }

        .plusminus {
            cursor: pointer;
            padding: 0;
            margin: 0;
            border: 0;
        }
    </style>


</asp:Content>

<asp:Content runat="server" ID="scr1" ContentPlaceHolderID="placeScripts">
    <script src="../order/ord01.js" type="text/javascript"></script>
    <script src="../good/good.js"></script>
    <script type="text/javascript">

        $(function () {
            $(".inputqty").keypress(function (event) {
                if (event.which == '13') {
                    event.preventDefault();
                    $(this).blur();
                }
            });
        });

        $(function () {
            $('select').chosen({ no_results_text: 'Нет результатов по' });

        });


        function showan(goodCode, ownerId, priceType) {
            showComplect(goodCode, priceType, ownerId, "komplgoodies");
            showSoput(goodCode, priceType, ownerId, "sopgoodies");
            showAnalog(goodCode, priceType, ownerId, "angoodies");
        }




        function plusq(goodId, qty) {

            var inpqty = $("#qch_" + goodId);
            inpqty.val(+inpqty.val() + +qty);
            changeQty(goodId, qty);
        }
        function minusq(goodId, qty) {
            var inpqty = $("#qch_" + goodId);
            var nq = myParseFloat(inpqty.val()) - qty;
            if (nq < 0) {
                inpqty.val("");

            } else {

                inpqty.val(nq);

            }
            changeQty(goodId, qty);
        }


        function checksaved() {
            var ret = false;
            if ($('.itemstack').val() != '' && $("span[id*='the_ord']").text().trim() == "") {
                ret = confirm('реально все очистить?');
            }
            else {
                ret = true;
                //alert($("span[id*='the_ord']").text().trim());
            }
            return ret;
        }

        $("label").bind("click", selchks);
        $("input[name='ch_tks']").bind("click", selchks);





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


<asp:Content ID="Content3" ContentPlaceHolderID="ContentPlaceHolder1" runat="server">

    <div class="floatLeft">
        <h2 onclick="showStruct()">Самостоятельный подбор заявки
            <asp:Literal ID="lbForSubj" runat="server"></asp:Literal></h2>
    </div>
    <div class="floatRight">
        <asp:LinkButton ID="lbtnGotoCart" Visible="false" runat="server" OnClick="lbtnGotoCart_Click">Перейти в заявку</asp:LinkButton>
    </div>




    <div class="message">
        <asp:Literal ID="lbReadonly" Visible="false" runat="server" Text="Выбранную заявку нельзя редактировать на сайте. Обратитесь к своему менеджеру."></asp:Literal>
    </div>
    <div id="msgcart" name="msgcart"></div>

    <table width="100%">
        <tr>
            <td style="padding: 5px; vertical-align: top;">
                <asp:HiddenField ClientIDMode="Static" ID="itemstack" runat="server" />
                <asp:HiddenField ClientIDMode="Static" ID="tkstack" runat="server" />

                <div class="message">
                    <asp:Literal ID="lbMessage" runat="server"></asp:Literal>
                </div>
                <uc1:TabControl ID="TabControl1" runat="server" />
                
                <div class="tabcontrol">
            
         <!--Источник списка номенклатуры-->         
                    <!--<div class="floatRight">-->
                    
                    
                        
                        <div class="col-md-12 col-sm-12 box-source-tmc">
                        <div id="pnlSourceSelect" runat="server" visible="true">
                            <span class="box-source-tmc-title">Источник списка номенклатуры:</span> 
        
                            <span class="btn-group" data-toggle="buttons">
                                
                            <asp:LinkButton ID="lnkJust" CommandArgument="just" runat="server" ClientIDMode="Static" CssClass="" OnClick="lnkSelectREG_Click">Весь каталог</asp:LinkButton>
                                
                                
                            <asp:LinkButton ID="lnkMyFavor" CssClass="" ClientIDMode="Static" OnClick="lnkSelectREG_Click" CommandArgument="my" runat="server">Из истории заказов</asp:LinkButton>
                            
                                
                            <asp:LinkButton CommandArgument="spec" CssClass="" ClientIDMode="Static" ID="lnkSpecial" OnClick="lnkSelectREG_Click" runat="server">Распродажа</asp:LinkButton>
                                
                                <!-- &nbsp;<a href="../good/sales.aspx" class="bold">Акции!!!</a> -->
                            </span>

                            <span class="box-source-tmc-stock"><asp:CheckBox ID="chIncash" ClientIDMode="Static" runat="server" Text="из наличия" AutoPostBack="True" CssClass="bold" OnCheckedChanged="chIncash_CheckedChanged" /></span>
                        </div>
                        </div>
                    
        <!--#Источник списка номенклатуры-->

                    <div class="clearBoth"></div>
                    <asp:Panel ID="pnlStruct" runat="server">
    
         <!--Режим выбора товарных категорий-->
                        <!--<div class="grayfon floatLeft">-->
                        
                            <div class="col-md-12 block-radio-tmc">
                           <div class="row">
                                 <div class="col-md-5">
                            <span class="block-radio-tmc-title">Выбора категорий</span>
                            <asp:RadioButtonList ID="rbRegselect" runat="server" OnSelectedIndexChanged="rbRegselect_SelectedIndexChanged" AutoPostBack="true" RepeatDirection="Horizontal" RepeatLayout="Flow">
                                <asp:ListItem Selected="True" Value="single">Одинарный</asp:ListItem>
                                <asp:ListItem Value="multy">Множественный</asp:ListItem>
                            </asp:RadioButtonList>
                            </div>
                            <div class="col-md-5">
                            <span class="block-radio-tmc-title">Список </span>
                                    <asp:RadioButtonList ID="rbVidcat" runat="server" OnSelectedIndexChanged="rbVidcat_SelectedIndexChanged" AutoPostBack="true" RepeatDirection="Horizontal" RepeatLayout="Flow">
                                        <asp:ListItem Selected="True" Value="min">Сворачиваемый</asp:ListItem>
                                        <asp:ListItem Value="max">Раскрытый</asp:ListItem>
                                    </asp:RadioButtonList>
                            </div>
                            <div class="col-md-2"><span class="block-radio-tmc-clean"><asp:LinkButton ID="lbtnClearsearchcat" runat="server" OnClick="lbtnClearsearchcat_Click">Очистить</asp:LinkButton></span></div>
                            </div></div>
                        
        <!--#Режим выбора товарных категорий-->

                      
                  


                </div>
                        <div class="clearBoth">
                        </div>

                        <div class="message">
                            <asp:Label ID="lbMessSource" runat="server" Text="Label"></asp:Label>
                        </div>
                        <asp:PlaceHolder ID="PlaceHolder1" runat="server" Visible="false">
                            <div class="bold">
                                найти номенклатуру 
                            <asp:TextBox ID="txSimSearch" MaxLength="50" runat="server" Width="400px"></asp:TextBox><asp:Button ID="btnSimSearch" runat="server" Text="найти" OnClick="btnSimSearch_Click" />
                            </div>
                            <div class="right">
                                найти товарную категорию
                            <input class="italic" maxlength="20" type="text" id='searchtks' onkeypress="findtks()" onchange="javas                    cript:return false;"
                                value='' />
                            </div>
                        </asp:PlaceHolder>
                        <div style="overflow: auto; height: 500px" class="tab-tmc-wrap" id="divPlaceStruct">
                            <asp:Literal ID="struct_place" runat="server"></asp:Literal>
                        </div>
                    </asp:Panel>

                    <asp:Panel ID="pnlList" runat="server">
                        <div class="message small">
                            <asp:Literal ID="lbListMessage" runat="server"></asp:Literal>
                        </div>

                        <%--<i><span class="red bold border"> * </span> - <span class="small">звездочка перед ценой означает заказную позицию и цена ориентировочная. Её нужно уточнить у своего менеджера. Двойной клик на цене - послать запрос по этой позиции.</span></i>--%>
                        
                        <!--Все товарные направления-->
                        <!--<div class="floatLeft big bold">
                            <asp:LinkButton ID="lbtnChangeTN" CssClass="italic micro bold" runat="server" OnClick="lbtnChangeTN_Click"><img src="../simg/16/arrow-left.png" alt="<=" title="К выбору Товарное направление/категории"/></asp:LinkButton>
                            <asp:Literal Visible="false" ID="lbSelTN" runat="server"></asp:Literal>

                        </div>-->
                        <!--#Все товарные направления-->

                        <!--<div class="floatLeft-10" id="catTitle">&nbsp;</div>-->
                        <div class="clearBoth">
                        </div>
                        
                        <!--Фильтр-->
                        <div class="block-filtr-tmc">
                        
                                            <div class="col-md-10">
                                            <div class="form-inline">
                                                <span class="filter-tmc-item">Бренд:
                            <asp:DropDownList ID="dlBrends" runat="server" Width="160px" AutoPostBack="true"
                                OnSelectedIndexChanged="dlBrends_SelectedIndexChanged" ClientIDMode="Static" data-placeholder="Выберите Бренд">
                            </asp:DropDownList></span>
                                            
                                            <span class="filter-tmc-item">Категория:
                            <asp:DropDownList ID="dlTK" runat="server" Width="200px" CssClass="small" AutoPostBack="true" OnSelectedIndexChanged="dlTK_SelectedIndexChanged" ClientIDMode="Static" data-placeholder="Выберите Категорию">
                            </asp:DropDownList></span>
                                            
                                            
                                            
                                            <span class="filter-tmc-item">Номенклатура:
                            <asp:DropDownList ID="dlNames" runat="server" OnSelectedIndexChanged="txSearch_TextChanged" data-placeholder="Выберите номенклатуру" Width="160px"
                                ClientIDMode="Static" AutoPostBack="True">
                            </asp:DropDownList> </span>
                                            </div>


                                            </div>
                                           
                                            <div class="col-md-2">
                                                <asp:LinkButton ID="LinkButton1" ClientIDMode="Static" CssClass=" clear-filter " runat="server" OnClick="btnClearFilter_Click">сбросить фильтр</asp:LinkButton>
                                        <asp:Button ID="Button1" CssClass="acptfltr" runat="server" OnClick="btnAcptFlt_Click" Text="применить фильтр" Visible="false" />

                                             </div>                               
                                        
                                        
                                        
                                            <div class="col-md-12 search-row"><span class="search-row-title">Поиск по коду, артикулу или части названия</span>

                                                <asp:TextBox ID="txSearch" runat="server" CssClass="search-input normalsize" MaxLength="150"></asp:TextBox><asp:Button Visible="true" ClientIDMode="Static" ID="btnSearchGood" runat="server" CssClass="btn btn-default btn-xs" Text="Найти" OnClick="txSearch_TextChanged" />
                                            </div>
                                
                            
                                      


                                   
                                
                              
                                   
                               

                                    <div class="clearBoth"></div>
                                
                            
                            </div>
                        <!--#фильтр-->
                       
                         <!-- Кол-во позиций на странице -->
                       
                        
                        <div class="col-md-12 page-count-tmc">
                        На странице
                        <asp:RadioButtonList ID="rbPageSize" runat="server" AutoPostBack="true"
                            RepeatDirection="Horizontal" RepeatLayout="Flow"
                            OnSelectedIndexChanged="rbPageSize_SelectedIndexChanged">
                            <asp:ListItem Text="15" Value="15" Selected></asp:ListItem>
                            <asp:ListItem Text="30" Value="30"></asp:ListItem>
                            <asp:ListItem Text="50" Value="50"></asp:ListItem>
                        </asp:RadioButtonList>
                        позиций 
                            </div>
                        <!-- #Кол-во позиций на странице -->

                        <!--Номенклатура-->
                        <div>
                        <asp:DataGrid ID="dgList" CssClass="grid" runat="server" AllowPaging="True" PageSize="15"
                            OnPageIndexChanged="dgList_PageIndexChanged" AutoGenerateColumns="False" CellPadding="4"
                            OnItemDataBound="dgList_ItemDataBound" Width="100%" AllowSorting="True" OnSortCommand="dgList_SortCommand">
                            <Columns>
                                <asp:BoundColumn DataField="id" Visible="False"></asp:BoundColumn>
                                <asp:TemplateColumn HeaderText="выбор" ItemStyle-Width="70px">
                                    <ItemStyle Width="70px" />
                                </asp:TemplateColumn>
                                <asp:TemplateColumn HeaderText="" ItemStyle-Width="20px">
                                    <ItemStyle Width="20px" Font-Bold="False" Font-Italic="False" Font-Overline="False" Font-Strikeout="False" Font-Underline="False" />
                                </asp:TemplateColumn>
                                <asp:BoundColumn DataField="goodcode" HeaderText="код 1С">
                                    <ItemStyle Font-Bold="False" Font-Italic="False" Font-Overline="False" Font-Size="8pt" Font-Strikeout="False" Font-Underline="False" />
                                </asp:BoundColumn>
                                <asp:BoundColumn DataField="name" HeaderText="наименование" SortExpression="name"></asp:BoundColumn>
                                <asp:TemplateColumn HeaderText="наличие"></asp:TemplateColumn>
                                <asp:TemplateColumn HeaderText="цена, (руб.)" SortExpression="price">
                                    <ItemStyle Font-Bold="False" Font-Italic="False" Font-Overline="False" Font-Strikeout="False" Font-Underline="False" HorizontalAlign="Right" />
                                </asp:TemplateColumn>
                                <asp:TemplateColumn HeaderText="">
                                    <ItemStyle Font-Bold="False" Font-Italic="False" Font-Overline="False" Font-Strikeout="False" Font-Underline="False" Wrap="False" />
                                </asp:TemplateColumn>
                            </Columns>
                            <HeaderStyle CssClass="mainbackcolor small bold white" />
                            <FooterStyle CssClass="mainbackcolor small bold white" />
                            <PagerStyle CssClass="bold mypager" HorizontalAlign="Center"
                                Font-Size="Large" Mode="NumericPages" Position="TopAndBottom" />
                            <SelectedItemStyle BackColor="#D1DDF1" Font-Bold="True" ForeColor="#333333" />
                        </asp:DataGrid>
                            </div>
                        <!--Номенклатура-->

                    </asp:Panel>
                    <asp:Panel ID="pnlOrder" runat="server">
                    </asp:Panel>

                
            </td>
        </tr>
    </table>
    
<script type="text/javascript">
    
    

    function getListTN() {
        var jl;
        $.ajax({
            url: '../good/ajaxg.ashx?sid=' + sid + '&act=getlisttn',
            success: function (data) {
                jl = JSON.parse(data);
                showCats(jl ); 
            }
        });
        
    }
    
    function showCats(listTn) {
        var str = "";
        listTn.forEach(function (tn) {
            str += "<div class='blocktn'>" +
                "<a href='orderdefault.aspx?tn=" + tn.ID + "' class='tnlink' title='Можно выбрать всё направление'  >" + tn.Title + "</a>";
            str += "<div class='blocktk' id='tkdiv" + tn.ID + "' ><div class='floatLeft-90'><ul>";
            var listTk = tn.Childs;
            listTk.forEach(function (tk) {
                str += "<li><a id='tk" + tk.ID + "' onclick=\"tkInStack(this.id,'" + tk.Title + "')\" data='" + tk.Title + "' href='orderdefault.aspx?tk=" + tk.ID + "' class='tklink' >" + tk.Title + "</a></li>";
                });
                str += "</ul></div><div class='floatRight-10'></div><div class='clearBoth'></div></div>";
                str += "</div>";
            }
        );

        
        $("#divPlaceStruct").html(str);
    }
    function getstringtk_s(id, title) {
        return "<input value=\"бойлеры\" name=\"ch_tks\" id=\"ch_tks_5\" onclick=\"tkInStack(this.id,'бойлеры')\" type=\"checkbox\"><label for=\"ch_tks_5\" class=\"uncheck\">Бойлеры</label>";
    }
    $(document).ready(function () {
       // getListTN();
    });


</script>
</asp:Content>
