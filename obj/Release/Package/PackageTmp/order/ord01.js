$(document).ready(function () {
    $("tr[id*='tr_']").mouseover(function (e) { $(this).addClass('itemover'); });
    $("tr[id*='tr_']").mouseout(function (e) { $(this).removeClass('itemover'); });
    $("#dlTK").change(changefilter);
    $("#dlBrends").change(changefilter);
    $("#dlNames").change(changefilter);
    $("#chIncash").click(changefilter);
    $("#lnkJust").click(changefilter);
    $("#lnkMyFavor").click(changefilter);
    $("#lnkSpecial").click(changefilter);
    $("#btnSearchGood").click(changefilter);
    $("#btnClearFilter").click(changefilter);

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


        img.css("top", (e.pageY - 150));
        img.css("left", e.pageX - 100);

        img.show('fast');
        img.addClass("border");
        img.addClass("shadow");
    });


});

function tkInStack(thischeckboxid, val) {

    var thStack = $("#tkstack");
    var chbox = $("#" + thischeckboxid);
    var curval = thStack.val();

    if (chbox.prop("checked")) {
        if (curval.indexOf(val) < 0)
            thStack.val(curval + ((curval != "") ? "," : "") + val);
    }
    else {
        if (curval.indexOf(val) >= 0) {
            thStack.val(curval.replace("," + val, "").replace(val, "").replace(",,", ","));
        }
        if (thStack.val().indexOf(",") == 0)
            thStack.val(thStack.val().substring(1));


    }
    var r = "";
    $("form :checkbox[name='ch_tks']:checked").each(function () {

        var cc = $(this).val().replace(",", "@");
            r += ( r != "") ? "," : "";
                r+= cc;
        });
   

    

    $.ajax({
        url: '../order/iam.ashx?sid=' + sid + '&act=seltks&val=' + r,
        success: function (data) { }
    });
}


function setnewstate(orderId, newstate) {
    $.ajax({
        url: '../order/orderajax.ashx?act=sns&id=' + orderId + '&ns=' + newstate+ '&sid=' + sid,
        success: function (data) {
            this.forms[0].submit();
        }
    });
}
function setname() {
    $.ajax({
        url: '../order/orderajax.ashx?act=setname&sid=' + sid + '&name=' + $("#txNameOrder").val(),
        cache: false,
        error: function (jqxhr, status, errorMsg) {
            $('#qmsg').text(errorMsg);
            setTimeout(clearmsg, 7000);
        }
    });
}
function setdescr() {
    
    $.ajax({
        url: '../order/orderajax.ashx?act=setdescr&sid=' + sid + '&descr=' + $("#txDescr").val(),
        cache: false,
        error: function (jqxhr, status, errorMsg) {
            $('#qmsg').text(errorMsg);
            setTimeout(clearmsg, 7000);
        }
    });
}
function showOrderSumm() {
    $.ajax({
        url: '../order/orderajax.ashx?act=summ&sid=' + sid,
        cache: false,
        success: function (data) {
            $('.cartbtn').text(data);
        },
        error: function (jqxhr, status, errorMsg) {
            $('#qmsg').text(errorMsg);
            setTimeout(clearmsg, 7000);
        }
    });
}
function itemInOrder(act, goodId, qty, descr) {
    //alert('itemInOrder');
    $.ajax({
        url: '../order/orderajax.ashx?act=' + act + '&gid=' + goodId + '&qty=' + qty + '&descr='+descr+'&sid=' + sid,
        cache: false,
        success: function (data) {
            if ($("#cartbut") != null) {
                $("#cartbut").removeClass("hidden");
            }
            $(".cartbut").addClass("brighttab");
            $(".cartbtn").addClass("brighttab");
            
            showOrderSumm();
            setTimeout(clearmsg, 1000);

        },
        error: function (jqxhr, status, errorMsg) {
            $('#qmsg').text(errorMsg);
            setTimeout(clearmsg, 3000);
        }
    });
}

function clearmsg() {
    $(".cartbtn").removeClass("brighttab");
    $(".cartbut").removeClass("brighttab");
}



