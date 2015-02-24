using System;

namespace AnnAppro
{
	public interface INormalizer
	{
		double Normalize(double x);
		double Denormalize(double x);
	}

	public class LinearNormalizer : INormalizer
	{
		public double Normalize(double x)
		{
			return (x - stats.MinValue) / Denominator;
		}

		public double Denormalize(double x)
		{
			return x*Denominator + stats.MinValue;
		}

		public LinearNormalizer(Statistics stats)
		{
			this.stats = stats;
		}

		public double Denominator
		{
			get { return stats.MaxValue - stats.MinValue; }
		}

		readonly Statistics stats;
	}
}