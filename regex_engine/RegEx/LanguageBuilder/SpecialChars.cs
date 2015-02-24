using System.Linq;
using System.Collections.Generic;

namespace RegEx
{
	public static class SpecialChars
	{
		public const char Iteration = '*';
		public const char Alternation = '|';
		public const char Concatenation = '&';
		public const char OpenParenthesis = '(';
		public const char CloseParenthesis = ')';

		public static bool IsSpecialChar(char ch)
		{
			return All.Contains(ch);
		}

		public static bool IsPostfixOperator(char ch)
		{
			return ch == Iteration;
		}

		private static readonly IEnumerable<char> All = new[]
		{
			Iteration,
			Alternation,
			Concatenation,
			OpenParenthesis,
			CloseParenthesis
		};
	}
}