using System.Collections.Generic;

namespace RegEx
{
	public class ConstantExpression : Expression
	{
		public ConstantExpression(char character)
		{
			Character = character;
		}

		public char Character { get; private set; }
		
		public override IEnumerator<Expression> GetEnumerator()
		{
			yield break;
		}
	}
}