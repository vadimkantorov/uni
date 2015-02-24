using System;
using System.Collections.Generic;
using System.Configuration;
using System.IO;
using IndexingService;

namespace Frontend
{
	public class Global : System.Web.HttpApplication
	{
		protected void Application_Start(object sender, EventArgs e)
		{
			Index = Index.Load(indexDir);
			var urlsFilePath = Path.Combine(indexDir, "urls.txt");
			if(File.Exists(urlsFilePath))
			{
				var urls = File.ReadAllLines(urlsFilePath);
				for(int i = 1; i <= urls.Length; i++)
					PostingUrls.Add(i, urls[i - 1]);
			}
		}
		
		public static Index Index;
		public static IDictionary<int, string> PostingUrls = new Dictionary<int, string>();
		private static string indexDir = ConfigurationManager.AppSettings["indexDir"];
	}
}