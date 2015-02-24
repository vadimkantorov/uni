using System;
using System.Collections.Generic;
using System.Linq;

namespace Okulobot
{
	public class Strategy
	{
		public delegate void MoveMade(MoveInfo info, ITactics source);

		private void InvokeOnMove(MoveInfo info, ITactics tactics)
		{
			MoveMade move = OnMove;
			if (move != null) move(info, tactics);
		}

		public Strategy()
		{
			AvailableTactics = new List<ITactics>();
		}

		public MoveInfo MakeMove(State state)
		{
			var bestTactics = AvailableTactics
				.Where(x => x.ShouldBeStarted(state))
				.OrderBy(x => x.Priority)
				.Last();

			if (CurrentTactics == null
			    || CurrentTactics.Priority < bestTactics.Priority
			    || !CurrentTactics.CanMove(state))
			{
				//CurrentTactics = bestTactics;
				bestTactics.Start(state, CurrentTactics);
				CurrentTactics = bestTactics;

				Log.Write("Выбрана новая тактика: " + bestTactics.GetType().Name);
			}


			var move = CurrentTactics.MakeMove(state);
			if (move != null)
			{
				InvokeOnMove(move, CurrentTactics);

				Log.Write("Сделан ход: " + move);
			}
			return move;
		}

		public event MoveMade OnMove;
		private ITactics CurrentTactics { get; set; }
        protected List<ITactics> AvailableTactics { get; private set; }

		public ILog Log { get; set; }
	}
}