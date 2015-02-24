using System;

namespace Insurance.Forms
{
	public class DigitalInputForm : GenericForm
	{
		protected override void OnLoad(System.EventArgs e)
		{
			base.OnLoad(e);

			Mask = "#######";
		}

		public int IntegerInput { get { return Convert.ToInt32(TextInput); } }
	}
}