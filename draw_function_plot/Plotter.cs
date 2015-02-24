using System;
using System.Drawing;
using System.Linq;

namespace ComputerGraphics
{
	public class Plotter
	{
		public Plotter(Func<double, double> f, double x1, double x2)
		{
			this.f = f;
			this.x1 = (float)x1;
			this.x2 = (float)x2;
		}

		public void DrawPlot(Graphics g)
		{
			const int d = 1000000;
			var dx = (x2 - x1)/d;
			var points = from k in Enumerable.Range(0, d)
						 let x = x1 + k*dx
						 select new PointF(x, (float) f(x));

			var minValue = points.Min(x => x.Y);
			var maxValue = points.Max(x => x.Y);
			var clipBounds = g.VisibleClipBounds;
			
			var ps = points.Select(p => new PointF((p.X - x1)/(x2 - x1)*clipBounds.Width,
			                           (maxValue - p.Y)/(maxValue - minValue)*clipBounds.Height));

			g.DrawLines(plotPen, ps.ToArray());

			if (minValue <= 0 && maxValue >= 0)
			{
				var horizontalY = maxValue/(maxValue - minValue)*clipBounds.Height;
				g.DrawLine(axePen, new PointF(0, horizontalY), new PointF(clipBounds.Width, horizontalY));
			}

			if (x1 <= 0 && x2 >= 0)
			{
				var verticalX = - x1/(x2 - x1)*clipBounds.Width;
				g.DrawLine(axePen, new PointF(verticalX, clipBounds.Height), new PointF(verticalX, 0));
			}
		}

		readonly Func<double, double> f;
		readonly float x1;
		readonly float x2;
		Pen plotPen = new Pen(Color.Black, 3);
		Pen axePen = new Pen(Color.Blue, 3);
	}
}