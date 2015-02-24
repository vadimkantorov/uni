using System;
using System.Collections.Generic;
using System.Text.RegularExpressions;

namespace RegEx
{
	public class RegularExpressionASTBuilder
	{
		readonly string rpn;

		public RegularExpressionASTBuilder(string regex)
		{
			rpn = new RPNBuilder(regex).GetRPN();
		}

		public Expression BuildExpressionTree()
		{
			const string InsufficientAmountOfOperands = "The amount of operands is insufficient";

			try
			{
				var exprStack = new Stack<Expression>();
				foreach (char c in rpn)
				{
					Expression expr = null;
					switch (c)
					{
						case '*':
							if (exprStack.Count < 1)
								throw new InvalidOperationException(InsufficientAmountOfOperands);

							expr = new IterationExpression(exprStack.Pop());
							break;

						case '|':
							if (exprStack.Count < 2)
								throw new InvalidOperationException(InsufficientAmountOfOperands);

							expr = new AlternationExpression(exprStack.Pop(), exprStack.Pop());
							break;

						case '&':
							if (exprStack.Count < 2)
								throw new InvalidOperationException(InsufficientAmountOfOperands);

							expr = new ConcatenationExpression(exprStack.Pop(), exprStack.Pop());
							break;

						default:
							expr = new ConstantExpression(c);
							break;
					}
					exprStack.Push(expr);
				}

				if (exprStack.Count != 1)
					throw new InvalidOperationException(InsufficientAmountOfOperands);

				return exprStack.Pop();
			}
			catch (InvalidOperationException ex)
			{
				throw new FormatException("The regular expression is incorrect", ex);
			}
		}
	}
}