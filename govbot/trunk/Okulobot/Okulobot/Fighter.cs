using System;

namespace Okulobot
{
	public class Fighter : Unit
	{
		public ILog Log { get; private set; }

		public Fighter(char type, Strategy strategy, ILog log)
			: base(type)
		{
			Log = log;
            Strategy = strategy;
			Health = 100;
			Mana = 20;

			if(Strategy != null)
				Strategy.Log = Log;
		}

		public MoveInfo MakeMove(State state)
		{
			return Strategy.MakeMove(state);
		}

		public bool IsAlive { get; set; }

		public int Mana { get; set; }

		public double Angle { get; set; }

		public Strategy Strategy { get; set; }

		public bool CanShoot { get { return Mana >= Properties.ShotCost; } }
	}
}