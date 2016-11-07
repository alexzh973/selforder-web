var selectedRowColor = '#eeeeFF';
var zamcol = '';
var curentRowId = 0;

$(document).ready(function () {
    $("a[href='#']").attr("href", "javascript:return 0;");
    
});

function maybeEsc(keycode) {
    alert(keyCode);
    //if (keyCode==13) {
    //    $("#blumdiv").hide();
    //}
}

function thisRow(selectedRowId) {
    thisrow(selectedRowId);
}

$(document).ready(function () {
    $(".message").hide();
    $(".message").each(function () { if ($(this).text().trim() != '') $(this).show(); });
});

function thisrow(selectedRowId) {
    
    $("tr.rowItem").css("background-color",'#FFFFFF');
    $("tr.rowFolder").css("background-color",'#FFFFFF');
	try{
	if (zamcol!=""){
		$('#'+curentRowId).css("background-color",zamcol);
	}
		zamcol = ($('#'+selectedRowId).css("background-color")!='undefined')?$('#'+selectedRowId).css("background-color"):'#FFFFFF';
		$('#'+selectedRowId).css("background-color", selectedRowColor);
		curentRowId = selectedRowId;
	}
	catch(e){
		if($('#'+curentRowId)) $('#'+curentRowId).css("background-color",'#FFFFFF');
		if( $('.curid') && $('.curid').length>0 ) $('.curid')[0].value='';			
	}
}
var selectedRowColor_lite = '#EBEBFB';
var zamcol_lite = '';
var curentRowId_lite = 0;

function thisRowLite(selectedRowId) {
    $("tr.rowItem").css("background-color", '#FFFFFF');
    $("tr.rowFolder").css("background-color", '#FFFFFF');
    try {
        if (zamcol_lite != "") {
            $('#' + curentRowId_lite).css("background-color", zamcol_lite);
        }
        zamcol_lite = ($('#' + selectedRowId).css("background-color") != 'undefined') ? $('#' + selectedRowId).css("background-color") : '#FFFFFF';
        $('#' + selectedRowId).css("background-color", selectedRowColor_lite);
        curentRowId_lite = selectedRowId;
    }
    catch (e) {
        if ($('#' + curentRowId_lite)) $('#' + curentRowId_lite).css("background-color", '#FFFFFF');
       
    }
}


function myConfirm(type) {
    var mess = '';
    switch (type) {
        case '':
            mess = 'Подтвердите действие?';
            break;
        case 'DEL_LINK': 
            mess = 'Вы подтверждаете удаление?\n\nОбъект НЕ будет удален.\nУдаляется только связь.'; 
            break;
        default:
            mess = type;
            break;

    }
    return confirm(mess);
}
$(function () {
    if ($("#dialog")) {
        $("#dialog:ui-dialog").dialog("destroy");

        $("#dialog").dialog({
            autoOpen: false
        });
    }
});
function openflywin(queryLink) {
    a = openflywin(queryLink, 500, 500);
    return a;
}
function openflywin(queryLink, width, height) {
    openflywin(queryLink, width, height, '');
   
     return false;
 }

// function openflymsg(textmsg, w, h, window_title) {
//     if (!$("#dialog")) return false;
//     $.fx.speeds._default = 200;
//     $("#dialog").dialog({
//         autoOpen: false,
//         top: "0",
//         hide: "fade",
//         modal: true,
//         width: w,
//         height: h,
//         title: window_title
//     });

//     if ($("#dialog")) {
//         $("#dialog").html(textmsg);
//         
//         $("#dialog").dialog("open");
//     }
//     return false;
// 
// }

function openflywin(queryLink, w, h, window_title) {
    if (!$("#dialog")) return false;
    $.fx.speeds._default = 200;
    $("#dialog").dialog({
        autoOpen: false,
        top: "0",
        hide: "fade",
        modal: true,
        width: w,
        height: h,
        title: window_title
    });

    if ($("#dialog")) {
        $("#dialog").html("<iframe width='99%' height='99%' style='border:0;' frameborder='0' border='0' src='" + queryLink + "'></iframe>");
        //$("#dialog").css("top", "100px");
        $("#dialog").dialog("open");
    }
    return false;
}


function printBlock(printing_block_id) {

    prn_area = $("#" + printing_block_id).html(); //забираем контент нужного нам блока (в моем случае ссылка на печать находится внути его)
    //alert(prn_area);
    $('body').addClass('printSelected'); //добавляем класс к body

    $('body').append('<div class="printSelection">' + prn_area + '</div>'); //создаем новый блок внутри body
    window.print(); //печатаем

    window.setTimeout(pageCleaner, 0); //очищаем нашу страницу от "мусора"

    return false; //баним переход по ссылке, чтобы она не пыталась перейти по адресу, указанному внутри аттрибута href
}

function pageCleaner() {
    $('body').removeClass('printSelected'); //убираем класс у body
    $('.printSelection').remove(); //убиваем наш только что созданный блок для печати
    $('#help-button').show();
}

