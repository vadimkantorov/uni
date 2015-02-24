using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;

namespace Task1
{
	public class ChannelDecomposer : IInvertible<IEnumerable<Matrix>, IEnumerable<Bitmap>>
	{
		public IEnumerable<Bitmap> Fwd(IEnumerable<Matrix> ycrcb)
		{
			var ycrcbArr = ycrcb.ToArray();
			yield return Convert(ycrcbArr[0], 0, 255);
			yield return Convert(ycrcbArr[1], -127.5, 127.5);
			yield return Convert(ycrcbArr[2], -127.5, 127.5);
		}
			
		public IEnumerable<Matrix> Back(IEnumerable<Bitmap> _)
		{
			throw new NotImplementedException();
		}
		
		private Bitmap Convert(Matrix m, double min, double max)
		{
			var res = new Bitmap(m.Cols, m.Rows);
			for (int i = 0; i < m.Rows; i++)
			{
				for (int j = 0; j < m.Cols; j++)
				{
					var luma = (int)Math.Max(0, Math.Min(255, 255*(m[i, j] - min)/(max - min)));
					res.SetPixel(j, i, Color.FromArgb(luma, luma, luma));
				}
			}
			return res;
		}
	}
}