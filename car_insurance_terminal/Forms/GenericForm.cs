using System;
using System.Windows.Forms;

namespace Insurance.Forms
{
	public partial class GenericForm : InsuranceForm
	{
		public GenericForm()
		{
			InitializeComponent();

			tbMasked.Mask = "????????????????????";
		}

		public Type KeyboardType { get { return keyboardType ?? typeof (RussianKeyboard); } set { keyboardType = value;} }

		public string Mask { get { return tbMasked.Mask; } set { tbMasked.Mask = value; } }

		public string TextInput { get { return tbMasked.Text.Replace(" ", ""); } }

		protected override bool CanProceed()
		{
			return tbMasked.MaskCompleted;
		}

		Type keyboardType;
		Control lastActiveControl;

		private void GenericForm_Load(object sender, EventArgs e)
		{
			var keyboard = (IVirtualKeyboard) Activator.CreateInstance(KeyboardType);
			keyboard.VirtualKeyPress += keyboard_VirtualKeyPress;

			pnKeyboard.Controls.Add((Control)keyboard);

			tbMasked.Focus();
		}

		void keyboard_VirtualKeyPress(object sender, KeyPressEventArgs e)
		{
			if (lastActiveControl != null)
			{
				lastActiveControl.Focus();
				SendKeys.SendWait(e.KeyChar.ToString());
			}
		}

		private void tbMasked_Enter(object sender, EventArgs e)
		{
			lastActiveControl = (Control) sender;
		}

		private void GenericForm_FormClosing(object sender, FormClosingEventArgs e)
		{
			
		}
	}
}