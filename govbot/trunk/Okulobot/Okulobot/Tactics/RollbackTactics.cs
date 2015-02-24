using System;
using System.Collections.Generic;
using Okulobot.Tactics;

namespace Okulobot
{
	public class RollbackTactics : ITactics
	{
		private readonly Stack<MoveInfo> history = new Stack<MoveInfo>();

		public RollbackTactics(Strategy strategy)
		{
			strategy.OnMove += AddToHistory;

			Priority = Priorities.RollbackTactics;
		}

		private void AddToHistory(MoveInfo info, ITactics source)
		{
			if (source is RollbackTactics)
				return;

			if(history.Count > 0 && history.Peek().Destination == info.Destination)
				return;
			
			if(source is DefaultTactics || source is DeadlyTactics)
				history.Clear();
			
			history.Push(info);
		}

		public override bool ShouldBeStarted(State state)
		{
			return CanMove(state);
		}

		public override bool CanMove(State state)
		{
			return history.Count > 1;
		}

		public override MoveInfo MakeMove(State state)
		{
			var top = history.Pop();
			double angle = top.Angle;
			
			if(history.Count > 0)
			{
				var prev = history.Peek();
				angle = AngleUtils.GetDirection(prev.Destination, top.Destination);
			}

			return new MoveInfo
			(
				top.Destination,
				angle
			);
		}
	}
}