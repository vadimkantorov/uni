using System.Collections.Generic;

namespace RegEx
{
	public class UnaryExpression : Expression
	{
		public Expression Inner { get; protected set; }
		
		public override IEnumerator<Expression> GetEnumerator()
		{
			yield return Inner;
		}
	}
}