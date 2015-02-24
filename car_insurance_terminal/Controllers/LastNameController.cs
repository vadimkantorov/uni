using System;
using Insurance.Forms;

namespace Insurance.Controllers
{
	public class LastNameController : Controller<GenericForm>
	{
		public override void UpdateInsuranceInfo()
		{
			info.PersonalDetails.LastName = form.TextInput;
		}

		public override Type GetNextControllerType()
		{
			return typeof(FirstNameController);
		}

		protected override void ConfigureForm(GenericForm f)
		{
			f.Title = "Фамилия";
		}
	}
}