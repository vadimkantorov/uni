using System.Drawing;

namespace BoozeMaster
{
	public interface IFeatureExtractor
	{
		double[] ExtractFeatures(Bitmap bmp);

		int Dimension { get; }
	}
}