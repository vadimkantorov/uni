using System;
using Insurance.Forms;

namespace Insurance.Controllers
{
	public class VinController : Controller<GenericForm>
	{
		public override void UpdateInsuranceInfo()
		{
			info.CarDetails.VIN = form.TextInput;
		}

		public override Type GetNextControllerType()
		{
			return typeof (StateIdController);
		}

		protected override void ConfigureForm(GenericForm f)
		{
			f.Title = "VIN";
			f.Mask = "CCCCCCCCCCCCCCCCCCC";
		}
	}
}