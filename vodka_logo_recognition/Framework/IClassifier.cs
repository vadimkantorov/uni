using System.Collections.Generic;
using System.Drawing;

namespace BoozeMaster
{
	public interface IClassifier
	{
		void Train(IList<ClassifiedTrademark> trainingSet);
		
		int Classify(Bitmap trademark);
	}
}