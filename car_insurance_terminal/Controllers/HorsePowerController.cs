using System;
using Insurance.Forms;

namespace Insurance.Controllers
{
	public class HorsePowerController : Controller<DigitalInputForm>
	{
		public override void UpdateInsuranceInfo()
		{
			info.CarDetails.HorsePower = form.IntegerInput;
		}

		public override Type GetNextControllerType()
		{
			return typeof(PrintController);
		}

		protected override void ConfigureForm(DigitalInputForm f)
		{
			f.Title = "Количество лошадиных сил";
		}
	}
}