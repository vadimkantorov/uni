using System;
using System.Collections.Generic;

namespace Task1
{
	public class Dct : IInvertible<IEnumerable<Matrix>, IEnumerable<Matrix>>
	{
		public IEnumerable<Matrix> Fwd(IEnumerable<Matrix> inputs)
		{
			return BlockTransformer.Transform(inputs, m => T*m*T.Transpose());
		}

		public IEnumerable<Matrix> Back(IEnumerable<Matrix> matrices)
		{
			return BlockTransformer.Transform(matrices, m => T.Transpose() * m * T);
		}

		static Dct()
		{
			double sqM = Math.Sqrt(BlockTransformer.BlockSize);
			double c1 = 1/sqM;
			double c2 = Math.Sqrt(2)/sqM;
			for (int i = 0; i < BlockTransformer.BlockSize; i++)
			{
				for (int j = 0; j < BlockTransformer.BlockSize; j++)
					T[i, j] = i == 0 ? c1 : c2 * Math.Cos(Math.PI * (2 * j + 1) * i / (2 * BlockTransformer.BlockSize));
			}
		}

		private static readonly Matrix T = new Matrix(BlockTransformer.BlockSize, BlockTransformer.BlockSize);
	}
}