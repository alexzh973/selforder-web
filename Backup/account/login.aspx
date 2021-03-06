﻿<%@ Page Title="" Language="C#" MasterPageFile="../common/mainpage.Master" AutoEventWireup="true"
    CodeBehind="login.aspx.cs" Inherits="wstcp.login" %>

<%@ Register src="CaptchaControl.ascx" tagname="CaptchaControl" tagprefix="uc1" %>

<asp:Content ID="Content1" ContentPlaceHolderID="head" runat="Server">
    <script type="text/javascript" src="../js/jquery.js"></script>
    <script type="text/javascript" >
        function aa() {
            
            var h = 300;
            var w = 400;
            $("#dv").height(h);
            $("#dv").width(w);
            $("#dv").css("top", "" + ($(document).height() / 2 - h / 2) + "px");
            $("#dv").css("left", "" + ($(document).width() / 2 - w / 2) + "px");

        }

        aa();
    </script>
</asp:Content>
<asp:Content ID="Content2" ContentPlaceHolderID="ContentPlaceHolder1" runat="Server">
    <div class="small italic">
        <asp:Literal ID="divMessage" runat="server"></asp:Literal></div>
    <div id='dv' class="ipad grayfon" style="position: absolute; z-index: auto; float: left;">
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
                                    <asp:Label ID="lbLogin" runat="server" Text="" CssClass="errormessage bold"></asp:Label>
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
                                            <asp:LinkButton ID="linkRememberPsw" runat="server" OnClick="linkRememberPsw_Click">забыли пароль?</asp:LinkButton></li>
                                        <li style="list-style: none; line-height: 16pt;">
                                        <a href="login.aspx?act=new">хочу получить доступ</a>
                                            </li>
                                    </ul>
                                </td>
                            </tr>
                        </table>
                    </asp:View>
                    <asp:View ID="vSendMessage" runat="server">
                        <asp:Label ID="lbSendMessage" runat="server" Text=""></asp:Label><br />
                        <asp:Button ID="Button1" runat="server" Text=" ОК " onclick="Button1_Click1"/>
                    </asp:View>
                    <asp:View ID="vNewUser" runat="server">
                    <p>Добро пожаловать на сайт santechportal.</p>
                    <p>
                                        Заполните поля ниже и отправьте запрос.<br />
                                        По почте Вам будет выслан пароль для доступа.
                                    </p>
                        <asp:PlaceHolder ID="PlaceHolder1" runat="server">
                        <table width="400px" cellpadding="5" border="0" style="background-color: #EEEEEE;">
                            
                            
                            <tr>
                                <td nowrap="nowrap" valign="top">
                                    Имя
                                </td>
                                <td>
                                    <asp:TextBox ID="txNewName" runat="server" Width="100%" MaxLength="200"></asp:TextBox>
                                    <asp:RequiredFieldValidator ID="RequiredFieldValidator2" runat="server" 
                                        CssClass="errormessage" ErrorMessage="пожалуйста назовитесь" 
                                        ControlToValidate="txNewName" Display="Dynamic" EnableClientScript="False"></asp:RequiredFieldValidator>
                                </td>
                            </tr>
                            <tr>
                                <td nowrap="nowrap" valign="top">
                                    Ваш e-mail
                                </td>
                                <td>
                                    <asp:TextBox ID="txNewEmail" runat="server" Width="100%" MaxLength="200"></asp:TextBox>
                                    <asp:RequiredFieldValidator ID="RequiredFieldValidator5" runat="server" 
                                        CssClass="errormessage" 
                                        ErrorMessage="e-mail нужен, чтобы передать Вам пароль для доступа" 
                                        ControlToValidate="txNewEmail" Display="Dynamic" 
                                        EnableClientScript="False"></asp:RequiredFieldValidator>
                                </td>
                            </tr>
                            
                            <tr>
                            <td></td>
                            <td>
                            <uc1:CaptchaControl ID="CaptchaControl1" runat="server" />
                                <div><asp:Label ID="lbCapchaMessage" runat="server" Text="Окорались с капчей" Visible="false"></asp:Label></div>
                            </td>
                            
                            </tr>

                            <tr>
                                <td nowrap="nowrap" valign="top" colspan="2">
                                    <asp:Button ID="btnNewOK" runat="server" Text="послать запрос" OnClick="btnNewOK_Click" />
                                    <asp:Button ID="btnNewCancel" runat="server" Text="отмена" OnClick="btnNewCancel_Click" />
                                </td>
                            </tr>
                        </table></asp:PlaceHolder>
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
