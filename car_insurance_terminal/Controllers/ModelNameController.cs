using System;
using Insurance.Forms;

namespace Insurance.Controllers
{
	public class ModelNameController : Controller<GenericForm>
	{
		public override void UpdateInsuranceInfo()
		{
			info.CarDetails.ModelName = form.TextInput;
		}

		public override Type GetNextControllerType()
		{
			return typeof (VinController);
		}

		protected override void ConfigureForm(GenericForm f)
		{
			f.Title = "Модель";
		}
	}
}