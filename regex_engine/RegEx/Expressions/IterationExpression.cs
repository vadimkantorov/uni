namespace RegEx
{
	public class IterationExpression : UnaryExpression
	{
		public IterationExpression(Expression expr)
		{
			Inner = expr;
		}
	}
}