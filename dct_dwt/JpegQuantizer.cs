using System;
using System.Collections.Generic;
using System.Linq;

namespace Task1
{
	public class JpegQuantizer : IInvertible<IEnumerable<Matrix>, IEnumerable<Matrix>>
	{
		public IEnumerable<Matrix> Fwd(IEnumerable<Matrix> inputs)
		{
			return
				BlockTransformer.Transform(inputs.Take(1), x => Quantize(x, ym, -1))
				.Concat(BlockTransformer.Transform(inputs.Skip(1), x => Quantize(x, crcbm, -1)));
		}

		public IEnumerable<Matrix> Back(IEnumerable<Matrix> matrices)
		{
			return
				BlockTransformer.Transform(matrices.Take(1), x => Quantize(x, ym, 1))
				.Concat(BlockTransformer.Transform(matrices.Skip(1), x => Quantize(x, crcbm, 1)));
		}

		Matrix Quantize(Matrix src, Matrix quantizationMatrix, int power)
		{
			var res = new Matrix(src.Rows, src.Cols);
			for (int i = 0; i < res.Rows; i++)
			{
				for (int j = 0; j < res.Cols; j++)
					res[i, j] = Math.Round(src[i, j] * Math.Pow(quantizationMatrix[i, j], power));
			}
			return res;
		}

		public JpegQuantizer(double gamma)
		{
			for (int i = 0; i < ym.Rows; i++)
			{
				for (int j = 0; j < ym.Cols; j++)
				{
					ym[i, j] *= gamma;
					if (ym[i, j] < 1)
						ym[i, j] = 1;
					crcbm[i, j] *= gamma;
					if (crcbm[i, j] < 1)
						crcbm[i, j] = 1;
				}
			}
		}

		readonly Matrix ym = new Matrix(new double[,]
			{
				{16, 11, 10, 16, 24, 40, 51, 61},
				{12, 12, 14, 19, 26, 58, 60, 55},
				{14, 13, 16, 24, 40, 57, 69, 56},
				{14, 17, 22, 29, 51, 87, 80, 62},
				{18, 22, 37, 56, 68, 109, 103, 77},
				{24, 35, 55, 64, 81, 104, 113, 92},
				{49, 64, 78, 87, 103, 121, 120, 101},
				{72, 92, 95, 98, 112, 100, 103, 99},
			});

		readonly Matrix crcbm = new Matrix(new double[,]
			{
				{17, 18, 24, 47, 99, 99, 99, 99},
				{18, 21, 26, 66, 99, 99, 99, 99},
				{24, 26, 56, 99, 99, 99, 99, 99},
				{47, 66, 99, 99, 99, 99, 99, 99},
				{99, 99, 99, 99, 99, 99, 99, 99},
				{99, 99, 99, 99, 99, 99, 99, 99},
				{99, 99, 99, 99, 99, 99, 99, 99},
				{99, 99, 99, 99, 99, 99, 99, 99},
			});
	}
}