using System;
using System.Collections.Generic;
using System.Linq;

namespace Task1
{
	public class Rgb2YCrCb : IInvertible<IEnumerable<Matrix>, IEnumerable<Matrix>>
	{
		public IEnumerable<Matrix> ConvertColorSpace(IEnumerable<Matrix> components, Func<Matrix, Matrix> pixelTransform)
		{
			var rgb = components.ToArray();
			int rows = rgb.First().Rows;
			int cols = rgb.First().Cols;

			var res = new Matrix[3];
			for (int i = 0; i < res.Length; i++)
				res[i] = new Matrix(rows, cols);

			for (int i = 0; i < rows; i++)
			{
				for (int j = 0; j < cols; j++)
				{
					var p = new Matrix(3, 1);
					for(int k = 0; k < 3; k++)
						p[k, 0] = rgb[k][i, j];

					var t = pixelTransform(p);
					for (int k = 0; k < 3; k++)
						res[k][i, j] = t[k, 0];
				}
			}

			return res;
		}

		public IEnumerable<Matrix> Fwd(IEnumerable<Matrix> inputs)
		{
			return ConvertColorSpace(inputs, p => TransformMatrix*p + Delta);
		}

		public IEnumerable<Matrix> Back(IEnumerable<Matrix> matrices)
		{
			return ConvertColorSpace(matrices, p => InvTransformMatrix*(p + -Delta));
		}
		
		private static readonly Matrix TransformMatrix = new Matrix(new[,]
		{
			{0.299, 0.587, 0.114, },
			{-0.1687, -0.3313, 0.5, },
			{0.5, -0.4187, -0.0813, },
		});
		
		private static readonly Matrix InvTransformMatrix = new Matrix(new[,]
		{
			{1.0, 0.0, 1.402, },
			{1.0, -0.34414, -0.71414, },
			{1.0, 1.772, 0.0, },
		});

		private static readonly Matrix Delta = new Matrix(new double[,] {{0, 128, 12}}).Transpose();
	}
}