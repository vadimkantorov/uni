namespace BoozeMaster
{
	class RatioBasedProblemSource : IProblemSource
	{
		public ProblemInstance Produce(TrademarkRepository rep)
		{
			var res = new ProblemInstance {TrademarkRepository = rep};
			foreach (var trademark in rep.AllTrademarks)
			{
				if(res.TrainingSetRatio <= trainingSetRatio)
					res.TrainingSet.Add(trademark);
				else
					res.ControlSet.Add(trademark);
			}
			return res;
		}

		public RatioBasedProblemSource(double trainingSetRatio)
		{
			this.trainingSetRatio = trainingSetRatio;
		}

		readonly double trainingSetRatio;
	}
}