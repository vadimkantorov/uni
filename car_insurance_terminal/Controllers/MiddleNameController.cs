using System;
using Insurance.Forms;

namespace Insurance.Controllers
{
	public class MiddleNameController : Controller<GenericForm>
	{
		public override void UpdateInsuranceInfo()
		{
			info.PersonalDetails.MiddleName = form.TextInput;
		}

		public override Type GetNextControllerType()
		{
			return typeof (ModelNameController);
		}

		protected override void ConfigureForm(GenericForm f)
		{
			f.Title = "Отчество";
		}
	}
}