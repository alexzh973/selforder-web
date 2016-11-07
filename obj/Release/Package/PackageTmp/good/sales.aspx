<%@ Page Title="" Language="C#" MasterPageFile="../common/mainpage.Master" AutoEventWireup="true"
    CodeBehind="sales.aspx.cs" Inherits="wstcp._sales" %>




<asp:Content ID="Content1" ContentPlaceHolderID="head" runat="server">
    <style type="text/css">
        .reclamaplace {
            border: 2px dotted #AAAAAA;
            padding: 4px;
            margin: 5px;
            text-align: center;
            vertical-align: middle;
            font-weight: bold;
            background-repeat: no-repeat;
            width: 205px;
            height: 200px;
            vertical-align: bottom;
        }

            .reclamaplace table {
                height: 100%;
            }

        .reclama-price {
            font-size: 12pt;
            color: red;
        }

        .reclama-title {
            font-size: 8.5pt;
            color: #333366;
        }


        .reclamaplace a {
            text-decoration: none !important;
        }

            .reclamaplace a:hover {
                color: blue;
            }

            .brighttab {
    background-color: #ffa500 !important;
    color: white !important;
}
    </style>
    <script type="text/javascript">
        $(document).ready(function () {

            $(".pic").mouseover(function (e) { $(this).addClass('picover'); });
            $(".pic").mouseout(function (e) { $(this).removeClass('picover'); });


            //$(".reclamaplace").draggable({
            //    cursor: 'move',
            //    containment: '#reclContainer',
            //    stop: reclMoved
            //});

            //$(".reclamaplace").droppable({
            //    drop:eventHandlerDrop
            //});
        });

        //function reclMoved(event, ui) {
        //    //alert("X:" + ui.offset.left + ", Y:" + ui.offset.top);
        //}
        //function eventHandlerDrop(event, ui) {
        //    var good = ui.draggable;
        //    alert("Code:" + good.attr("id") + " над " + $(this).attr("id"));
        //}

        //function checkemail(emailfield, messagefieldId) {

        //    if ($('#' + emailfield) && '' + $('#' + emailfield).val() == '') {
        //        alert('в поле логин (еmail) нужно указать свой еmail');
        //        return false;
        //    }
        //    else {
        //        return true;
        //    }

        //}

        //function setnewstate(id, newstate) {
        //    $.ajax({
        //        url: "../order/orderajax.ashx?sid=" + sid + "&act=sns&ns=" + newstate + "&id=" + id,
        //        cache: false,
        //        success: function (data) {
        //            if (newstate == 'D')
        //            { $("tr[id*='ord_" + id + "']").hide(); }

        //            $('#hdRefresh').val('Y');
        //            $('form').submit();


        //        },
        //        error: function (jqxhr, status, errorMsg) {
        //            $("#qmsg").text(errorMsg);
        //            setInterval(clearmsg, 7000);
        //        }
        //    });
        //}

        function itemInOrder(act, goodId, qty, descr) {
            $.ajax({
                url: '../order/orderajax.ashx?act=a&gid=' + goodId + '&qty=' + qty + '&sid=' + sid,
                cache: false,
                success: function (data) {
                    if ($("#cartbut") != null) {
                        $("#cartbut").removeClass("hidden");
                    }
                    $(".cartbut").addClass("brighttab");

                    showCartSumm();
                    setTimeout(clearbrighttab,1000);
                    
                    

                },
                error: function (jqxhr, status, errorMsg) {
                    $(".cartbut").removeClass("brighttab");
                    $('#qmsg').text(errorMsg);
                    setTimeout(clearmsg, 3000);
                }
            });
        }

        function showCartSumm() {
            $.ajax({
                url: '../order/orderajax.ashx?act=currinfo&sid=' + sid,
                cache: false,
                success: function (data) {
                    if (data != "") {
                        $("#cartbut").html(data);
                    } else {
                        $("#cartbut").html("");
                    }
                    

                }
                
            });
        }
        function clearbrighttab() {
           
            $(".cartbut").removeClass("brighttab");
        }
    </script>
