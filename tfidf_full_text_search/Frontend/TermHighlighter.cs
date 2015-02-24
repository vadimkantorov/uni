using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;

namespace Frontend
{
	public class TermHighlighter
	{
		public TermHighlighter(string[][] termsToHighlight, string openingTag, string closingTag)
		{
			this.termsToHighlight = termsToHighlight;
			this.openingTag = openingTag;
			this.closingTag = closingTag;
			var termUnion = string.Join("|", termsToHighlight.SelectMany(x => x).Select<string, string>(Regex.Escape).ToArray());
			termsRegex = new Regex(string.Format(@"\b(?<term>{0})\b", termUnion), RegexOptions.IgnoreCase);
		}

		public string Highlight(string text)
		{
			return Highlight(text, null);
		}

		public string Highlight(string text, HashSet<int> highlightedTerms)
		{
			return termsRegex.Replace(text, x =>
				{
					if(highlightedTerms != null)
						highlightedTerms.Add(Array.FindIndex(termsToHighlight, termSet => Array.IndexOf(termSet, x.Value.ToLower()) >= 0));
					return string.Format("{0}{1}{2}", openingTag, x, closingTag);
				});
		}

		private readonly string[][] termsToHighlight;
		private readonly string openingTag;
		private readonly string closingTag;
		private readonly Regex termsRegex;
	}
}