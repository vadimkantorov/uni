using System;
using System.Windows.Forms;
using Insurance.Forms;

namespace Insurance
{
	public abstract class Controller<TForm> : IController
		where TForm : InsuranceForm, new()
	{
		protected TForm form;
		protected InsuranceInfo info;

		public void Start(InsuranceInfo info)
		{
			this.info = info;
			
			form = new TForm();
			ConfigureForm(form);
			Application.Run(form);
		}

		public ScreenResult Result
		{
			get
			{
				if (form.DialogResult == DialogResult.OK)
					return ScreenResult.Proceed;
				if (form.DialogResult == DialogResult.Cancel)
					return ScreenResult.Cancel;
				return ScreenResult.Close;
			}
		}
		
		public abstract void UpdateInsuranceInfo();
		
		public abstract Type GetNextControllerType();

		protected virtual void ConfigureForm(TForm f) {  }
	}
}