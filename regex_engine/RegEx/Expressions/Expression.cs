using System.Collections;
using System.Collections.Generic;

namespace RegEx
{
	public abstract class Expression : IEnumerable<Expression>
	{
		public abstract IEnumerator<Expression> GetEnumerator();

		IEnumerator IEnumerable.GetEnumerator()
		{
			return GetEnumerator();
		}
	}
}