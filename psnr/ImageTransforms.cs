using System;
using System.Drawing;

namespace ImgComp
{
	public class ImageTransforms
	{
		public Bitmap ToYuvAndBack(Bitmap original,int yBits, int uvBits)
		{
			var bmp = new Bitmap(original.Width, original.Height);
			var yuv = new Yuv {YBits = yBits, UBits = uvBits, VBits = uvBits};
			for (int i = 0; i < bmp.Width; i++)
			{
				for (int j = 0; j < bmp.Height; j++)
				{
					yuv.FromRgb(original.GetPixel(i, j));
					bmp.SetPixel(i,j,yuv.ToRgb());
				}

			}
			return bmp;
		}

		public Bitmap ToBlackAndWhite(Bitmap original, bool dumb)
		{
			var bmp = new Bitmap(original.Width, original.Height);
			for (int i = 0; i < bmp.Width; i++)
			{
				for (int j = 0; j < bmp.Height; j++)
				{
					var rgb = original.GetPixel(i, j);

					var yuv = new Yuv();
					if(dumb)
						yuv.Y = (int)(0.333*rgb.R + 0.333*rgb.G + 0.333*rgb.B);
					else
						yuv.FromRgb(rgb);

					yuv.U = yuv.V = 0;

					bmp.SetPixel(i, j, yuv.ToRgb());
				}
			}
			return bmp;
		}

		public double Psnr(Bitmap left, Bitmap right)
		{
			var max = 255;
			var mse = Mse(left, right);
			return 10*Math.Log10(Math.Pow(max, 2)/mse);
		}

		static double Mse(Bitmap left, Bitmap right)
		{
			double res = 0;
			int w = left.Width;
			int h = left.Height;
			for (int i = 0; i < w; i++)
			{
				for (int j = 0; j < h; j++)
				{
					var lp = left.GetPixel(i, j);
					var rp = right.GetPixel(i, j);
					res += Math.Pow(lp.R - rp.R, 2)
						   + Math.Pow(lp.G - rp.G, 2)
					       + Math.Pow(lp.B - rp.B, 2);
				}
			}
			return res / (w * h * 3);
		}
	}
}