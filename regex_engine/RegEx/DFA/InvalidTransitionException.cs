using System;

namespace RegEx
{
	class InvalidTransitionException : Exception
	{
		public InvalidTransitionException(object state, object via) : base(FormatMessage(state, via))
		{
		}

		static string FormatMessage(object state, object via)
		{
			return string.Format(
				"Cannot find a transition edge from state '{0}' with character '{1}'",
				state, via);
		}
	}
}