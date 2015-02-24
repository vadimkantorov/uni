using System;
using System.Linq;

namespace Okulobot.Strategies
{
	public class SniperJackStrategy : BaseStrategy
	{
		public SniperJackStrategy()
		{
			var lt = new LinearTactics(ImportWayPoints("jack.txt"));

			DefaultTactics.AddInnerTactics(lt);
			//DefaultTactics.AddInnerTactics(new RotateTactics(lt.WayPoints.Last(), 0, 1));
			DefaultTactics.AddInnerTactics(new RotateTactics(lt.WayPoints.Last(), 0, null));
		}
	}
}