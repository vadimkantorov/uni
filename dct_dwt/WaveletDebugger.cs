using System;
using System.Collections.Generic;
using System.Linq;

namespace Task1
{
	public class WaveletDebugger : IInvertible<IEnumerable<Matrix>, IEnumerable<Matrix>>
	{
		public IEnumerable<Matrix> Fwd(IEnumerable<Matrix> input)
		{
			new Bitmap2Matrices().Back(input.Select(Normalize)).Save(DateTime.Now.ToString("HH-mm-ss") + ".bmp");
			return input;
		}

		public IEnumerable<Matrix> Back(IEnumerable<Matrix> output)
		{
			return output;
		}

		private static Matrix Normalize(Matrix m)
		{
			var minElement = m.Min();
			var maxElement = m.Max();
			var res = new Matrix(m.Rows,m.Cols);
			for (var r = 0; r < res.Rows; r++)
			{
				for (var c = 0; c < res.Cols; c++)
					res[r, c] = (m[r, c] - minElement)/maxElement*255;
			}
			return res;
		}
	}
}