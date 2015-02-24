using System;
using System.Linq;

using IndexingService;

namespace Frontend
{
	public partial class ViewDocument : System.Web.UI.Page
	{
		protected void Page_Load(object sender, EventArgs e)
		{
			string documentId = Request.QueryString["id"];
			if(documentId.Any(x => !char.IsDigit(x)))
				GoAway();
			Document document;
			try
			{
				document = new Document(documentId);
			}
			catch
			{
				GoAway();
				return;
			}
			string sourceQuery = Request.QueryString["sourceQuery"];
			var highlighter = new TermHighlighter(
				RawQueryParser.Parse(sourceQuery, null, null, null).TermsWithSynonyms,
				"<span style='background-color: yellow'>",
				"</span>");
			if(!string.IsNullOrEmpty(sourceQuery))
				document.Content = document.Content.Select(x => highlighter.Highlight(x));
			string finalText = string.Join("", document.Content.Select(x => string.Format("<div>{0}</div>", x)).ToArray());
			lblOriginalText.Text = string.Format("<h1>{0}</h1><div>{1}</div>", highlighter.Highlight(document.Title), finalText);
		}

		private void GoAway()
		{
			Response.Redirect("~/Search.aspx");
		}
	}
}
