using System;
using System.Collections.Generic;
using System.Linq;

namespace Task1
{
	public static class BlockTransformer
	{
		public static IEnumerable<Matrix> Transform(IEnumerable<Matrix> m, Func<Matrix, Matrix> blockTransformer )
		{
			return m.Select(x => TransformOne(x, blockTransformer));
		}

		static Matrix TransformOne(Matrix m, Func<Matrix, Matrix> blockTransformer)
		{
			var res = new Matrix(m.Rows, m.Cols);
			for (int r = 0; r < m.Rows; r += BlockSize)
			{
				for (int c = 0; c < m.Cols; c += BlockSize)
				{
					var block = m.Slice(r, c, BlockSize, BlockSize);
					res.Paste(r, c, blockTransformer(block));
				}
			}
			return res;
		}

		public const int BlockSize = 8;
	}
}