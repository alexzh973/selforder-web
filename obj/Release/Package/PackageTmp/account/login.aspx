<%@ Page Title="" Language="C#" MasterPageFile="../common/mainpage.Master" AutoEventWireup="true"
    CodeBehind="login.aspx.cs" Inherits="wstcp.login" %>

<%@ Register src="CaptchaControl.ascx" tagname="CaptchaControl" tagprefix="uc1" %>

<asp:Content ID="Content1" ContentPlaceHolderID="head" runat="Server">
    <script type="text/javascript" src="../js/jquery.js"></script>
    <script type="text/javascript" >
        function aa() {
            var h = 300;
            var w = 600;
            //$("#dv").height(h);
            $("#dv").width(w);
            $("#dv").css("top", "" + ($(document).height() / 2 - h / 2) + "px");
            $("#dv").css("left", "" + ($(document).width() / 2 - w / 2) + "px");

        }

        aa();

        function findsubj() {
            $.ajax({
                url: '../account/acc.ashx?sid=' + sid + '&act=finds&val=' + $('#txINN').val(),
                success: function(data) {
                    var s = data.split('+');
                    if (s.length > 0) $('#txSubjName').val(s[0]);
                    if (s.length > 1) $('#txEmailTAs').val(s[1]);
                    if (s.length > 2) {
                        if (s[2] == '*') {
                            $('#lbFIO').addClass('bold');
                            $('#lbEmail').addClass('bold');
                        } else {
                            $('#lbFIO').toggleClass('bold');
                            $('#lbEmail').toggleClass('bold');
                        }
                    } else {
                        $('#lbFIO').addClass('bold');
                            $('#lbEmail').addClass('bold');
                    }
                   
                    
                }
            });
        }
    </script>
    
