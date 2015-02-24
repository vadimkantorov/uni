using System.Drawing;

namespace ImgComp
{
	public class Yuv
	{
		int y;
		int u;
		int v;

		public int Y
		{
			get { return y; }
			set { y = Scale(value, YBits); }
		}

		public int U
		{
			get { return u; }
			set { u = Scale(value, UBits); }
		}

		public int V
		{
			get { return v; }
			set { v = Scale(value, VBits); }
		}

		static int Scale(int x, int bits)
		{
			const int depth = 1 << 8;
			return (int)((1 << bits)*x/(double)depth);
		}

		public Yuv()
		{
			YBits = UBits = VBits = 8;
		}

		public int YBits { get; set; }
		public int UBits { get; set; }
		public int VBits { get; set; }
		
		public void FromRgb(Color rgb)
		{
            Y = (int) (0.299*rgb.R + 0.587*rgb.G + 0.114*rgb.B);
			U = (int)(-0.147 * rgb.R - 0.289 * rgb.G + 0.436 * rgb.B);
			V = (int)(0.615 * rgb.R - 0.515 * rgb.G - 0.100 * rgb.B);
		}

		public Color ToRgb()
		{
			return Color.FromArgb(
				(int)(Y + 0.140*V),
				(int)(Y-0.394*U-0.581*V),
				(int)(Y + 2.032 *U));
		}
	}
}