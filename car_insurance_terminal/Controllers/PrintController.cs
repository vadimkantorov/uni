using System;
using System.Windows.Forms;
using Insurance.Forms;

namespace Insurance.Controllers
{
	public class PrintController : Controller<PrintForm>
	{
		public override void UpdateInsuranceInfo()
		{ }

		public override Type GetNextControllerType()
		{
			return null;
		}

		protected override void ConfigureForm(PrintForm f)
		{
			f.Controller = this;
			f.Title = "������";
		}

		public void PrintReceiptAndAddToDatabase()
		{
			var clientService = new ClientService();
			PrintHtmlString(FormatHtml(clientService));
			clientService.AddOrUpdateClient(info);
        }

		string FormatField(string text,  string val)
		{
			return string.Format("<p><b>{0}:</b> {1}</p>", text, val);
		}

		string FormatHtml(ClientService clientService)
		{
			var calculator = new PriceCalculator(Configuration.Read(), clientService);
			
			var res = "";
			res += @"<html><body>";
			res += FormatField("�������", info.PersonalDetails.LastName);
			res += FormatField("���", info.PersonalDetails.FirstName);
			res += FormatField("��������", info.PersonalDetails.MiddleName);
			res += FormatField("������", info.CarDetails.ModelName);
			res += FormatField("���. ����",  info.CarDetails.StateId);
			res += FormatField("VIN", info.CarDetails.VIN);
			res += FormatField("���", info.CarDetails.Passport);
			res += FormatField("���������",  calculator.Calculate(info).ToString());

			res += @"</body></html>";
			return res;
		}

		void PrintHtmlString(string s)
		{
			var browser = new WebBrowser {DocumentText = s};
			browser.DocumentCompleted += (o, e) => ((WebBrowser)o).Print();
		}
	}
}