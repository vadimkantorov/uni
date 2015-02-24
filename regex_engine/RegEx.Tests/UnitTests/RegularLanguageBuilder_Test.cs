using System;
using System.Linq;
using System.Collections.Generic;
using NUnit.Framework;

namespace RegEx.Tests.UnitTests
{
	[TestFixture]
	public class RegularLanguageBuilder_Test
	{
		public class TestCase
		{
			public readonly Expression Root;
			public readonly LanguageWordGenerator InLanguage;
			public readonly Mangler Mangler;

			public TestCase(Expression root, LanguageWordGenerator inLanguage, Mangler mangler)
			{
				Root = root;
				InLanguage = inLanguage;
				Mangler = mangler;
			}
		}

		[TestCaseSource("Languages")]
		public void Test(TestCase testCase)
		{
			const int attempts = 1000;
			var builder = new RegularLanguageBuilder(testCase.Root);
			var dfa = builder.BuildLanguage();
			var recognizer = new LanguageRecognizer<RegularLanguageState>(dfa);
			for (int i = 0; i < attempts; i++)
			{
				string word = testCase.InLanguage();
				Assert.IsTrue(recognizer.Accepts(word), "Word {0} is NOT in language, but should be", word);
			}
			for (int i = 0; i < attempts; i++)
			{
				string word = testCase.Mangler(testCase.InLanguage());
				Assert.IsFalse(recognizer.Accepts(word), "Word {0} is in language, but should NOT be", word);
			}
		}

		[SetUp]
		public void Init()
		{
			rnd = new Random();
		}

		public delegate string LanguageWordGenerator();
		public delegate string Mangler(string word);

		// Each test case is specified by:
		//  1. An ExpressionTree (use CreateTree() to generate a tree with an ad-hoc DSL).
		//  2. A random language words generator.
		//  3. A mangler, which takes a valid language word and messes it up so that it becomes invalid.
		private IEnumerable<TestCase> Languages()
		{
			const int n = 100;
			return new[]
			       	{
			       		// (aa)*a
			       		new TestCase(CreateTree("CCICaaa#"),
			       		             () =>
			       		             	{
			       		             		var res = "a";
			       		             		for (int i = 0; i < rnd.Next(n); i++)
			       		             		{
			       		             			res += "aa";
			       		             		}
			       		             		return res;
			       		             	},
			       		             str => str + "a"),
			       		// c(a|bb)*d
			       		new TestCase(CreateTree("CCCcIAaCbbd#"),
			       		             () =>
			       		             	{
			       		             		var res = "";
			       		             		for (int i = 0; i < n; i++)
			       		             		{
			       		             			if (rnd.Next(2) == 0)
			       		             			{
			       		             				res += "a";
			       		             			}
			       		             			else
			       		             			{
			       		             				res += "bb";
			       		             			}
			       		             		}
			       		             		return "c" + res + "d";
			       		             	},
			       		             str =>
			       		             	{
			       		             		if (rnd.Next(2) == 0)
			       		             		{
			       		             			str += "e";
			       		             		}
			       		             		else
			       		             		{
			       		             			var index = rnd.Next(0, str.Length - 1);
			       		             			str = str.Insert(index, "b"); // this will always corrupt the word
			       		             		}
			       		             		return str;
			       		             	}),
			       	};
		}

		private Expression CreateTree(string expr)
		{
			int i = 0;
			return CreateTreeRec(expr, ref i);
		}

		private Expression CreateTreeRec(string expr, ref int i)
		{
			char ch = expr[i++];
			if (char.IsLower(ch))
			{
				return new ConstantExpression(ch);
			}
			if (ch == '#')
			{
				return new TerminalExpression();
			}
			if (ch == 'C')
			{
				var right = CreateTreeRec(expr, ref i);
				var left = CreateTreeRec(expr, ref i);
				return new ConcatenationExpression(left, right);
			}
			if (ch == 'A')
			{
				var right = CreateTreeRec(expr, ref i);
				var left = CreateTreeRec(expr, ref i);
				return new AlternationExpression(left, right);
			}
			if (ch == 'I')
			{
				return new IterationExpression(CreateTreeRec(expr, ref i));
			}
			throw new Exception("CreateTreeRec internal failure - blame the author of this test");
		}

		private static Random rnd;
	}
}