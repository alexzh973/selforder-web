<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="imgselect.aspx.cs" Inherits="imgselect" %>

<!DOCTYPE HTML PUBLIC "-//W3C//DTD HTML 4.0 Transitional//EN">
<html>
<head id="Head1" runat="server">
    <title>image select</title>
    <script type="text/javascript" src="../js/jquery.js"></script>
    <script type="text/javascript" src="../js/ensoCom.js"></script>
    <script type="text/javascript">
        function selectImg(imgpath) {
            if ($(window.parent.$("#img_sel"))) {
                $(window.parent.$("#img_sel").val(imgpath));
            }
        }
        function go_to(pathfieldId, pathValue) {
            $('#' + pathfieldId).val(pathValue);          

            $('form').submit();

        }
        function showselimg(imgpath, w, h) {

            $('#selimg').attr("src", imgpath);
            $('#selimg').width(w);
            $('#selimg').height(h);
        }
    </script>
    <style type="text/css">
        body {
            padding: 0;
            margin: 0;
            font-family: Arial, MS Sans Serif, Verdana;
            font-size: 8pt;
        }

        input {
            font-family: Arial, MS Sans Serif, Verdana;
            font-size: 8pt;
        }

        table {
            font-family: Arial, MS Sans Serif, Verdana;
            font-size: 8pt;
        }

        ul.dircontent {
            padding: 0;
            margin: 0;
        }

            ul.dircontent li {
                float: left;
                list-style: none;
                padding: 2px;
                margin: 2px;
                width: 62px;
                height: 90px;
                border: 1px solid #CCCCCC;
                text-align: center;
            }
    </style>
</head>
<body>
    <form id="form1" runat="server" style="padding: 0; margin: 0;">
        <asp:HiddenField ID="hdSrvPath" runat="server" />
        <table cellpadding="1" cellspacing="1" style="height: 420px; width: 100%;" border="0">
            <tr style="background-color: #DDDDDD;">
                <td><h4>библиотека картинок</h4></td>
                <td><strong>предосмотр </strong>
                </td>
            </tr>
            <tr>
                <td style="width: 50%; border: 1px solid silver" valign="top">
                    <div style="height: 370px; overflow: auto; padding: 0; margin: 0;">
                        <ul class="dircontent">
                            <asp:Literal ID="divBrowse" runat="server"></asp:Literal>
                        </ul>
                        <div style="clear: both;">
                        </div>
                </td>
                <td style="background-color: #FFFFFF; width: 50%; border: 1px solid silver" valign="top"
                    title='возьмите и перетащите к себе'><i>(перетащите картинку мышкой)</i><br/>
                    <img id='selimg' width="24px" height="24px" src="">
                </td>
            </tr>
            <tr style='background-color: #AAAAAA;'>
                <td colspan="2">
                    <hr />
                    <h4>управление библиотекой картинок</h4>
                    <div>
                        <strong>создать новый каталог:</strong>
                        <asp:TextBox runat="server" ID="txNewFolder" Width="100px"></asp:TextBox>
                        <asp:Button ID="btnCreateNewFolder" runat="server" Text="создать" OnClick="btnCreateNewFolder_Click" />
                        <asp:Button Width="18px" runat="server" ID="reload" Text="r" OnClick="reload_Click"
                            Visible="False" />
                    </div>
                    <div>

                        <strong>загрузить в библиотеку новую картинку:</strong>                         
                        <asp:FileUpload ID="FileUpload1" runat="server" />
                        <asp:Button ID="Button1" runat="server" OnClick="Button1_Click" Text="загрузить" />
                    </div>
                </td>
            </tr>
        </table>
    </form>
</body>
</html>
