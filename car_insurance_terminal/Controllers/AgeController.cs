using System;
using Insurance.Forms;

namespace Insurance.Controllers
{
	public class AgeController : Controller<DigitalInputForm>
	{
		public override void UpdateInsuranceInfo()
		{
			info.PersonalDetails.Age = form.IntegerInput;
		}

		public override Type GetNextControllerType()
		{
			return typeof (HorsePowerController);
		}

		protected override void ConfigureForm(DigitalInputForm f)
		{
			f.Title = "Возраст";
		}
	}
}