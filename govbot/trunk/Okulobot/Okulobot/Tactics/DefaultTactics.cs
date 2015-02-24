namespace Okulobot
{
	public class DefaultTactics : ITactics
	{
		private readonly CycleTactics ct;

		public DefaultTactics()
		{
			ct = new CycleTactics();
			Priority = Priorities.DefaultTactics;
		}

		public void AddInnerTactics(ITactics t)
		{
			ct.AddInnerTactics(t);
		}

		public override bool ShouldBeStarted(State state)
		{
			return (state.Fighter.Coordinates.X == 0 && state.Fighter.Coordinates.Y <= 20) || ct.ShouldBeStarted(state);
		}

		public override bool CanMove(State state)
		{
			return ct.CanMove(state);
		}

		public override void Start(State state, ITactics cur)
		{
			ct.Start(state, cur);
		}

		public override MoveInfo MakeMove(State state)
		{
			return ct.MakeMove(state);
		}
	}
}