namespace Task1
{
	public static class MatrixUtils
	{
		public static Matrix Slice(this Matrix m, int r1, int c1, int rows, int cols)
		{
			var res = new Matrix(rows, cols);
			for (int i = 0; i < rows; i++)
			{
				for (int j = 0; j < cols; j++)
					res[i, j] = m[i + r1, j + c1];
			}
			return res;
		}
		
		public static void Paste(this Matrix dest, int r1, int c1, Matrix src)
		{
			for (int i = 0; i < src.Rows; i++)
			{
				for (int j = 0; j < src.Cols; j++)
					dest[r1 + i, c1 + j] = src[i, j];
			}
		}
	}
}
