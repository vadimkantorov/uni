using System;

namespace AnnAppro
{
	public class TimeConstants
	{
		public const int Interval = 4;
		public const int IntervalsInHour = 60 / Interval;
		public const int EarlyEvening = 30;
		public const int LastMeasurement = 6*IntervalsInHour;
		public static readonly DateTime T18 = DateTime.Today.AddHours(18);
		public static readonly DateTime T16 = DateTime.Today.AddHours(16);

		public static DateTime EveningTime(int ind4)
		{
			return T16.AddMinutes(ind4*4);
		}
	}
}