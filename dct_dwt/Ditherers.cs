using System;
using System.Drawing;

namespace Task1
{
	public interface IDitherer
	{
		Bitmap Dither(Bitmap oldImage, Bitmap newImage);
	}
	
	public class DummyDitherer : IDitherer
	{
		public Bitmap Dither(Bitmap oldImage, Bitmap newImage)
		{
			return newImage;
		}
	}
	
	public class FloydDitherer : IDitherer
	{
		public Bitmap Dither(Bitmap oldImage, Bitmap newImage)
		{
			var res = new Bitmap(newImage.Width, newImage.Height);
			Action<double, ColorVector, int, int> f = (coef, diff, x, y) =>
			{
				if (x >= 0 && y >= 0 && x < res.Width && y < res.Height)
					res.SetPixel(x, y, new ColorVector(newImage.GetPixel(x, y)) + diff*(coef/16D));
			};
			for (int y = 0; y < newImage.Height; y++)
			{
				for (int x = 0; x < newImage.Width; x++)
				{
					var diff = new ColorVector(oldImage.GetPixel(x, y)) - new ColorVector(newImage.GetPixel(x, y));
					f(7, diff, x + 1, y);
					f(1, diff, x + 1, y + 1);
					f(5, diff, x, y + 1);
					f(3, diff, x - 1, y + 1);
				}
			}
			return res;
		}
	}
}