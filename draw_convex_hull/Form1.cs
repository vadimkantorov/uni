using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading;
using System.Windows.Forms;

namespace ConvexHull
{
	public partial class Form1 : Form
	{
		public Form1()
		{
			InitializeComponent();
		}

		private void button1_Click(object sender, EventArgs e)
		{
			if(ps.Count %3 != 0)
			{
				MessageBox.Show("Число точек должно делиться на три!");
				return;
			}
			int n = ps.Count / 3;
			var builder = new ConvexHullBuilder();
			hull = builder.BuildConvexHull(ps);
			Redraw();

			if (builder.PointsOnHull(ps, hull) > 2 * n)
			{
				MessageBox.Show("На выпуклой оболочке больше двух третей точек. Внутрь треть не поместится никак");
				return;
			}

			var workingSet = new List<Point>(ps);
			for (int i = 0; i < n; i++)
			{
				int minOnHull = 0;//int.MaxValue;
				Point? toExtract = null;

				foreach(var p in hull)
				{
					var newWorkingSet = workingSet.Where(x => x != p).ToArray();
					var newHull = builder.BuildConvexHull(newWorkingSet);
					
					int onHull = builder.PointsOnHull(newWorkingSet, newHull);
					int inside = newWorkingSet.Length - onHull;

					if(inside >= n && onHull > minOnHull)
					{
						minOnHull = onHull;
						toExtract = p;
					}
				}

				if(toExtract == null)
				{
					MessageBox.Show("На всех выпуклых оболочках больше двух третей точек. Жадина наелась");
					return;
				}

				workingSet.Remove(toExtract.Value);
				hull = builder.BuildConvexHull(workingSet);
				Redraw();
			}
		}

		void Redraw()
		{
			Refresh();
			Thread.Sleep(1000);
		}

		private void Form1_MouseDoubleClick(object sender, MouseEventArgs e)
		{
			var x = e.X - ClientRectangle.Left;
			var y = e.Y - ClientRectangle.Top;
			ps.Add(new Point(x, y));
			Refresh();
		}

		IList<Point> ps = new List<Point>();
		IList<Point> hull = new List<Point>();

		private void Form1_Paint(object sender, PaintEventArgs e)
		{
			const int w = 5;
			foreach (var p in ps)
				e.Graphics.FillEllipse(Brushes.Blue, p.X-w, p.Y-w, 2*w, 2*w);
			if (hull.Count > 0)
				e.Graphics.DrawLines(Pens.Black, hull.Concat(hull.Take(1)).ToArray());
		}

		private void button2_Click(object sender, EventArgs e)
		{
			ps = new List<Point>();
			hull = new List<Point>();
			Redraw();
		}
	}
}
