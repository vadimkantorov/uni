using System;

namespace Okulobot
{
	public abstract class ITactics
	{
		public abstract bool ShouldBeStarted(State state);
		public double Priority { get; set; }
		public abstract bool CanMove(State state);
		public virtual void Start(State state, ITactics prev) {}
		public abstract MoveInfo MakeMove(State state);
	}
}