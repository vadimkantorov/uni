<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="Search.aspx.cs" ValidateRequest="false" Inherits="Frontend.SearchPage" %>

<!DOCTYPE html PUBLIC "-//W3C//DTD XHTML 1.0 Transitional//EN" "http://www.w3.org/TR/xhtml1/DTD/xhtml1-transitional.dtd">
<html xmlns="http://www.w3.org/1999/xhtml">
<head runat="server">
	<title>Поиск по новостям</title>
</head>
<body>
	<form action="Search.aspx" method="get">
	<div>
		<input name="text" size="50" maxlength="300" style="width: 70%" runat="server" id="text" />
		<input type="submit" value="Найти" />
	</div>
	<div>
		<input type="checkbox" name="rank" value="rank" checked="checked" runat="server" id="rank"/>Ранжировать результаты
	</div>
	</form>
	<span style="float:right; font-size: larger"><asp:Literal id="lblPage" runat="server" Visible="false" /></span>
	<asp:Literal runat="server" ID="lblParadigms"/>
	<ol runat="server" id="listResults">
		<asp:Repeater id="rptSearchResults" runat="server" Visible="false">
			<ItemTemplate>
			<li>
				<div><a href="/ViewDocument.aspx?id=<%# Eval("DocumentId") %>&sourceQuery=<%# HttpUtility.UrlEncode(rawQuery) %>"><%# Eval("Title") %></a></div>
				<div><%# Eval("Snippet") %></div>
				<div style="color: green"><%# Eval("Description") %><span style='float:right; color:red; font-size: smaller'><%# string.Join("<br/>", (string[])Eval("DebugInfo")) %></span></div>
			</li>
			</ItemTemplate>
		</asp:Repeater>
	</ol>
	<asp:Literal id="lblNoData" runat="server" Visible="false"><big>По вашему запросу ничего не найдено</big></asp:Literal>
	<div>
	<asp:HyperLink id="hrefPrev" runat="server" Enabled="false" Visible="false">&larr;Назад</asp:HyperLink>
	&nbsp;
	<asp:HyperLink id="hrefNext" runat="server" Enabled="false" Visible="false">Ещё записей!&rarr;</asp:HyperLink>
	</div>
</body>
</html>
