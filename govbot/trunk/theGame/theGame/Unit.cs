using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace theGame
{
    /// <summary>
    /// Используется классом Adaptor
    /// </summary>
	class Unit
	{
		public int x { get; private set; }
		public int y { get; private set; }
		public int team { get; private set; }
		public char clazz { get; private set; }
		public int health { get; private set; }

		public Unit(int x, int y, int team, char clazz, int health)
		{
			this.x = x;
			this.y = y;
			this.team = team;
			this.clazz = clazz;
			this.health = health;
		}
	}
}
