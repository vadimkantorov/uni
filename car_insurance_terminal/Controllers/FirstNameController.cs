using System;
using Insurance.Forms;

namespace Insurance.Controllers
{
	public class FirstNameController : Controller<GenericForm>
	{
		public override void UpdateInsuranceInfo()
		{
			info.PersonalDetails.FirstName = form.TextInput;
		}

		public override Type GetNextControllerType()
		{
			return typeof (MiddleNameController);
		}

		protected override void ConfigureForm(GenericForm f)
		{
			f.Title = "Èìÿ";
		}
	}
}