<%@ Control Language="c#" AutoEventWireup="false" Codebehind="guestbook.ascx.cs" Inherits="UsuSite.PageModules.guestbook" TargetSchema="http://schemas.microsoft.com/intellisense/ie5"%>
<%@ Register TagPrefix="recaptcha" Namespace="Recaptcha" Assembly="Recaptcha" %>
<table border="1" style="WIDTH: 100%">
	<tr>
		<td>Имя</td>
		<td><asp:TextBox Runat="server" ID="tbName" MaxLength="30" /></td>
		<td style="WIDTH:30%">
			<asp:RequiredFieldValidator Runat="server" ControlToValidate="tbName" ErrorMessage="Имя обязательно для заполнения"
				id="RequiredFieldValidator1" />
		</td>
	</tr>
	<tr>
		<td>Эл. почта</td>
		<td><asp:TextBox Runat="server" ID="tbEmail" MaxLength="30" /></td>
		<td style="WIDTH:30%">
			<asp:RegularExpressionValidator runat="server" ControlToValidate="tbEmail" ErrorMessage="Адрес электронной почты должен быть корректным"
				ValidationExpression="^[a-zA-Z]+(([\'\,\.\- ][a-zA-Z ])?[a-zA-Z]*)*\s+<(\w[-._\w]*\w@\w[-._\w]*\w\.\w{2,3})>$|^(\w[-._\w]*\w@\w[-._\w]*\w\.\w{2,3})$"
				id="RegularExpressionValidator1" />
		</td>
	</tr>
	<tr>
		<td>Сообщение</td>
		<td><asp:TextBox Runat="server" ID="tbMessage" TextMode="MultiLine" Rows="10" Width="100%" MaxLength="10000" /></td>
		<td style="WIDTH:30%"></td>
	</tr>
</table>
<div align="center">
	<recaptcha:RecaptchaValidator ID="recaptcha" runat="server" ControlToValidate="tbMessage" Theme="White"
		PublicKey="6LdB5wYAAAAAAGQFgOnuWOgDza7_JraLaujVl6wU" PrivateKey="6LdB5wYAAAAAAFq7va4ssF8lCETd4b4ECh6tIPqj" />
	<asp:Button Runat="server" Text="Отправить" ID="btnSubmit" />
</div>
<asp:DataList Runat="server" ID="dlEntries">
	<ItemTemplate>
		<h3><a href='mailto:<%#Server.HtmlEncode((string)DataBinder.Eval(Container.DataItem, "Email"))%>'><%#Server.HtmlEncode((string)DataBinder.Eval(Container.DataItem, "Name"))%></a></h3>
		написал (<%#DataBinder.Eval(Container.DataItem, "SubmittedAt", "{0:MM.dd.yyyy HH:mm}")%>):
		<p>
			<%#Server.HtmlEncode((string)DataBinder.Eval(Container.DataItem, "Message"))%>
		</p>
	</ItemTemplate>
</asp:DataList>
