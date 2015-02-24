<%@ Control Language="c#" AutoEventWireup="false" Codebehind="master.ascx.cs" Inherits="UsuSite.master" TargetSchema="http://schemas.microsoft.com/intellisense/ie5"%>
<!DOCTYPE html PUBLIC "-//W3C//DTD XHTML 1.0 Strict//EN" "http://www.w3.org/TR/xhtml1/DTD/xhtml1-strict.dtd">
<html xmlns="http://www.w3.org/1999/xhtml">
<head>
<meta http-equiv="content-type" content="text/html; charset=utf-8" />
	<title>Вадим Канторов. <%= PageName %> </title>
	<meta name="keywords" content="" />
	<meta name="description" content="" />
	<link href="default.css" rel="stylesheet" type="text/css" />
	<link href="highslide/highslide.css" rel="stylesheet" type="text/css" />
	<!--[if lt IE 7]>
		<link rel="stylesheet" type="text/css" href="/highslide/highslide-ie6.css" />
	<![endif]-->
	<script type="text/javascript" src="highslide/highslide.js"></script>
</head>
<body>
<form runat="server">
<div id="wrapper">
<!-- start header -->
<div id="header">
	<div id="logo">
		<h1><a href="#">Вадим Канторов </a></h1>
		<h2> Даёшь допуск к экзамену!</h2>
	</div>
	<div id="menu">
		<ul>
			<li class="current_page_item"><a href="default.aspx">Главная</a></li>
			<li><a href="guestbook.aspx">Гостевая книга</a></li>
			<li><a href="photos.aspx">Галерея</a></li>
			<li><a href="bio.aspx">О себе</a></li>
			<li class="last"><a href="contacts.aspx">Контактная информация</a></li>
		</ul>
	</div>
</div>
<!-- end header -->

<!-- start page -->
<div id="page">
	<!-- start content -->
	<div id="content">
		<asp:placeholder id="phContent" runat="server" />
	</div>
	<!-- end content -->
	<!-- start sidebar -->
	<div id="sidebar">
		<ul>
			<li>
				<h2>Полезные ссылки</h2>
				<ul>
					<li><a href="http://icanhascheezburger.com">I CAN HAS CHEEZBURGER</a></li>
					<li><a href="http://community.livejournal.com/ru_explosm">Цианистый калий и Счастье</a></li>
					<li><a href="http://xkcd.com/">A webcomic of romance, sarcasm, math and language</a></li>
					<li><a href="http://aviasales.ru">Спецпредложения по авиабилетам</a></li>
				</ul>
			</li>
			<li>
				<h2>Бесполезные ссылки</h2>
				<ul>
					<li><a href="http://usu.ru">УрГУ</a></li>
					<li><a href="http://math.usu.ru">Мат-мех</a></li>
				</ul>
			</li>
		</ul>
	</div>
	<!-- end sidebar -->
	<div style="clear: both;">&nbsp;</div>
</div>
<!-- end page -->
</div>
<!-- start footer -->
<div id="footer" align="center">
	Посещений с этого IP ( <%=Request.UserHostAddress %>): <%= CurrentClientHits %><br/>
	Всего посещений: <%= TotalHits %>
	<p id="legal">(С) 2009. Вадим Канторов </p>
	Username: <%=Environment.UserName %>
</div>
<!-- end footer -->
</form>
</body>
</html>
