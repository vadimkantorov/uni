using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using Insurance.Controllers;

namespace Insurance.Forms
{
	public partial class PrintForm : InsuranceForm
	{
		public PrintForm()
		{
			InitializeComponent();

			//((Control) AcceptButton).Visible = false;
		}

		public PrintController Controller { get; set; }

		private void btnPrint_Click(object sender, EventArgs e)
		{
			Controller.PrintReceiptAndAddToDatabase();
			//Proceed();
		}
	}
}
