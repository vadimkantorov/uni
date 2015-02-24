using System;
using Insurance.Forms;

namespace Insurance.Controllers
{
	public class LimitationsController : Controller<LimitationsForm>
	{
		public override void UpdateInsuranceInfo()
		{
			info.InsuranceDetails.WithLimitations = form.WithLimitations;
		}

		public override Type GetNextControllerType()
		{
			if (form.WithLimitations)
				return typeof (ExperienceController);
			return typeof (HorsePowerController);
		}

		protected override void ConfigureForm(LimitationsForm f)
		{
			f.Title = "Вид страховки";
		}
	}
}