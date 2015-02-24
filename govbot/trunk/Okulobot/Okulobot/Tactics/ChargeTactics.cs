using System;
using System.Linq;

namespace Okulobot.Tactics
{
	class ChargeTactics : ITactics
	{
		public ChargeTactics()
		{
			Priority = Priorities.ChargeTactics;
		}


		public override bool ShouldBeStarted(State state)
		{
			return CanMove(state);
		}

		public override bool CanMove(State state)
		{
			//TODO: видим врага, но нет маны, валить надо! <<-- и так валим, но по вейпоинтам, ибо куда боле не знаем
			return state.Enemies.Count > 0 && state.Fighter.CanShoot;
		}

		public override MoveInfo MakeMove(State state)
		{
			var nearest = state.Enemies[0];
			var me = state.Fighter.Coordinates;

			foreach( var enem in state.Enemies)
			{
				if (length(me, enem.Coordinates) < length(me, nearest.Coordinates))
				{
					nearest = enem;
				}
			}

			var toCoordinates = state.Fighter.Coordinates;
			var toAngle = state.Fighter.Angle;

			if (length(nearest.Coordinates, me) > state.Fighter.Properties.Speed)
			{
				toCoordinates.X = me.X + ((me.X - nearest.Coordinates.X) * state.Fighter.Properties.Speed) / length(nearest.Coordinates, me);
				toCoordinates.Y = me.Y + ((me.Y - nearest.Coordinates.Y) * state.Fighter.Properties.Speed) / length(nearest.Coordinates, me);
			}
			toAngle = AngleUtils.GetDirection(me, nearest.Coordinates);

			return new MoveInfo(
						toCoordinates,
						toAngle)
						{
							FireTarget = nearest.Coordinates,
						};
		}
		private static int length(Point a, Point b)
		{
			return (int)Math.Sqrt((double) ((a.X - b.X) * (a.X - b.X) + (a.Y - b.Y)*(a.Y - b.Y)));

		}
	}
}
