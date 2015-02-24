using System.Collections.Generic;

namespace RegEx
{
	public class BinaryExpression : Expression
	{
		public Expression Left { get; protected set; }

		public Expression Right { get; protected set; }

		public BinaryExpression(Expression right, Expression left)
		{
			Left = left;
			Right = right;
		}

		public override IEnumerator<Expression> GetEnumerator()
		{
			yield return Left;
			yield return Right;
		}
	}
}