function changeQty(goodId,krat) {
    var chbox = $("#ch_" + goodId);
    var inpqty = $("#qch_" + goodId);
    if (inpqty.val().trim() == "" || inpqty.val().trim() == "0") {
        chbox.removeAttr("checked");
        inpqty.val("");
        itemInOrder('r', goodId, inpqty.val(), '');
        $('#tr_' + goodId).removeClass("selitem");
    }
    else {
        chbox.attr("checked", "checked");
        inpqty.val(calcByKrat(inpqty.val(), krat));
        itemInOrder('a', goodId, inpqty.val(), '');
        $('#tr_' + goodId).addClass("selitem");
    }
}
function checkChange(goodId,qty) {
    var chbox = $("#ch_" + goodId);
    var inpqty = $("#qch_" + goodId);

   
    


    if (chbox.prop("checked")) {
        if (!inpqty.val() || inpqty.val().trim() == "" || inpqty.val().trim() == "0") {
            inpqty.val(qty);
            
        }
        itemInOrder('a', goodId, inpqty.val(),'');
        $('#tr_' + goodId).addClass("selitem");
    }
    else {
        itemInOrder('r', goodId, inpqty.val(),'');
        $('#tr_' + goodId).removeClass("selitem");
        inpqty.val("");
    }
}

function shw(thischeckboxid, num) {
    var chbox = $("#" + thischeckboxid);

    if (chbox.prop("checked")) {
        $("#tr_" + num).addClass("selitem");
    }
    else {
        $("#tr_" + num).removeClass("selitem");
    }
}
function recount(goodid) {
    recount(goodid, 1);
}
function recount(goodid, krat) {
    var qf = $("#q_" + goodid);
    qf.val(calcByKrat(qf.val(), krat));

    var q = myParseFloat( qf.val());
    var pr = myParseFloat( $("#pr_" + goodid).val());
    
    $("#sm_" + goodid).text("" + pr * q);
    $("#refresh").val("Y");
    $("form").submit();
}

function chngitemdescr(goodId) {
   // alert($("#ds_" + goodId).text());
    var q = $("#q_" + goodId).val();
    var ds = $("#ds_" + goodId).val();
    //$("#refresh").val("Y");
    //$("form").submit();

    $.ajax({
        url: '../order/orderajax.ashx?&gid=' + goodId + '&qty=' + q + '&descr=' + ds + '&sid=' + sid,
        cache: false,
        error: function (jqxhr, status, errorMsg) {
            $('#qmsg').text(errorMsg);
            setTimeout(clearmsg, 3000);
        }
    });
}

function recount_no(goodid) {
    var q = $("#q_" + goodid).val();
    q = myroundEx(q, 0);
    $("#q_" + goodid).val('' + q);
    $.ajax({
        url: "orderajax.ashx?sid=" + sid + "&act=rcnt&gid=" + goodid + "&qty=" + q,
        cache: false,
        success: function (data) {
            var d;
            var prId, qId, smId;
            var pr, q, sm, itg;
            itg = 0;
            $("input[id^='q_']").each(function () {
                d = $(this).attr("id").replace("q_","");
                qId = $(this).attr("id");
                prId = "pr_" + d;
                smId = "sm_" + d;
                
                pr = $("#" + prId).text(); 
                q = $("#" + qId).val();alert(pr);
                sm = myroundEx(pr * q, 2);
                itg += sm;
                $("#" + smId).text(sm);
                
            });
            $("#orderitg").text(myroundEx(itg,2));
            $('#qmsg').text(data); $('#qmsg').show();
            showOrderSumm();
            setTimeout(clearmsg, 3000);

        },
        error: function (jqxhr, status, errorMsg) {
            $('#qmsg').text(errorMsg);
            setTimeout(clearmsg, 3000);
        }
    });
}


function showImg(goodId) {
    var img = $("#img" + goodId);
    if (img) {



    }
}



function findtks() {
    var txs = "" + $("#searchtks").val();
    
    $(".tklink").removeClass('finded');
    
    $("a[data*='" + txs + "']").each(function () {
        
        $('#' + this.id).addClass('finded');
    });

    return;
    $("label[for*='ch_tk']").removeClass('finded');
   
    if (txs.length < 3) return;
    $("input[value*='" + txs + "']").each(function () {
        $("label[for='" + this.id + "']").addClass('finded');
    });
    $("div:contains(" + txs + ")]").each(function () {
        alert(txs);
        this.addClass('finded');

    });
}

function changefilter() {
    if ($("#catTitle"))
        $("#catTitle").addClass('waitload');
}
