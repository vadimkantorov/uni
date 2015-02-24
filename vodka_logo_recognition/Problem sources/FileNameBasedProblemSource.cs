using System;
using System.IO;

namespace BoozeMaster
{
	public class FileNameBasedProblemSource : IProblemSource
	{
		public ProblemInstance Produce(TrademarkRepository rep)
		{
			var res = new ProblemInstance {TrademarkRepository = rep};
			foreach (var trademark in rep.AllTrademarks)
			{
				if(Path.GetFileNameWithoutExtension(trademark.FilePath).Contains("T"))
					res.TrainingSet.Add(trademark);
				else
					res.ControlSet.Add(trademark);
			}
			return res;
		}
	}
}