using System;
using System.Collections.Generic;
using System.Linq;
using IndexingService;

namespace Frontend
{
	public class SnippetGenerator
	{
		public SnippetGenerator(ParsedQuery query, Document document)
		{
			this.query = query;
			this.document = document;
		}

		public string[] GenerateSnippet()
		{
			var highlightedTerms = new HashSet<int>();
			var highlighter = new TermHighlighter(query.TermsWithSynonyms, "<b>", "</b>");
			var snippet = new List<string> {highlighter.Highlight(document.Title, highlightedTerms)};
			string lastLineWithTerm = null;
			foreach(string line in document.Content)
			{
				int termCountBeforeHighlighting = highlightedTerms.Count;
				string highlightedLine = highlighter.Highlight(line, highlightedTerms);
				if(highlightedTerms.Count > termCountBeforeHighlighting && termCountBeforeHighlighting != query.TermsWithSynonyms.Length)
					snippet.Add(highlightedLine);
				if(line != highlightedLine && lastLineWithTerm == null)
					lastLineWithTerm = highlightedLine;
			}
			if(snippet.Count == 1)
				snippet.Add(lastLineWithTerm ?? highlighter.Highlight(document.Content.First()));
			return snippet.ToArray();
		}

		private readonly ParsedQuery query;
		private readonly Document document;
	}
}