using System;

namespace AnnAppro
{
	public class BipolarNormalizer : INormalizer
	{
		public double Normalize(double x)
		{
			return (x - mean)/variance;
		}

		public double Denormalize(double x)
		{
			return x*variance + mean;
		}

		public BipolarNormalizer(double mean, double stdDev)
		{
			this.mean = mean;
			this.variance = stdDev;//Math.Pow(stdDev, 2.0);
		}

		readonly double mean;
		readonly double variance;
	}
}