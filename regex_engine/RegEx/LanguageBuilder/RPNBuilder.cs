using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace RegEx
{
	public class RPNBuilder
	{
		public RPNBuilder(string input)
		{
			this.input = input;
		}

		public string GetRPN()
		{
			var stack = new Stack<char>();
			foreach (var ch in InputWithConcatenation)
			{
				if (!SpecialChars.IsSpecialChar(ch))
				{
					AddToOutput(ch);
					continue;
				}
				switch (ch)
				{
					case SpecialChars.OpenParenthesis:
						stack.Push(ch);
						break;
					case SpecialChars.CloseParenthesis:
						AddToOutput(DestructiveTakeWhile(stack, c => c != '('));
						stack.Pop();
						break;
					default:
						AddToOutput(DestructiveTakeWhile(stack, c => priorities[ch] <= priorities[c]));
						if (SpecialChars.IsPostfixOperator(ch))
						{
							AddToOutput(ch);
						}
						else
						{
							stack.Push(ch);
						}
						break;
				}
			}
			AddToOutput(DestructiveTakeWhile(stack, c => true));
			return output.ToString();
		}

		private IEnumerable<char> DestructiveTakeWhile(Stack<char> stack, Predicate<char> predicate)
		{
			while (stack.Count > 0 && predicate(stack.Peek()))
				yield return stack.Pop();
		}

		private void AddToOutput(char ch)
		{
			output.Append(ch);
		}

		private void AddToOutput(IEnumerable<char> sequence)
		{
			foreach (var ch in sequence)
				output.Append(ch);
		}

		private IEnumerable<char> InputWithConcatenation
		{
			get 
			{
				char previous = char.MaxValue;
				foreach (var current in input)
				{
					if ((previous != char.MaxValue) && NeedConcatenation(previous, current))
					{
						yield return '&';
					}
					yield return current;
					previous = current;
				}
			}
		}

		private bool NeedConcatenation(char previous, char current)
		{
			Predicate<char> special = SpecialChars.IsSpecialChar;
			Func<char, char, bool>[] cases = {
				(a, b) =>
					(!special(a) || (a == SpecialChars.CloseParenthesis)) &&
					(!special(b) || (b == SpecialChars.OpenParenthesis)), // )( -> )&(, )a -> )&a, a( -> a&(, ab -> a&b
				(a, b) => (a == SpecialChars.Iteration) && (!special(b) || (b == SpecialChars.OpenParenthesis)), // *a -> *&a, *( -> *&(
			};
			return cases.Any(f => f(previous, current));
		}

		private readonly string input;
		private readonly StringBuilder output = new StringBuilder();
		private static readonly IDictionary<char, int> priorities = new Dictionary<char, int>
		{
			{ SpecialChars.Iteration, 3 },
			{ SpecialChars.Concatenation, 2 },
			{ SpecialChars.Alternation, 1 },
			{ SpecialChars.OpenParenthesis, 0 },
		};
	}
}