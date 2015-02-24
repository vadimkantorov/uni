using System.Collections.Generic;

namespace RegEx
{
	public class TerminalExpression : Expression
	{
		public override IEnumerator<Expression> GetEnumerator()
		{
			yield break;
		}
	}
}