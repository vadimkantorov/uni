using System;

namespace BoozeMaster
{
	public class NaiveEstimator : IEstimator
	{
		public double Estimate(IClassifier classifier)
		{
			classifier.Train(problem.TrainingSet);
			Console.WriteLine(classifier.GetType().Name + " trained...");

			int classifiedSuccessfully = 0;
			foreach (var trademark in problem.ControlSet)
			{
				if (classifier.Classify(trademark.Image) == trademark.TrademarkClass)
					classifiedSuccessfully++;
			}
			return (double) classifiedSuccessfully / problem.ControlSet.Count;
		}

		public NaiveEstimator(ProblemInstance problem)
		{
			this.problem = problem;
		}

		readonly ProblemInstance problem;
	}
}