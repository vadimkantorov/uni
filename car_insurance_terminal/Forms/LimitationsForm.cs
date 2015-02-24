using System.Windows.Forms;

namespace Insurance.Forms
{
	public partial class LimitationsForm : InsuranceForm
	{
		public LimitationsForm()
		{
			InitializeComponent();

			((Control) AcceptButton).Visible = false;
		}

		public bool WithLimitations { get; private set; }

		private void btnWithLimitations_Click(object sender, System.EventArgs e)
		{
			WithLimitations = true;
			Proceed();
		}

		private void btnWithoutLimitations_Click(object sender, System.EventArgs e)
		{
			WithLimitations = false;
			Proceed();
		}
	}
}
