using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;

namespace IndexingService
{
	public class LowercasingTokenizer : RegisterRespectingTokenizer
	{
		public LowercasingTokenizer(string text) : base(text)
		{}

		public override IEnumerable<string> Tokenize()
		{
			return base.Tokenize().Select(x => x.ToLower());
		}
	}
}