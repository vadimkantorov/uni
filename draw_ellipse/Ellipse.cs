using System;
using System.Drawing;
using System.Windows.Forms;

namespace Ellipse
{
	public partial class Ellipse : Form
	{
		public Ellipse()
		{
			InitializeComponent();
		}

		private void btnRedraw_Click(object sender, EventArgs e)
		{
			p = new Plotter(Convert.ToInt32(tbA.Text), Convert.ToInt32(tbB.Text), Convert.ToInt32(tbDx.Text), Convert.ToInt32(tbDy.Text));
			var bmp = new Bitmap(pnPlot.Width, pnPlot.Height);
			
			p.DrawEllipse(drawPixel(bmp));
			pnPlot.Image = bmp;
			pnPlot.Invalidate();
		}

		Plotter p;

		readonly Func<Bitmap, Action<int, int>> drawPixel = g => (x,y) =>
			       	{
			       		int x0 = g.Width/2;
			       		int y0 = g.Height/2;

			       		g.SetPixel(x0 + x, y0 - y, Color.Black);
			       	};
	}
}
