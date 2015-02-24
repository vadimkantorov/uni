using System;
using Insurance.Forms;

namespace Insurance.Controllers
{
	public class StateIdController : Controller<GenericForm>
	{
		public override void UpdateInsuranceInfo()
		{
			info.CarDetails.StateId = form.TextInput;
		}

		public override Type GetNextControllerType()
		{
			return typeof (PassportController);
		}

		protected override void ConfigureForm(GenericForm f)
		{
			f.Title = "Гос. номер";
			f.Mask = "L 000 LL 009";
		}
	}
}