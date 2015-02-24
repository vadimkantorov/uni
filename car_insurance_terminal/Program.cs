using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows.Forms;
using Insurance.Forms;

namespace Insurance
{
	static class Program
	{
		/// <summary>
		/// The main entry point for the application.
		/// </summary>
		[STAThread]
		static void Main()
		{
			Application.EnableVisualStyles();
			Application.SetCompatibleTextRenderingDefault(false);

			//Application.Run(new GenericForm {Title = "Тестовая форма"});
			
			new Bootstrapper().Start();
		}
	}
}
