using System;

namespace Okulobot
{
	public class Unit
	{
		public Unit(char type)
		{
			Properties = new UnitProperties(type);
		}

		public Point Coordinates { get; set; }

		public int Health { get; set; }

		public UnitProperties Properties { get; private set; }
	}
}