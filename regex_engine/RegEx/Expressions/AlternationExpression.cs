namespace RegEx
{
	public class AlternationExpression : BinaryExpression
	{
		public AlternationExpression(Expression left, Expression right) : base(left, right)
		{ }
	}
}