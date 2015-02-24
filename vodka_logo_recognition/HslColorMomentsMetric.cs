using System;

namespace BoozeMaster
{
	public class HslColorMomentsMetric : IMetric<double[]>
	{
		/*const double
				hm = 1,
				hd = 1,
				hs = 1,
				sm = 3,
				sd = 3,
				ss = 3,
				lm = 0.7,
				ld = 0.7,
				ls = 0.7;
		*/
		const double
				hm = 1,
				hd = 1,
				hs = 1,
				sm = 1,
				sd = 1,
				ss = 1,
				lm = 1,
				ld = 1,
				ls = 1;
		
		double[,] mm = new[,]
			{
				{hm, hd, hs},
				{sm, sd, ss},
				{lm, ld, ls}
			};

		public double Distance(double[] a, double[] b)
		{
			var res = 0.0;
			for (int i = 0; i < 3; i++)
				for (int k = 0; k < 3; k++)
					res += mm[i, k]*Math.Abs(a[3*i + k] - b[3*i + k]);

			return res;
		}
	}
}