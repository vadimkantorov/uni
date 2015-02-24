using System;
using System.Linq;

namespace Okulobot
{
	public class DestroyTactics : ITactics
	{
		public DestroyTactics()
		{
			Priority = Priorities.DestroyTactics;
		}
		
		public override bool ShouldBeStarted(State state)
		{
			return CanMove(state);
		}

		public override bool CanMove(State state)
		{
			//TODO: ����� �����, �� ��� ����, ������ ����!
			return state.Enemies.Count > 0 && state.Fighter.CanShoot;
		}

		public override MoveInfo MakeMove(State state)
		{
			return new MoveInfo(
						state.Fighter.Coordinates,
						state.Fighter.Angle){
						//TODO: �������� � ����������
						FireTarget = state.Enemies.First().Coordinates,
						
			       	};
		}

		public override void Start(State state, ITactics prev)
		{
			prev.Priority += 0.05;
		}
	}
}