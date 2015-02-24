using System;
using System.Collections.Generic;
using System.Linq;

namespace IndexingService
{
	public static class Ranker
	{
		public static IEnumerable<WeightedPosting> Rank(this IEnumerable<WeightedPosting> postings)
		{
			return
				from p in postings
				orderby
					p.ExactWordMatchCount descending,
					GetTfIdf(p) descending
				select p;
		}
		
		public static float GetTfIdf(WeightedPosting posting)
		{
			return Enumerable.Range(0, posting.TFs.Length).Select(x => posting.TFs[x] * posting.IDFs[x]).Sum();
		}
	}
}
