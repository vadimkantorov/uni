using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Drawing;
using System.Linq;

namespace Task1
{
	public class Bitmap2Matrices : IInvertible<Bitmap, IEnumerable<Matrix>>
	{
		public IEnumerable<Matrix> Fwd(Bitmap input)
		{
			var res = new Matrix[3];
			for (int i = 0; i < res.Length; i++)
				res[i] = new Matrix(input.Height, input.Width);
			for (int x = 0; x < input.Width; x++)
			{
				for (int y = 0; y < input.Height; y++)
				{
					var sourcePixel = input.GetPixel(x, y);
					res[0][y, x] = sourcePixel.R;
					res[1][y, x] = sourcePixel.G;
					res[2][y, x] = sourcePixel.B;
				}
			}
			return res;
		}

		public Bitmap Back(IEnumerable<Matrix> output)
		{
			var matrixArray = output.ToArray();
			Debug.Assert(matrixArray.Length == 3);
			var res = new Bitmap(matrixArray.First().Cols, matrixArray.First().Rows);
			Func<int, int, int, int> f = (i, x, y) => Math.Min(Math.Max(0, (int)matrixArray[i][y, x]), 255);
			for (int x = 0; x < res.Width; x++)
			{
				for (int y = 0; y < res.Height; y++)
				{
					res.SetPixel(x, y, Color.FromArgb(f(0, x, y), f(1, x, y), f(2, x, y)));
				}
			}
			return res;
		}
	}
}