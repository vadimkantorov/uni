using NUnit.Framework;

namespace RegEx.Tests.UnitTests
{
	[TestFixture]
	public class RPNBuilder_Test
	{
		[TestCase("", Result = "")]
		[TestCase("ab", Result = "ab&")]
		[TestCase("abcdef", Result = "ab&c&d&e&f&")]
		[TestCase("(a|b)*abb", Result = "ab|*a&b&b&")]
		[TestCase("ef(a|b)*abb", Result = "ef&ab|*&a&b&b&")]
		[TestCase("a(c)b", Result = "ac&b&")]
		[TestCase("(0|1|11)*|b*d|ca", Result = "01|11&|*b*d&|ca&|")]
		[TestCase("aa*|bb*", Result = "aa*&bb*&|")]
		public string BuildRPN(string regexp)
		{
			return new RPNBuilder(regexp).GetRPN();
		}
	}
}