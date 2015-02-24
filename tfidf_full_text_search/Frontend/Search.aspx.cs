using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using IndexingService;

namespace Frontend
{
	public partial class SearchPage : Page
	{
		protected void Page_Load(object sender, EventArgs e)
		{
			rawQuery = Request.QueryString["text"];
			if(rawQuery == null)
				return;
			int page;
			int.TryParse(Request.QueryString["page"], out page);
			var rankResults = Request.QueryString["rank"] != null;

			text.Value = rawQuery;
			rank.Checked = rankResults;
			var paradigms = new List<Paradigm[]>();
			var selectedParadigms = new List<int>();
			terms = new List<string>();
			var parsedQuery = RawQueryParser.Parse(rawQuery, paradigms, selectedParadigms, terms);
			IEnumerable<Hit> hits = Search(parsedQuery, page, rankResults);
			if(hits.Count() != 0)
			{
				rptSearchResults.Visible = true;
				rptSearchResults.DataSource = hits;
				rptSearchResults.DataBind();
				BuildParadigms(paradigms, selectedParadigms);
			}
			else
				lblNoData.Visible = true;
		}

		private void BuildParadigms(List<Paradigm[]> paradigms, List<int> selectedParadigms)
		{
			string text = "";
			for(int i = 0; i < terms.Count; i++)
			{
				text += string.Format("<h4>{0}</h4>", HttpUtility.HtmlEncode(terms[i]));
				text += "<table style='border: 1px solid black; border-collapse: collapse'>";
				text += "<tr><th>Основа</th><th>Часть речи</th><th>Парадигма</th></tr>";
				for(int j = 0; j < paradigms[i].Length; j++)
				{
					string style = j == selectedParadigms[i] ? " style='background-color: #ffc'" : "";
					var p = paradigms[i][j];
					text += string.Format("<tr{0}><td><b>{1}</b></td><td><b>{2}</b></td><td>{3}</td></tr>", style, p.Lemma, p.PartOfSpeech, string.Join(" ", p.Inflections.ToArray()));
				}
				text += "</table>";
			}
			
			lblParadigms.Text = text;
		}

		protected string rawQuery;
		protected List<string> terms;

		private IEnumerable<Hit> Search(ParsedQuery query, int page, bool rankResults)
		{
			var results = Global.Index.GetPostingsByQuery(query);
			if(rankResults)
				results = results.Rank();
			int resultCount = results.Count();
			int pageCount = resultCount / PageSize + 1;
			if(resultCount % PageSize == 0)
				pageCount--;
			if(page >= pageCount)
				page = 0;
			IEnumerable<Hit> pagedResults = results.Skip(PageSize*page).Take(PageSize).Select(x => ConvertDocumentIdToHit(x, query));
			if(pagedResults.Count() > 0)
			{
				lblPage.Visible = hrefPrev.Visible = hrefNext.Visible = true;
				int firstResult = page * PageSize + 1;
				int lastResult = firstResult + pagedResults.Count() - 1;
				listResults.Attributes["start"] = firstResult.ToString();
				lblPage.Text = string.Format("Показываем результаты <b>{0}</b>&mdash;<b>{1}</b> из <b>{2}</b>", firstResult, lastResult, resultCount);
				if(page > 0)
					BuildUrlForPage(page - 1, hrefPrev);
				if(page + 1 < pageCount)
					BuildUrlForPage(page + 1, hrefNext);
			}
			return pagedResults;
		}

		private void BuildUrlForPage(int newPage, HyperLink link)
		{
			link.Enabled = true;
			var newQueryString = Request.QueryString.Keys
				.OfType<string>()
				.Where(x => x != "page")
				.Select(x => new {Key = x, Value = Request.QueryString[x]})
				.Concat(new[] {new {Key = "page", Value = newPage.ToString()}})
				.Select(x => x.Key + "=" + HttpUtility.UrlEncode(x.Value))
				.ToArray();
			link.NavigateUrl = "/Search.aspx?" + string.Join("&", newQueryString);
		}

		private Hit ConvertDocumentIdToHit(WeightedPosting posting, ParsedQuery query)
		{
			string formattedDocumentId = string.Format("{0:D6}", posting.PostingId);
			var document = new Document(formattedDocumentId);
			var snippet = new SnippetGenerator(query, document).GenerateSnippet();
			return
				new Hit
					{
						DocumentId = formattedDocumentId,
						Description = GetDescription(posting.PostingId, string.Format("№{0}", formattedDocumentId)),
						DebugInfo =
							new[]
								{
									string.Format("tf-idf = {0:F5} = {1}", Ranker.GetTfIdf(posting), FormatTfIdf(posting)),
									string.Format("ExactWordMatchCount = {0}", posting.ExactWordMatchCount)
								},
						Snippet = string.Join("&nbsp;&hellip;&nbsp;", snippet.Skip(1).ToArray()),
						Title = snippet.First(),
					};
		}

		private static string GetDescription(int postingId, string defaultDescription)
		{
			if(!Global.PostingUrls.ContainsKey(postingId))
				return defaultDescription;
			return Global.PostingUrls[postingId];
		}
		
		private string FormatTfIdf(WeightedPosting posting)
		{
			IEnumerable<string> summands = Enumerable
				.Range(0, posting.TFs.Length)
				.Select(x => string.Format("{0:F5}&times;{1:F5} ({2})", posting.TFs[x], posting.IDFs[x], terms[x]));
			return string.Join(" + ", summands.ToArray());
		}

		private const int PageSize = 10;
	}
}