</asp:Content>
<asp:Content ID="Content2" ContentPlaceHolderID="ContentPlaceHolder1" runat="Server">
    <div class="message">
        <asp:Literal ID="divMessage" runat="server"></asp:Literal>
    </div>
    <div id='dv' class="ipad grayfon">
        <asp:Panel ID="login_div" runat="server" CssClass="visible_login">
            <div style="vertical-align: middle;">
                <asp:MultiView ID="MultiView1" runat="server">
                    <asp:View ID="vLinks" runat="server">
                        <table width="400px" cellpadding="5" border="0" style="background-color: #EEEEEE;">
                            <tr>
                                <td colspan="2" style="background-color: #5469A7; color:White;">
                                    Добро пожаловать
                                </td>
                            </tr>
                            <tr>
                                <td>
                                </td>
                                <td>
                                    <div class="message"><asp:Label ID="lbLogin" runat="server" Text="" CssClass="errormessage bold"></asp:Label></div>
                                </td>
                            </tr>
                            <tr>
                                <td nowrap="nowrap" valign="top">
                                    <strong>ваш e-mail</strong>:
                                </td>
                                <td>
                                    <asp:TextBox ID="txEmail" runat="server" CausesValidation="True" Width="100%" 
                                        MaxLength="100" AutoCompleteType="Email"></asp:TextBox>
                                    <div class='attributes'>
                                        <asp:RequiredFieldValidator ID="RequiredFieldValidator1" runat="server" CssClass="errormessage" 
                                            ControlToValidate="txEmail" ErrorMessage="Вы забыли указать email"></asp:RequiredFieldValidator>
                                    </div>
                                    
                                    <div id="divsmscode">
                           
                            <span>для доступа используйте пожалуйста пароль переданный Вам СМС-сообщением</span>
                           <a href="#" onclick="showSmsCode()">повторить отправку пароля</a>
                            </div>
                                </td>
                            </tr>
                            <tr>
                                <td nowrap="nowrap" valign="top">
                                    <strong>пароль</strong>:
                                </td>
                                <td width="90%">
                                    <asp:TextBox ID="txPwd" runat="server" TextMode="Password" CausesValidation="True"  
                                        Width="100%" MaxLength="100"></asp:TextBox><br />
                                    <div class='attributes'>
                                    </div>
                                </td>
                            </tr>
                            
                            
                            <tr>
                                <td>
                                </td>
                                <td>
                                    <asp:Button ID="btnLogin" UseSubmitBehavior="true"  runat="server" Text="ОК" OnClick="btnLogin_Click" Width="83px" />
                                </td>
                            </tr>
                            <tr>
                                <td>
                                </td>
                                <td>
                                    <ul>
                                        <li style="list-style: none; line-height: 16pt;">
                                            <asp:LinkButton ID="linkRememberPsw" runat="server" OnClick="linkRememberPsw_Click" ToolTip="Нажмите, чтобы вспомнить">забыли пароль?</asp:LinkButton></li>
                                        <li style="list-style: none; line-height: 16pt;">
                                        <a href="login.aspx?act=new">хочу получить доступ</a>
                                            </li>
                                    </ul>
                                </td>
                            </tr>
                        </table>
                        
                        <script type="text/javascript">

                            $(document).ready(function () {
                                $('#divsmscode').hide();
                            });


                            function showSmsCode() {
                                $.ajax({
                                    url: '../subj.ashx?sid=' + sid + '&act=trygetsms&e=' + $('#txLogin').val(),
                                    success: function (data) {
                                        if (data == 'Y') {
                                            alert('Вам выслана смс с паролем доступа');
                                            $('#divsmscode').show();
                                        }
                                    }
                                });
                            }
            </script>
                    </asp:View>
                    <asp:View ID="vSendMessage" runat="server">
                        <asp:Label ID="lbSendMessage" runat="server" Text=""></asp:Label><br />
                        <asp:Literal ID="linkPrn" runat="server"></asp:Literal>
                        <asp:Button ID="Button1" runat="server" Text=" ОК " onclick="Button1_Click1"/>
                    </asp:View>
                    <asp:View ID="vNewUser" runat="server">
                    <p>Добро пожаловать на сайт santechportal.</p>
                    <p>
                                        Заполните поля ниже и отправьте запрос.<br />
                                        По почте Вам будет выслан пароль для доступа.
                                    </p>
                        <asp:PlaceHolder ID="PlaceHolder1" runat="server">
                            <script>
                                function acckpp() {
                                    $('.txkpp').val($('.txinn').val().substr(0, 4) + '01001');
                                    return;
                                }
                            </script>
                                    <table width="400px" cellpadding="5" border="0" style="background-color: #EEEEEE;">
                                        <tr><td><div class="message">
                                            <asp:Literal ID="lbError" runat="server"></asp:Literal></div></td></tr>
                                    <tr>
                                    <td nowrap="nowrap" valign="top">
                                    <b >* ИНН Предприятия, которое Вы представляете</b>&nbsp;<asp:LinkButton ID="LinkButton1" ToolTip="Попробовать найти" runat="server" OnClick="lbtnFind_Click">???</asp:LinkButton> <br/>
                                    
                                    <asp:TextBox ID="txINN" runat="server" CssClass="txinn"  onchange="findsubj()" Width="100%" ClientIDMode="Static" MaxLength="12"></asp:TextBox>
                                    
                                    </td>
                                    </tr>
                                    
