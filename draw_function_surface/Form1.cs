using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Globalization;
using System.Linq;
using System.Linq.Dynamic;
using System.Linq.Expressions;
using System.Text;
using System.Windows.Forms;

namespace Horizon
{
	public partial class Form1 : Form
	{
		public Form1()
		{
			InitializeComponent();
		}

		void Form1_Load(object sender, EventArgs e)
		{
			Func<double, string> cd = x => x.ToString(CultureInfo.InvariantCulture);
			tbScale.Text = cd(plotter.Scale);
			tbStep.Text = cd(plotter.Step);
			tbFront.Text = cd(plotter.FrontY);
			tbBack.Text = cd(plotter.BackY);
		}

		private void btnDraw_Click(object sender, EventArgs e)
		{
			btnDraw.Enabled = false;
			var f = ((Expression<Func<double,double,double>>)
				DynamicExpression.ParseLambda(
				new[] {Expression.Parameter(typeof (double), "x"), Expression.Parameter(typeof (double), "y")},
				typeof (double), tbFunction.Text)).Compile();

			Func<string, double> cd = x => Convert.ToDouble(x, CultureInfo.InvariantCulture);
			plotter.Scale = cd(tbScale.Text);
			plotter.Step = cd(tbStep.Text);
			plotter.FrontY = cd(tbFront.Text);
			plotter.BackY = cd(tbBack.Text);

			var bmp = new Bitmap(pnPlot.Width, pnPlot.Height);
			using(var g = Graphics.FromImage(bmp))
				plotter.DrawSurface(f,pnPlot.Width, pnPlot.Height,g);
			pnPlot.Image = bmp;
			btnDraw.Enabled = true;
		}

		readonly Plotter plotter = new Plotter();
	}
}
