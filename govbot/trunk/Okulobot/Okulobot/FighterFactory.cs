using Okulobot.Strategies;

namespace Okulobot
{
	public class FighterFactory
	{
		private static string FormatLogName(InitData data, string name)
		{
			return string.Format("{0}   {1} {2}.txt", data.Team, data.Number, name);
		}
		
		public Fighter Create(InitData data)
		{
			var fighters = new Fighter[5];

			Strategy bob = new SniperBobStrategy();
			Strategy jack = new SniperJackStrategy();
			Strategy mark = new SniperMarkStrategy();
			Strategy lenny = new MinigunLennyStrategy();
			Strategy gordon = new MinigunGordonStrategy();

			fighters[0] = new Fighter('S', bob, new NullLogger(FormatLogName(data, "Bob")));
			fighters[1] = new Fighter('S', jack, new NullLogger(FormatLogName(data, "Jack")));
			fighters[2] = new Fighter('S', mark, new NullLogger(FormatLogName(data, "Mark")));
			fighters[3] = new Fighter('P', lenny, new NullLogger(FormatLogName(data, "Lenny")));
			fighters[4] = new Fighter('M', gordon, new NullLogger(FormatLogName(data, "Gordon")));

			return fighters[data.Number];
		}
	}
}