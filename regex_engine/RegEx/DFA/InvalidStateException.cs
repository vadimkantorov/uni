using System;

namespace RegEx
{
	class InvalidStateException : Exception
	{
		public InvalidStateException(object state) : base(FormatMessage(state))
		{
		}

		static string FormatMessage(object state)
		{
			return string.Format(
				"The transition table doesn't contain state '{0}'",
				state);
		}
	}
}