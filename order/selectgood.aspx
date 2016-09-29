<%@ Page Title="" Language="C#" MasterPageFile="~/common/mainpage.Master" AutoEventWireup="true"
    CodeBehind="selectgood.aspx.cs" Inherits="wstcp.selectgood" %>




<%@ Register Src="../UC/TabControl.ascx" TagName="TabControl" TagPrefix="uc1" %>




<asp:Content ID="Content1" ContentPlaceHolderID="head" runat="server">

    <link href="../css/tooltip.css" rel="stylesheet" type="text/css" />
    <link href="../order/ord.css" rel="stylesheet" type="text/css" />
    <script src="../order/ord.js" type="text/javascript"></script>
    <style type="text/css">
        .printSelected div {
            display: none;
        }

            .printSelected div.printSelection {
                display: block;
            }

                .printSelected div.printSelection div {
                    display: block;
                }

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
    <script src="../good/glist.js"></script>
    <script type="text/javascript">

        $(function () {
            $(".inputqty").keypress(function (event) {
                if (event.which == '13') {
                    event.preventDefault();
                    $(this).blur();
                }
            });
        });


        function showStruct() {
            $("#structWindow").hide();
            return;
            if ($("#structWindow").css("display") == "none")
                $("#structWindow").show("slow");
            else {
                $("#structWindow").hide();
            }
        }

        function showan(goodCode, ownerId, priceType) {
            showComplect(goodCode, priceType, ownerId, "komplgoodies");
            showSoput(goodCode, priceType, ownerId, "sopgoodies");
            showAnalog(goodCode, priceType, ownerId, "angoodies");
        }


        function plusq(goodId) {
            var inpqty = $("#qch_" + goodId);
            inpqty.val("" + (myParseFloat(inpqty.val()) + 1));
            changeQty(goodId);
        }
        function minusq(goodId) {
            var inpqty = $("#qch_" + goodId);
            var nq = myParseFloat(inpqty.val()) - 1;
            if (nq < 0) {
                inpqty.val("");

            } else {
                inpqty.val("" + nq);

            }
            changeQty(goodId);
        }
        function plusqo(goodId) {
            var inpqty = $("#q_" + goodId);
            inpqty.val("" + (myParseFloat(inpqty.val()) + 1));
            recount(goodId);
        }
        function minusqo(goodId) {
            var inpqty = $("#q_" + goodId);
            var nq = myParseFloat(inpqty.val()) - 1;
            if (nq < 0) {
                inpqty.val("");

            } else {
                inpqty.val("" + nq);

            }
            recount(goodId);
        }


        $(document).ready(function () {

            $("#showHideCatalog").click(function () {
                if ($("#mCat").is(":hidden")) {

                    $("#mCat").show("slow");

                } else {

                    $("#mCat").hide("slow");

                }
                return false;
            });
        });



    </script>

</asp:Content>

