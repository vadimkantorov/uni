using System.Linq;

namespace Okulobot.Strategies
{
	public class MinigunGordonStrategy : BaseStrategy
	{
		public MinigunGordonStrategy()
		{
			var lt = new LinearTactics(ImportWayPoints("gordon/main"));

			DefaultTactics.AddInnerTactics(lt);
			DefaultTactics.Priority += 0.05;

			var alt3 = new DefaultTactics();
			var lt3 = new LinearTactics(ImportWayPoints("gordon/defensive"));
			alt3.AddInnerTactics(lt3);
			AvailableTactics.Add(alt3);
		}
	}
}