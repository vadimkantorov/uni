using System;
using Insurance.Forms;

namespace Insurance.Controllers
{
	public class ExperienceController : Controller<DigitalInputForm>
	{
		public override void UpdateInsuranceInfo()
		{
			info.PersonalDetails.Experience = form.IntegerInput;
		}

		public override Type GetNextControllerType()
		{
			return typeof (AgeController);
		}

		protected override void ConfigureForm(DigitalInputForm f)
		{
			f.Title = "Стаж вождения";
		}
	}
}