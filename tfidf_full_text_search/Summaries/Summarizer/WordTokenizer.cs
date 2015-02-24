using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;

namespace Summarizer
{
	class WordTokenizer
	{
		public WordTokenizer(string text) : this(new[]{text})
		{
		}

		public WordTokenizer(IEnumerable<string> text)
		{
			this.text = text;
		}

		public IEnumerable<string> Tokenize()
		{
			return text.Select(x => x.Substring(2)).SelectMany(x => splitRegex.Split(x));
		}

		readonly IEnumerable<string> text;
		private static readonly Regex splitRegex = new Regex(@"[^\p{L}\p{Nd}-]+", RegexOptions.Compiled);
	}
}