<asp:Content ID="Content3" ContentPlaceHolderID="ContentPlaceHolder1" runat="server">

    <div class="floatLeft">
        <h2 onclick="showStruct()">Самостоятельный подбор заявки
            <asp:Literal ID="lbForSubj" runat="server"></asp:Literal></h2>
    </div>
    <div class="floatRight">
        <asp:LinkButton ID="lbtnGotoCart" CssClass="ipad button" runat="server" OnClick="lbtnGotoCart_Click">Перейти в заявку</asp:LinkButton>

    </div>
    <div class="floatRight">
        <div id="pnlSourceSelect" runat="server" visible="true" style="padding-bottom: 10px">
            <strong>Источник списка номенклатуры </strong>: 
        <asp:LinkButton ID="lnkJust" CommandArgument="just" runat="server" ClientIDMode="Static" CssClass="" OnClick="lnkSelectREG_Click">Весь каталог</asp:LinkButton>
            <asp:LinkButton ID="lnkMyFavor" CssClass="" ClientIDMode="Static" OnClick="lnkSelectREG_Click" CommandArgument="my" runat="server">Из истории моих заказов</asp:LinkButton>
            <asp:LinkButton CommandArgument="spec" CssClass="" ClientIDMode="Static" ID="lnkSpecial" OnClick="lnkSelectREG_Click" runat="server">Распродажа!!!</asp:LinkButton>
            &nbsp;<asp:CheckBox ID="chIncash" ClientIDMode="Static" runat="server" Text="из наличия" AutoPostBack="True" CssClass="bold" OnCheckedChanged="chIncash_CheckedChanged" />
        </div>
    </div>
    <div class="clearBoth"></div>



    <div class="message">
        <asp:Literal ID="lbReadonly" Visible="false" runat="server" Text="Выбранную заявку нельзя редактировать на сайте. Обратитесь к своему менеджеру."></asp:Literal>
    </div>
    <div id="msgcart" name="msgcart"></div>
    <asp:HiddenField ClientIDMode="Static" ID="itemstack" runat="server" />
    <asp:HiddenField ClientIDMode="Static" ID="tkstack" runat="server" />

    <div class="message">
        <asp:Literal ID="lbMessage" runat="server"></asp:Literal>
    </div>
    <div class="message">
        <asp:Literal ID="lbMess" runat="server"></asp:Literal>
    </div>
    <asp:HiddenField ClientIDMode="Static" ID="refresh" runat="server" />


    <ul class="f-nav f-nav-tabs big bold">
    <li runat="server" id="tab1"><asp:LinkButton ID="lbtnStruct" runat="server" OnClick="lbtnStruct_Click">Классификатор</asp:LinkButton></li>
    <li runat="server" id="tab2"><asp:LinkButton ID="lbtnGoodlist" runat="server" OnClick="lbtnGoodlist_Click">Справочник номенклатуры</asp:LinkButton></li>
        </ul>
    <asp:MultiView ID="mvGoodies" runat="server">


        <style type="text/css">
            #mCat {
                position: absolute;
                z-index: 999;
            }

            .divoverflow {
                overflow: auto;
                /*width: 190px; */
                height: 700px;
            }

            .divTN {
                z-index: 999;
                border: 1px solid #BBBBBB;
                background-color: #DEDEDE;
            }

                .divTN a {
                    text-decoration: none;
                }
        </style>
        <script type="text/javascript">
            $(document).ready(function () {

                $("#btnOpenCat").click(function () {
                    if ($("#mCat").is(":hidden")) {

                        $("#mCat").show("fast");
                        $("#btnOpenCat").text("<< каталог");
                        $("#mCat").css("width:200px");

                    } else {
                        $("#btnOpenCat").text("каталог >>");
                        $("#mCat").hide("fast");
                        $("#mCat").css("width:1px");
                    }
                    return false;
                });
            });


        </script>


       



        <asp:View ID="vCategories" runat="server">
            <div id="mCat">




                <asp:Literal ID="struct_place" runat="server"></asp:Literal>


            </div>
        </asp:View>





        <asp:View ID="vList" runat="server">

            <div class="message small">
                <asp:Literal ID="lbListMessage" runat="server"></asp:Literal>
            </div>


            <%--<i><span class="red bold border"> * </span> - <span class="small">звездочка перед ценой означает заказную позицию и цена ориентировочная. Её нужно уточнить у своего менеджера. Двойной клик на цене - послать запрос по этой позиции.</span></i>--%>
            <div class="big bold">

                <asp:Literal Visible="false" ID="lbSelTN" runat="server"></asp:Literal>

            </div>
            <div id="catTitle" style="line-height: 5px;">&nbsp;</div>
            <div class=" orangefon padding5 ">
                <asp:LinkButton ID="btnClearFilter" runat="server" ClientIDMode="Static" CssClass="small white clear-filter " OnClick="btnClearFilter_Click">сбросить фильтр</asp:LinkButton>
                <div class="bold padding2 white">
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
                <div class="white padding2 bold">
                    поиск по коду, артикулу или части названия
                            <asp:TextBox ID="txSearch" runat="server" Width="200px" MaxLength="150">&nbsp;</asp:TextBox><asp:Button Visible="true" ClientIDMode="Static" ID="btnSearchGood" runat="server" CssClass="f-bu" Text="найти" OnClick="txSearch_TextChanged" />
                </div>
            </div>
            <div>
                на странице
                        <asp:RadioButtonList ID="rbPageSize" runat="server" AutoPostBack="true"
                            RepeatDirection="Horizontal" RepeatLayout="Flow"
                            OnSelectedIndexChanged="rbPageSize_SelectedIndexChanged">
                            <asp:ListItem Text="15" Value="15" Selected></asp:ListItem>
                            <asp:ListItem Text="30" Value="30"></asp:ListItem>
                            <asp:ListItem Text="50" Value="50"></asp:ListItem>
                        </asp:RadioButtonList>
                позиций
            </div>
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








            <script type="text/javascript">






                function checksaved() {
                    var ret = false;
                    if ($('.itemstack').val() != '' && $("span[id*='the_ord']").text().trim() == "") {
                        ret = confirm('реально все очистить?');
                    }
                    else {
                        ret = true;
                        alert($("span[id*='the_ord']").text().trim());
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

        </asp:View>
    </asp:MultiView>
</asp:Content>
