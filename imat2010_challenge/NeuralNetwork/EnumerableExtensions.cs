using System;
using System.Collections.Generic;
using System.Linq;

namespace AnnAppro
{
	public static class EnumerableExtensions
	{
		public static void Each<T>(this IEnumerable<T> ts, Action<T> act)
		{
			foreach (var t in ts)
				act(t);
		}

		public static IList<T> Choice<T>(this IList<T> ts, double ratio)
		{
			var shuffled = ts.ToList();
			RandomShuffle(ts);
			return shuffled.Take((int) (ts.Count*ratio)).ToArray();
		}

		static void RandomShuffle<T>(IList<T> ts)
		{
			var rnd = new Random();
			for(int i = ts.Count-1; i >= 0; i--)
			{
				int k = rnd.Next(i+1);

				var tmp = ts[k];
				ts[k] = ts[i];
				ts[i] = tmp;
			}
		}
	}
}