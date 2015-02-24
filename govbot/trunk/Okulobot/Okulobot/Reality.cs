using System.Collections.Generic;

namespace Okulobot
{
	public class Reality
	{
		public Fighter ActiveFighter { get; set; }

		public List<Unit> Friends { get; set; }

		public List<Unit> Enemies { get; set; }

		public List<string> Messages { get; set; }

		public Reality()
		{
			Friends = new List<Unit>();
			Enemies = new List<Unit>();
			Messages = new List<string>();
		}

	}
}