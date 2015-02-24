using System.Collections.Generic;

namespace BoozeMaster
{
	public class ProblemInstance
	{
		public IList<ClassifiedTrademark> TrainingSet { get; set; }

		public IList<ClassifiedTrademark> ControlSet { get; set; }

		public TrademarkRepository TrademarkRepository { get; set; }

		public double TrainingSetRatio
		{
			get
			{
				if (TrainingSet.Count == 0)
					return 0;
				return (double) TrainingSet.Count / (TrainingSet.Count + ControlSet.Count);
			}
		}

		public ProblemInstance()
		{
			TrainingSet = new List<ClassifiedTrademark>();
			ControlSet = new List<ClassifiedTrademark>();
		}
	}
}