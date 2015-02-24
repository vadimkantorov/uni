using System;

namespace Okulobot
{
	public class RotateTactics : ITactics
	{
		private readonly WayPoint current;
		private readonly double angle;
		private readonly int? initialTimes;
		private int? timesLeft;

		public RotateTactics(WayPoint current, double angle, int? initialTimes)
		{
			this.current = current;
			this.angle = angle;
			this.initialTimes = initialTimes;

			Priority = Priorities.RotateTactics;
		}

		public override bool ShouldBeStarted(State state)
		{
			return state.Fighter.Coordinates == current.Coordinates;
		}

		public override bool CanMove(State state)
		{
			return timesLeft == null || timesLeft > 0;
		}

		public override void Start(State state, ITactics tactics)
		{
			timesLeft = initialTimes;
		}

		public override MoveInfo MakeMove(State state)
		{
			//System.Diagnostics.Debugger.Break();
			
			current.Angle += angle;
			
			if (timesLeft != null)
				timesLeft--;

			return new MoveInfo(current.Coordinates, current.Angle);
		}
	}
}