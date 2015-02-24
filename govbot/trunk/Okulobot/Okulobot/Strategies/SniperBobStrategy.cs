using System;
using System.Linq;

namespace Okulobot.Strategies
{
	public class SniperBobStrategy : BaseStrategy
	{
		public SniperBobStrategy()
		{
			var lt = new LinearTactics(ImportWayPoints("bob/main"));
			
			DefaultTactics.AddInnerTactics(lt);
			//DefaultTactics.AddInnerTactics(new RotateTactics(lt.WayPoints.Last(), 0, 1));
			DefaultTactics.AddInnerTactics(new RotateTactics(lt.WayPoints.Last(), 0, null));
			DefaultTactics.Priority += 0.05;
			
			var alt2 = new DefaultTactics();
			var lt2 = new LinearTactics(ImportWayPoints("bob/second"));
			alt2.AddInnerTactics(lt2);
			alt2.AddInnerTactics(new RotateTactics(lt2.WayPoints.Last(),0,null));
			AvailableTactics.Add(alt2);

			var alt3 = new DefaultTactics();
			var lt3 = new LinearTactics(ImportWayPoints("bob/defensive"));
			alt3.AddInnerTactics(lt3);
			alt3.AddInnerTactics(new RotateTactics(lt3.WayPoints.Last(), 0, null));
			AvailableTactics.Add(alt3);
		}
	}
}