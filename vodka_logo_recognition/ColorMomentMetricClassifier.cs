using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;

namespace BoozeMaster
{
	class ColorMomentMetricClassifier : IClassifier
	{
		public void Train(IList<ClassifiedTrademark> trainingSet)
		{
			known = trainingSet.ToDictionary(x => x, x => featureExtractor.ExtractFeatures(x.Image));
		}

		public int Classify(Bitmap trademark)
		{
			var featureVector = featureExtractor.ExtractFeatures(trademark);
			int best = -1;
			double bestDist = double.MaxValue;

			foreach (var kvp in known)
			{
				var dist = metric.Distance(featureVector, kvp.Value);
				if (dist < bestDist)
				{
					best = kvp.Key.TrademarkClass;
					bestDist = dist;
				}
			}
			return best;
		}

		Dictionary<ClassifiedTrademark, double[]> known;
		readonly HslColorMomentsExtractor featureExtractor = new HslColorMomentsExtractor();
		readonly HslColorMomentsMetric metric = new HslColorMomentsMetric();
	}
}