using System.Collections;
using System.Collections.Generic;
using NUnit.Framework;

namespace RegEx.Tests.IntegrationTests
{
	[TestFixture]
	public class TestEverything
	{
		[TestCaseSource("Regexps")]
		public bool Test(string regex, string word)
		{
			var parser = new RegularExpressionASTBuilder(regex);
			var exprTree = parser.BuildExpressionTree();

			var regLanBuilder = new RegularLanguageBuilder(exprTree);
			var lang = regLanBuilder.BuildLanguage();
			var recognizer = new LanguageRecognizer<RegularLanguageState>(lang);
			return recognizer.Accepts(word);
		}

		private IEnumerable<TestCaseData> Regexps()
		{
			foreach (var testCase in testCases)
			{
				foreach (var valid in testCase.ValidWords)
				{
					yield return new TestCaseData(testCase.Regexp, valid).Returns(true);
				}
				foreach (var valid in testCase.InvalidWords)
				{
					yield return new TestCaseData(testCase.Regexp, valid).Returns(false);
				}
			}
		}

		private class TestCase
		{
			public string Regexp { get; set; }
			public string[] ValidWords { get; set; }
			public string[] InvalidWords { get; set; }
		}

		private static readonly TestCase[] testCases = new[]
		{
			new TestCase
			{
				Regexp = "(0|1((0(01|111)*(00|110))*(1|0(01|111)*10))(01*0(1(10|000)*(11|0(000)*01))*(0|1(10|000)*0(000)*1))*1)*",
				ValidWords = new [] { "111", "1101001", "10101111111011011000001", "0111", "0" },
				InvalidWords = new [] { "110", "10111001101", "011", "1111111" }
			},

			new TestCase
			{
				Regexp = "(2(((0|1|2|3|4)(0|1|2|3|4|5|6|7|8|9))|(5(0|1|2|3|4|5))))|(1(0|1|2|3|4|5|6|7|8|9)(0|1|2|3|4|5|6|7|8|9))|((1|2|3|4|5|6|7|8|9)(0|1|2|3|4|5|6|7|8|9))|((0|1|2|3|4|5|6|7|8|9))",
				ValidWords = new [] {"0", "255", "13", "120"},
				InvalidWords = new [] {"256", "-1", "2222", "999", "000"}
			}
		};
	}
}