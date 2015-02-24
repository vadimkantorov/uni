using System;
using System.Drawing;
using System.Drawing.Imaging;
using AForge.Imaging;

namespace BoozeMaster
{
	public class HslColorMomentsExtractor : IFeatureExtractor
	{
		public int Dimension
		{
			get { return 9; }
		}

		public double[] ExtractFeatures(Bitmap bmp)
		{
			int n = bmp.Width * bmp.Height;

			var res = new double[Dimension];
			ComputeMeans(bmp, res, n);
			ComputeStandardDeviationAndSkewness(bmp, res, n);
			return res;
		}

		static void ComputeStandardDeviationAndSkewness(Bitmap bmp, double[] res, int n)
		{
			ForAllPixels(bmp, pixel =>
			{
				for (int i = 0; i < 3; i++)
					for (int k = 1; k < 3; k++)
						res[3 * i + k] += SignSafeRoot(pixel[i] - res[3 * i + 0], k + 1);
			});

			for (int i = 0; i < 3; i++)
				for (int k = 1; k < 3; k++)
					res[3 * i + k] = SignSafeRoot(res[3 * i + 1] / n, k + 1);
		}

		static void ComputeMeans(Bitmap bmp, double[] res, int n)
		{
			ForAllPixels(bmp, pixel =>
			{
				for (int i = 0; i < 3; i++)
					res[3 * i + 0] += pixel[i];
			});

			for (int i = 0; i < 3; i++)
				res[3 * i + 0] /= n;
		}

		static double SignSafeRoot(double x, int k)
		{
			return Math.Pow(Math.Abs(x), 1.0 / k) * Math.Sign(x);
		}

		static void ForAllPixels(Bitmap image, Action<double[]> m)
		{
			/* 
				//Slow (2x times) but safe
				for (int i = 0; i < image.Width; i++)
				for (int j = 0; j < image.Height; j++)
					m(HSL.FromRGB(new RGB(image.GetPixel(i, j))));
			 */ 
			using (var unmanagedImg = UnmanagedImage.FromManagedImage(image))
			{
				CheckSourceFormat(unmanagedImg.PixelFormat);
				var pixelSize = (image.PixelFormat == PixelFormat.Format24bppRgb) ? 3 : 4;
				int pixelCnt = unmanagedImg.Height*unmanagedImg.Width;
				unsafe
				{
					var p = (byte*)unmanagedImg.ImageData.ToPointer();
					for (int pixels = 0; pixels < pixelCnt; pixels++, p += pixelSize)
					{
						var hsl = HSL.FromRGB(new RGB(p[RGB.R], p[RGB.G], p[RGB.B]));
						m(new[]{hsl.Hue/359.0, hsl.Saturation, hsl.Luminance});
					}
				}
			}
		}
		static void CheckSourceFormat(PixelFormat pixelFormat)
		{
			if (
				(pixelFormat != PixelFormat.Format24bppRgb) &&
				(pixelFormat != PixelFormat.Format32bppRgb) &&
				(pixelFormat != PixelFormat.Format32bppArgb))
			{
				throw new UnsupportedImageFormatException("Source pixel format is not supported.");
			}
		}
	}
}