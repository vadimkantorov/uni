using System;
using Insurance.Forms;

namespace Insurance.Controllers
{
	public class PassportController : Controller<GenericForm>
	{
		public override void UpdateInsuranceInfo()
		{
			info.CarDetails.Passport = form.TextInput;
		}

		public override Type GetNextControllerType()
		{
			return typeof(LimitationsController);
		}

		protected override void ConfigureForm(GenericForm f)
		{
			f.Title = "оря";
			f.Mask = "LL 00 LLLLLL";
		}
	}
}