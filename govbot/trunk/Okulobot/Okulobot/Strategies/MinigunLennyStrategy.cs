using System.Linq;

namespace Okulobot.Strategies
{
	public class MinigunLennyStrategy : BaseStrategy
	{
		public MinigunLennyStrategy()
		{
			var lt = new LinearTactics(ImportWayPoints("lenny.txt"));

			DefaultTactics.AddInnerTactics(lt);
		}
	}
}