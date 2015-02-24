namespace BoozeMaster
{
	public interface IEstimator
	{
		double Estimate(IClassifier classifier);
	}
}