using System;
using System.Linq;
using System.Collections.Generic;

namespace Okulobot
{
	public class CycleTactics : ITactics
	{
		public CycleTactics()
		{
			Priority = Priorities.CycleTactics;
		}
		
		private int index;
		private readonly List<ITactics> tactics = new List<ITactics>();

		public void AddInnerTactics(ITactics t)
		{
			tactics.Add(t);
		}

		public override bool ShouldBeStarted(State state)
		{
			return tactics.Any(x => x.ShouldBeStarted(state));
		}

		public override bool CanMove(State state)
		{
			return true;
			//return tactics[index].CanMove(state);
		}

		//TODO: remove dirty hack
		public override void Start(State state, ITactics ts)
		{
			index = tactics.FindIndex(x => x.ShouldBeStarted(state));
			if (index == -1)
				index = 0;
			tactics[index].Start(state, ts);
		}

		public override MoveInfo MakeMove(State state)
		{
			//System.Diagnostics.Debugger.Break();
			if (!tactics[index].CanMove(state))
			{
				index = (index + 1)%tactics.Count;
				tactics[index].Start(state, null);
			}

			return tactics[index].MakeMove(state);
		}
	}
}