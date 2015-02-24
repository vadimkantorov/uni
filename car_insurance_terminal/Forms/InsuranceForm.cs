using System;
using System.Windows.Forms;

namespace Insurance.Forms
{
	public partial class InsuranceForm : Form
	{
		public string Title 
		{ 
			get { return lblTitle.Text; } 
			set 
			{ 
				lblTitle.Text = value;
				Text = value; 
			}
		}

		public InsuranceForm()
		{
			InitializeComponent();
		}

		private void btnProceed_Click(object sender, EventArgs e)
		{
			Proceed();
		}

		protected void Proceed()
		{
			if (CanProceed())
				Exit(DialogResult.OK);
		}

		private void btnCancel_Click(object sender, EventArgs e)
		{
			Cancel();
		}

		protected virtual bool CanProceed() { return true; }

		protected void Cancel()
		{
			Exit(DialogResult.Cancel);
		}

		void Exit(DialogResult res)
		{
			DialogResult = res;
			Close();
		}

		private void InsuranceForm_Load(object sender, EventArgs e)
		{
			pnKeyboard.SendToBack();
		}
	}
}
