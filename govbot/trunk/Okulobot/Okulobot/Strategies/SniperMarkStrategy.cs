using System.Linq;

namespace Okulobot.Strategies
{
	public class SniperMarkStrategy : BaseStrategy
	{
		public SniperMarkStrategy()
		{
			var lt = new LinearTactics(ImportWayPoints("mark.txt"));
			
			DefaultTactics.AddInnerTactics(lt);
			//DefaultTactics.AddInnerTactics(new RotateTactics(lt.WayPoints.Last(), 0, 1));
			DefaultTactics.AddInnerTactics(new RotateTactics(lt.WayPoints.Last(), 0, null));
		}
	}
}