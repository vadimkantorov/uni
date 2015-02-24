using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace theGame
{
	class GameParseException:Exception
	{
		public GameParseException()
			:base()
		{
		}

		public GameParseException(String message)
			: base(message)
		{
		}
	}
}
