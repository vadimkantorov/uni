using System;
using System.Drawing;
using AForge.Imaging;

namespace BoozeMaster
{
	public class HistogramExtractor : IFeatureExtractor
	{
		public double[] ExtractFeatures(Bitmap bmp)
		{
			//var histogram = new ImageStatisticsHSL(bmp);
			//histogram.Saturation.
			return null;
		}

		public int Dimension
		{
			get { return 100; }
		}
	}
}