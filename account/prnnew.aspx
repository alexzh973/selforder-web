<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="prnnew.aspx.cs" Inherits="wstcp.account.prnnew" %>

<!DOCTYPE html>

<html xmlns="http://www.w3.org/1999/xhtml">
<head runat="server">
<meta http-equiv="Content-Type" content="text/html; charset=utf-8"/>
    <title></title>
    <style type="text/css">
        body{font-family: Arial Tahoma Verdana;}
        div{font-family: Arial Tahoma Verdana;} 
        p{font-family: Arial Tahoma Verdana;}
    </style>
    <script src="../js/jquery.js"></script>
    <script src="../js/ensoCom.js"></script>
</head>
<body >
    <form id="form1" runat="server">
    <div id="printed" style="border:2px #666666 solid; padding: 10px;">
        <img src="../media/forum.jpg" width="90%"/>
        <h1>Сервис Самостоятельных Заявок</h1>
        <h2>www.santechportal.ru</h2>
        <div >
            <p>Контрагент <strong><asp:Label ID="lbSubjectName" runat="server" Text=""></asp:Label></strong></p>
            <p>Логин <strong><asp:Label ID="lbLogin" runat="server" Text=""></asp:Label></strong></p>
            <p>Пароль <strong><asp:Label ID="lbPsw" runat="server" Text=""></asp:Label></strong></p>
            </div>
        <p>Главные преимущества  сервиса:</p>
        <ul>
            <li>Оперативность  работы.<br/>Ваша заявка автоматически становится счетом в нашей учетной системе –<br/> это означает, что наш  менеджер будет существенно быстрее<br/> обрабатывать ваш запрос</li> 
<li>Расчет заявки с учетом закрепленной за вами скидки</li> 
<li>Доступна информация о базовой цене</li> 
<li>Доступна информация о складском и заказном товаре</li> 
<li>Доступна информация о количестве товара на складе с учетом вашей заявки</li> 
<li>Доступна информация об акционных товарах и распродажах.</li> 
        </ul>
<p>Сервис самостоятельных заявок  находится  по адресу: www.santechportal.ru</p>

        <p>Только для новых зарегистрированных клиентов в период с 24 по 31 марта 2016 г.<br/> действуют уникально низкие цены на оборудование инженерных систем.</p> 

    </div>
        <a href="#" onclick="window.print();">print</a>
    </form>
</body>
</html>
