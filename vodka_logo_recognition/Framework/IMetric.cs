namespace BoozeMaster
{
	public interface IMetric<T>
	{
		double Distance(T a, T b);
	}
}