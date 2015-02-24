using System;
using System.Drawing;
using System.Linq;

namespace Task1
{
	public struct ColorVector
	{
		public ColorVector(Color c)
		{
			v = new double[] {c.R, c.G, c.B};
		}
		
		private ColorVector(double[] v)
		{
			this.v = v;
		}
		
		public static ColorVector operator*(ColorVector v1, double k)
		{
			return new ColorVector(v1.v.Select(x => x*k).ToArray());
		}
		
		public static ColorVector operator+(ColorVector v1, ColorVector v2)
		{
			return new ColorVector(Enumerable.Range(0, v1.v.Length).Select(i => v1.v[i] + v2.v[i]).ToArray());
		}
		
		public static ColorVector operator-(ColorVector v1, ColorVector v2)
		{
			return v1+v2*(-1);
		}
		
		public static implicit operator Color(ColorVector v)
		{
			var normalized = v.v.Select(x => (byte)Math.Max(0, Math.Min(255, x))).ToArray();
			return Color.FromArgb(normalized[0], normalized[1], normalized[2]);
		}
		
		private readonly double[] v;
	}
}