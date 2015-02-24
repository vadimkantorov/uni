namespace RegEx
{
	public class ConcatenationExpression : BinaryExpression
	{
		public ConcatenationExpression(Expression left, Expression right) : base(left, right)
		{ }
	}
}