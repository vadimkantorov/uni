using System;
using System.Collections.Generic;
using System.Linq;

namespace BoozeMaster
{
	public class RandomProblemSource : IProblemSource
	{
		public RandomProblemSource(double trainingSetRatio)
		{
			this.trainingSetRatio = trainingSetRatio;
		}

		public ProblemInstance Produce(TrademarkRepository rep)
		{
			var random = new Random();
			var res = new ProblemInstance { TrademarkRepository = rep};

			int n = random.Next(2, rep.AllTrademarks.Count);

			var trademarkIndices = Enumerable.Range(0, rep.AllTrademarks.Count).ToArray();
			Shuffle(trademarkIndices);

			for (int i = 0; i < n; i++)
			{
				if (res.TrainingSetRatio <= trainingSetRatio)
					res.TrainingSet.Add(rep.AllTrademarks[trademarkIndices[i]]);
				else
					res.ControlSet.Add(rep.AllTrademarks[trademarkIndices[i]]);
			}

			return res;
		}

		// Fisher-Yates shuffle from http://en.wikipedia.org/wiki/Fisher-Yates_shuffle
		static void Shuffle<T>(IList<T> list)
		{
			var random = new Random();
			int n = list.Count;
			while (n > 1)
			{
				int k = random.Next(n);
				n--;

				var tmp = list[k];
				list[k] = list[n - 1];
				list[n - 1] = tmp;
			}
		}

		readonly double trainingSetRatio;
	}
}