<tr>
                                    <td nowrap="nowrap" valign="top">
                                    <b>* Наименование Предприятия, которое Вы представляете</b>&nbsp;<asp:LinkButton ID="lbtnFind" ToolTip="Попробовать найти" runat="server" OnClick="lbtnFind_Click">???</asp:LinkButton> <br/>
                                
                                    <asp:TextBox ID="txSubjName" ClientIDMode="Static" runat="server" Width="100%" MaxLength="200"></asp:TextBox>
                                    <asp:GridView CssClass="small" ID="dgFinded" runat="server" AutoGenerateColumns="False" OnSelectedIndexChanged="dgFinded_SelectedIndexChanged" AllowPaging="True" OnPageIndexChanging="dgFinded_PageIndexChanging" PagerSettings-Mode="NextPreviousFirstLast" PagerSettings-Position="TopAndBottom" PagerSettings-NextPageText="--&gt;" PagerSettings-PreviousPageText="&lt;--" PagerSettings-LastPageText="&gt;|" PagerSettings-FirstPageText="|&lt;" ShowHeader="False">
                            <Columns>
                                <asp:BoundField DataField="INN" />
                                <asp:BoundField DataField="NameSubj" ShowHeader="False" />
                                <asp:BoundField DataField="Owner" ShowHeader="False" Visible="false" />
                                <asp:BoundField DataField="OwnerID" Visible="false"   />
                                <asp:CommandField ShowSelectButton="True" />
                            </Columns>
                        </asp:GridView>
                                    </td>
                                    </tr>
                                        <tr>
                                            <td><b>Филиал Сантехкомплекта</b><br/>
                                                <asp:DropDownList ID="dlOwners" runat="server"></asp:DropDownList>   
                                            </td>
                                        </tr>
                                    <tr>
                                    <td nowrap="nowrap" valign="top">
                                        <asp:Label ID="lbFIO" ClientIDMode="Static" runat="server" CssClass="bold" Text="* Ваше имя (ФИО)"></asp:Label><br/>
                                
                                    <asp:TextBox ID="txNewName" ClientIDMode="Static" runat="server" Width="100%" MaxLength="200"></asp:TextBox>
                                    
                                    </td>
                                    </tr>
                                    <tr>
                                    <td nowrap="nowrap" valign="top">
                                        <asp:Label ID="lbEmail" ClientIDMode="Static" CssClass="bold" runat="server" Text="* Ваш e-mail"></asp:Label>
                                    <br/>
                                
                                    <asp:TextBox ID="txNewEmail" ClientIDMode="Static" runat="server" Width="100%" MaxLength="200"></asp:TextBox>
                                    
                                    </td>
                                    </tr>
                                    <tr>
                                    <td nowrap="nowrap" valign="top">
                                        <asp:Label ID="lbPhones" runat="server" Text="Ваши телефоны"></asp:Label>
                                    <br/>
                                
                                    <asp:TextBox ID="txPhones" runat="server" Width="100%" MaxLength="200"></asp:TextBox>
                                    
                                    </td>
                                    </tr>
                                       
                                        <tr runat="server" id="fieldseller">
                                            <td>
                                                email ответственного ТА
                                                <asp:TextBox ID="txEmailTAs" ClientIDMode="Static" runat="server" Width="100%" MaxLength="200"></asp:TextBox>
                                            </td>
                                        </tr>
                                        

                                    <tr>
                           
                                    <td>
                                    <uc1:CaptchaControl ID="CaptchaControl1" runat="server" />
                                    
                                    </td>
                            
                                    </tr>

                                    <tr>
                                    <td nowrap="nowrap" valign="top" >
                                    <asp:Button ID="btnNewOK" runat="server" Text="послать запрос" OnClick="btnNewOK_Click" />
                                    <asp:Button ID="btnNewCancel" runat="server" Text="отмена" OnClick="btnNewCancel_Click" />
                                    </td>
                                    </tr>
                                        
                                        <tr><td>
                                            <asp:LinkButton ID="lbtnDemo" runat="server" OnClick="lbtnDemo_Click">Демонстрационный доступ</asp:LinkButton></td></tr>
                                    </table>
                                    </asp:PlaceHolder>
                        
                        

                                    </asp:View>
                                    </asp:MultiView>
                                    </div>
                                    </asp:Panel>
                                    </div>
                                    <div style="clear: both">
                                    </div>
                                    <script type="text/javascript" language="javascript">
                                        aa();
                                    </script>
</asp:Content>
