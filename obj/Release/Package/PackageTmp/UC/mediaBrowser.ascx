<%@ Control Language="C#" AutoEventWireup="true" CodeBehind="mediaBrowser.ascx.cs"
    Inherits="mediaBrowser" %>
<script type="text/javascript">

    function insertTag(textareaId) {
        var _tag_start = "BBBBBBB"; //"<img src='"+$('#img_sel').val()+"'>";
        var _tag_end = "DDDDDD";
        // берем объект
        var _obj_name = $('#' + textareaId).attr("name");
        //alert(_obj_name); alert(document.getElementsByName(_obj_name));
        var area = document.getElementsByName(_obj_name).item(0);

        // Mozilla и другие НОРМАЛЬНЫЕ браузеры
        if (document.getSelection)// если есть что-либо выделенное
        { // берем все что до выделения
            //alert(area.selectionStart);
            area.value = area.value.substring(0, area.selectionStart) +
            //alert('010');
            // вставляем стартовый тег
        _tag_start +

            // вставляем выделенный текст
  area.value.substring(area.selectionStart, area.selectionEnd) +

            // вставляем закрывающий тег
  _tag_end +

            // вставляем все что после выделения
  area.value.substring(area.selectionEnd, area.value.length);
            //alert('1');
        }

        // Заплатка для ебучего Internet Explorer, извинете за грубость,
        // но других слов просто нет, так как уже честно заебался в каждой функции
        // писать под него заплатки
        else {
            var selectedText = document.selection.createRange().text; // берем текст
            if (selectedText != '')// если имеется какой-то выделенный текст
            {
                var newText = _tag_start + selectedText + _tag_end; // составляем новые текст
                document.selection.createRange().text = newText; // вставляем новый текст
            }
            //alert('2');
        }
    }
</script>

<script type="text/javascript">
    $(function () {
        $("#MEDIABROWSE:ui-dialog").dialog("destroy");

        $("#MEDIABROWSE").dialog({
            autoOpen: false            
        });
    });

    function brws() {
        var fr = "<iframe id='frImgBrowse' src='../special/imgselect.aspx' style='width:598px;height:510px;border: 0px solid green;padding:0px;margin:0;' ></iframe>";
        $("#frmCont").html(fr);
        //alert($("#frmCont").html());

        $.fx.speeds._default = 200;
        $("#MEDIABROWSE").dialog({
            autoOpen: false,
            resizable: false,
             
            width:600,
            height:590,          
            title: 'Выберите изображение и перетащите к себе',
            modal: false
        });

        
        $("#MEDIABROWSE").dialog("open");
    }
</script>
<style>
    #MEDIABROWSE
    {
        margin:0;
        padding:0;
        border:0px solid red;

    }
    #divSelectImage
    {
        height:26px;
        border: 0px solid green;
        padding:4px;
        margin:0px;
    }
    #frImgBrowse
    {
        
    }
</style>
<a href="javascript: return false;" onclick="brws()" class="button" style='background-image: url(../simg/16/mediafolder.png);
    background-repeat: no-repeat; padding-left: 20px;'>выбрать изображение</a>
<div id="MEDIABROWSE">
    <div id="divSelectImage">
        выбран 
        <input style="width: 250px" id="img_sel" type="text" value="" /><asp:Literal ID="btnInsert" Visible="false"
            runat="server"></asp:Literal>
    </div>
    <div id="frmCont"></div>
<%--<iframe id='_frImgBrowse' runat='server' src='../simg/16/mediafolder.png' style='width:598px;height:510px;border: 0px solid green;padding:0px;margin:0;' ></iframe>--%>
</div>
