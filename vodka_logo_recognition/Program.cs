using System;
using System.Drawing;
using AForge.Imaging.Filters;

namespace BoozeMaster
{
	class Program
	{
		const string InputDir = @"..\..\Data\Best";
		const double TrainingSetRatio = 0.75;
		
		static void Main()
		{
			var preprocessor = new AForgeImagePreprocessor();
			var rep = new TrademarkRepository(InputDir, preprocessor);

			var classifier = new GansClassifier(rep.AllTrademarkClasses.Count);
			//var classifier = new ColorMomentANNClassifier(rep.AllTrademarkClasses.Count);
			//var classifier = new ColorMomentMetricClassifier(); 

			var estimator = new CrossValidationEstimator(40, TrainingSetRatio, rep);
			//var estimator = new NaiveEstimator(problem);

			Console.WriteLine("Classes: {0} ({1:P02} each); Training set: {2:F0}; Control set: {3:F0}", rep.AllTrademarkClasses.Count, 1.0 / rep.AllTrademarkClasses.Count, rep.AllTrademarks.Count*TrainingSetRatio, rep.AllTrademarks.Count*(1-TrainingSetRatio));

			var estimate = estimator.Estimate(classifier);
			Console.WriteLine(@"{0}: {1:P02}", classifier.GetType().Name, estimate);
		}
	}
}
