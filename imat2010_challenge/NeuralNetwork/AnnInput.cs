using System;
using System.Linq;

namespace AnnAppro
{
	public class AnnInput
	{
		public const int HistoricalVelocitiesCount = 2;
		public const int InputCount = HistoricalVelocitiesCount;

		public double[] HistoricalVelocities;
		public DateTime Time;

		public static implicit operator double[](AnnInput i)
		{
			return //new[] {TimeNormalizer.Normalize((i.Time - TimeConstants.T16).TotalMinutes)}.Concat
				//(Enumerable.Range(0, HistoricalVelocitiesCount / 2 - 1).Select(x => Av(i.HistoricalVelocities[2 * x], i.HistoricalVelocities[2 * x+1]))).ToArray();
				(Enumerable.Range(0, HistoricalVelocitiesCount).Select(x => i.HistoricalVelocities[x])).ToArray();
		}

		static double Av(double a, double b)
		{
			return (a + b)/2;
		}

		public AnnInput Shift(double nextVelocity, DateTime nextTime)
		{
			return new AnnInput
				{
					HistoricalVelocities =
						HistoricalVelocities.Skip(1).Concat(new[] {nextVelocity}).ToArray(),
					Time = nextTime
				};
		}

		static readonly INormalizer TimeNormalizer = new LinearNormalizer(new Statistics {MaxValue = TimeConstants.LastMeasurement*TimeConstants.Interval});
	}

	public class TimeAnnInput
	{
		public const int InputCount = 1;

		public DateTime Time;

		public static implicit operator double[](TimeAnnInput i)
		{
			return new[] {TimeNormalizer.Normalize((i.Time - TimeConstants.T16).TotalMinutes)};
				
		}

		public TimeAnnInput Shift(double nextVelocity, DateTime nextTime)
		{
			return new TimeAnnInput
			{
				Time = nextTime
			};
		}

		static readonly INormalizer TimeNormalizer = new LinearNormalizer(new Statistics { MaxValue = TimeConstants.LastMeasurement * TimeConstants.Interval });
	}
}