using System;
using System.Collections.Generic;
using System.Linq;

namespace Task1
{
	public class Downsampler : IInvertible<IEnumerable<Matrix>, IEnumerable<Matrix>>, IInvertible<Matrix, Matrix>
	{
		public Downsampler(int h, int v)
		{
			this.h = h;
			this.v = v;
		}

		public IEnumerable<Matrix> Fwd(IEnumerable<Matrix> input)
		{
			return input.Take(1).Concat(input.Skip(1).Select(Downsample));
		}

		public IEnumerable<Matrix> Back(IEnumerable<Matrix> output)
		{
			return output.Take(1).Concat(output.Skip(1).Select(Upsample));
		}

		public Matrix Fwd(Matrix input)
		{
			return Downsample(input);
		}

		public Matrix Back(Matrix output)
		{
			return Upsample(output);
		}

		private Matrix Downsample(Matrix m)
		{
			var res = new Matrix(m.Rows/v, m.Cols/h);
			for (var i = 0; i < res.Rows; i++)
			{
				for (var j = 0; j < res.Cols; j++)
					res[i, j] = m[i * v, j * h];
			}
			return res;
		}

		private Matrix Upsample(Matrix m)
		{
			var res = new Matrix(m.Rows*v, m.Cols*h);
			for (var i = 0; i < res.Rows; i++)
			{
				for (var j = 0; j < res.Cols; j++)
					res[i, j] = m[i / v, j / h];
			}
			return res;
		}

		private readonly int h;
		private readonly int v;
	}
}