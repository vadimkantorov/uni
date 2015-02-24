using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;

namespace Summarizer
{
	class LineTokenizer
	{
		public LineTokenizer(string fileName)
		{
			text = File.ReadAllText(fileName, Encoding.UTF8);
		}

		public virtual IEnumerable<string> Tokenize()
		{
			return text.Split(new[]{'\n'}, StringSplitOptions.RemoveEmptyEntries).Skip(2);
		}

		private readonly string text;
	}
}