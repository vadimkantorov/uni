using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Linq.Dynamic;
using System.Text;
using System.Windows.Forms;

namespace ComputerGraphics
{
	public partial class Plot : Form
	{
		Plotter p;
		
		public Plot()
		{
			InitializeComponent();
		}

		private void pnPlot_Paint(object sender, PaintEventArgs e)
		{
			if(p != null)
				p.DrawPlot(e.Graphics);
		}

		private void btnDraw_Click(object sender, EventArgs e)
		{
			var f = DynamicExpression.ParseLambda<double, double>(tbFunc.Text).Compile();
			p = new Plotter(f, Convert.ToDouble(tbX1.Text), Convert.ToDouble(tbX2.Text));
			pnPlot.Invalidate();
		}
	}
}
