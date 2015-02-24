using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Collections.Generic;

namespace Task1
{
	public class Matrix : IEnumerable<double>
	{
		readonly double[,] arr;

		public Matrix(int rows, int cols)
			: this(new double[rows,cols])
		{
		}

		public Matrix(double[,] initial)
		{
			arr = initial;
		}

		public Matrix(Matrix m)
			: this(m.arr)
		{
		}

		public double this[int row, int col]
		{
			get { return arr[row, col]; }
			set { arr[row, col] = value; }
		}

		public int Rows
		{
			get { return arr.GetLength(0); }
		}

		public int Cols
		{
			get { return arr.GetLength(1); }
		}

		public static Matrix operator *(Matrix lhs, Matrix rhs)
		{
			Debug.Assert(lhs.Cols == rhs.Rows);
			var res = new Matrix(lhs.Rows, rhs.Cols);
			for (int i = 0; i < res.Rows; i++)
			{
				for (int j = 0; j < res.Cols; j++)
				{
					double sum = 0;
					for (int k = 0; k < lhs.Cols; k++)
						sum += lhs[i, k]*rhs[k, j];
					res[i, j] = sum;
				}
			}
			return res;
		}

		public static Matrix operator +(Matrix lhs, Matrix rhs)
		{
			Debug.Assert(lhs.Rows == rhs.Rows && lhs.Cols == rhs.Cols);
			var res = new Matrix(lhs.Rows, lhs.Cols);
			for (int i = 0; i < res.Rows; i++)
			{
				for (int j = 0; j < res.Cols; j++)
					res[i, j] = lhs[i, j] + rhs[i, j];
			}
			return res;
		}

		public static Matrix operator -(Matrix m)
		{
			return m*(-1);
		}

		public static Matrix operator *(Matrix m, int x)
		{
			var res = new Matrix(m.Rows, m.Cols);
			for (int i = 0; i < res.Rows; i++)
			{
				for (int j = 0; j < res.Cols; j++)
					res[i, j] = x*m[i, j];
			}
			return res;
		}

		public Matrix Transpose()
		{
			var res = new Matrix(Cols, Rows);
			for (int i = 0; i < res.Rows; i++)
			{
				for (int j = 0; j < res.Cols; j++)
					res[i, j] = this[j, i];
			}
			return res;
		}

		public override string ToString()
		{
			var sb = new StringBuilder();
			for (int i = 0; i < Rows; i++)
			{
				for (int j = 0; j < Cols; j++)
					sb.AppendFormat("{0:F0}", this[i, j]).Append(" ");
				sb.AppendLine();
			}
			return sb.ToString();
		}

		public IEnumerator<double> GetEnumerator()
		{
			return arr.Cast<double>().GetEnumerator();
		}

		System.Collections.IEnumerator System.Collections.IEnumerable.GetEnumerator()
		{
			return GetEnumerator();
		}
	}
}