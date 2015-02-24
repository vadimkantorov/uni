using System.Linq;
using System.Collections.Generic;

namespace Okulobot
{
	public class LinearTactics : ITactics
	{
		public LinearTactics()
		{
			Priority = Priorities.LinearTactics;
		}
		
		private int index;
		public List<WayPoint> WayPoints { get; private set; }

		public LinearTactics(IEnumerable<WayPoint> wayPoints)
		{
			WayPoints = new List<WayPoint>(wayPoints);
		}

		public override bool ShouldBeStarted(State state)
		{
			var wp =  WayPoints.FirstOrDefault(x=>x.Coordinates == state.Fighter.Coordinates);
			return wp != null && wp != WayPoints.Last();
		}

		public override bool CanMove(State state)
		{
			return index != WayPoints.Count;
		}

		public override void Start(State state, ITactics tactics)
		{
			index = 1 + WayPoints.FindIndex(x=>x.Coordinates == state.Fighter.Coordinates);
		}

		public override MoveInfo MakeMove(State state)
		{
			var res = new MoveInfo(WayPoints[index].Coordinates,
			                       WayPoints[index].Angle);
			index++;
			return res;
		}
	}
}