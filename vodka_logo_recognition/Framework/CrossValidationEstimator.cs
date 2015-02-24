using System;
using System.Collections.Generic;
using System.Linq;

namespace BoozeMaster
{
	public class CrossValidationEstimator : IEstimator
	{
		public CrossValidationEstimator(int sampleCount, double trainingSetRatio, TrademarkRepository trademarkRepository)
		{
			this.sampleCount = sampleCount;
			this.trademarkRepository = trademarkRepository;
			problemSource = new RandomProblemSource(trainingSetRatio);
		}

		public double Estimate(IClassifier classifier)
		{
			double error = 0;
			for(int i = 0; i < sampleCount; i++)
			{
				var problem = problemSource.Produce(trademarkRepository);
				
				Console.WriteLine("Sample #"+ i);
				classifier.Train(problem.TrainingSet);
				
				error += Quality(classifier, problem.ControlSet);
			}
			return 1 - error / sampleCount;
		}

		static double Quality(IClassifier classifier, IList<ClassifiedTrademark> controlSet)
		{
			int error = 0;
			foreach (var trademark in controlSet)
			{
				if (classifier.Classify(trademark.Image) != trademark.TrademarkClass)
					error++;
			}
			return  (double) error / controlSet.Count; // the less the better
		}

		readonly int sampleCount;
		readonly TrademarkRepository trademarkRepository;
		readonly RandomProblemSource problemSource;
	}
}