</asp:Content>
<asp:Content ID="Content2" ContentPlaceHolderID="ContentPlaceHolder1" runat="server">
    <div class="message">
        <asp:Literal ID="lbMess" runat="server"></asp:Literal>
    </div>
    <asp:Repeater ID="rpListAcc" runat="server">
        <ItemTemplate>
            <div class="floatLeft shadow border padding5" style="width: 200px; margin: 10px;">
                <% if (iam.IsSuperAdmin || iam.IsAdmin)
                   {
                       %><div class="right micro"><a href="../good/list.aspx?id=<%#Eval("ID") %>">изменить</a></div><%
                   }
                     %>
                <a href="sales.aspx?id=<%#Eval("ID") %>" style="text-decoration: none;">
                    <table>
                    <tr><td><img src="../img.ashx?act=acc&id=<%#Eval("img") %>"/></td>
                   <td>
                    <span class="small bold"><%# cDate.cToString(Eval("StartDate")) %> - <%# cDate.cToString(Eval("FinishDate")) %></span>
                    </td>
                    </tr>
                        <tr><td colspan="2">
                    <span class="big bold"><%#Eval("Name") %></span>
                    <div class="small"><%#Eval("Descr") %> </div></td>
                            </tr>
                     </table>
                    
                </a>
            </div>
        </ItemTemplate>
        <FooterTemplate>
            <div class="clearBoth"></div>
        </FooterTemplate>
    </asp:Repeater>

    <div></div>
    <h2 class="padding5 center">
        <asp:Label ID="lbTitleAcc" runat="server" Text=""></asp:Label></h2>
    <asp:Repeater ID="rpAccItems" runat="server">
        <HeaderTemplate>

            <div id="reclContainer">
        </HeaderTemplate>
        <ItemTemplate>
            <div style="float: left;" class="reclamaplace" id='g_<%#Eval("GoodCode") %>'>
                <table>
                    <tr>
                        <td>
                            <img src="../img.ashx?act=good&id=<%#Eval("goodId") %>" width="100" /></td>
                        <td>
                            <div class="reclama-price"><%#Eval("price") %>р.</div>
                            <br />
                            <br />
                            <div><a class="bold" href="#" onclick="itemInOrder('a','<%#Eval("GoodId") %>','<%#Eval("SaleKrat") %>','')">купить</a></div>
                        </td>
                    </tr>
                    <tr>
                        <td colspan="2"><a class="reclama-title" href="javascript:return 0;" onclick="openflywin('http://santex-ens.webactives.ru/get/<%#Eval("ens") %>/?key=x9BS1EFXj0', 800,700,'Описание товара')"><%#Eval("NameGood") %></a></td>
                    </tr>
                    <tr>
                        <td colspan="2" class="right"></td>
                    </tr>
                </table>
            </div>

        </ItemTemplate>
        <FooterTemplate>
            <div style="clear: both;"></div>
            </div>
        </FooterTemplate>
    </asp:Repeater>

    <div></div>
    <asp:Repeater ID="rpOffer" runat="server">
        <HeaderTemplate>
            <h2 class="padding5 center">Распродажа остатков!!!</h2>
            <div id="reclContainer">
        </HeaderTemplate>
        <ItemTemplate>
            <div style="float: left;" class="reclamaplace" id="g_<%#Eval("GoodCode") %>">
                <table>
                    <tr>
                        <td>
                            
                            <img src="../img.ashx?act=good&id=<%#Eval("goodId") %>" width="100" />
                        </td>
                        <td>
                            <div class="reclama-price"><%#Eval("price") %>р.</div>
                            <br />
                            <br />
                            <div><a class="bold" href="#" onclick="itemInOrder('a','<%#Eval("GoodId") %>','<%#Eval("SaleKrat") %>','')">купить</a></div>
                        </td>
                    </tr>
                    <tr>
                        <td colspan="2"><a class="reclama-title" href="javascript:return 0;" onclick="openflywin('http://santex-ens.webactives.ru/get/<%#Eval("ens") %>/?key=x9BS1EFXj0', 800,700,'Описание товара')"><%#Eval("Name") %></a></td>
                    </tr>
                    <tr>
                        <td colspan="2" class="right"></td>
                    </tr>
                </table>
            </div>

        </ItemTemplate>
        <FooterTemplate>
            <div style="clear: both;"></div>
            </div>
        </FooterTemplate>
    </asp:Repeater>


</asp:Content>
<asp:Content ID="ContentL" runat="server" ContentPlaceHolderID="place_Left">
    <div>
    <asp:Repeater ID="Repeater1" runat="server">
        <HeaderTemplate><h4>Акции</h4></HeaderTemplate>
        <ItemTemplate>
            <div class=" shadow border padding5" style="width: 110px; margin: 0px;">
                <% if (iam.IsSuperAdmin || iam.IsAdmin)
                   {
                       %><div class="right micro"><a href="../good/list.aspx?id=<%#Eval("ID") %>">изменить</a></div><%
                   }
                     %>
                <a style="text-decoration: none;" href="sales.aspx?id=<%#Eval("ID") %>">
                    <span class="small bold"><%#Eval("Name") %></span><br />

                    <span class="micro bold">с <%# cDate.cToString(Eval("StartDate")) %> по <%# cDate.cToString(Eval("FinishDate")) %></span>
                </a>
            </div>
        </ItemTemplate>
        <SeparatorTemplate><br/></SeparatorTemplate>
        <FooterTemplate></FooterTemplate>
    </asp:Repeater>
        </div>
</asp:Content>
