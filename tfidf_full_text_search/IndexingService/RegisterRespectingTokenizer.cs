using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;

namespace IndexingService
{
	public class RegisterRespectingTokenizer
	{
		public RegisterRespectingTokenizer(string text)
		{
			this.text = text;
		}

		public virtual IEnumerable<string> Tokenize()
		{
			return splitRegex.Split(text).Where(x => x != "");
		}

		private readonly string text;
		private static readonly Regex splitRegex = new Regex(@"[^\p{L}\p{Nd}-]+", RegexOptions.Compiled);
	}
}