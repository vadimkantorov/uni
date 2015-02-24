using System;

namespace Okulobot.Tactics
{
	class DeadlyTactics : ITactics
	{
		public DeadlyTactics()
		{
			Priority = Priorities.DeadlyTactics;
		}
		
		public override bool ShouldBeStarted(State state)
		{
			return CanMove(state);
		}

		public override void Start(State state, ITactics prev)
		{
			prev.Priority -= .1;
		}

		public override bool CanMove(State state)
		{
			return state.Fighter.IsAlive == false;
		}

		public override MoveInfo MakeMove(State state)
		{
			return null;
		}
	}
}
