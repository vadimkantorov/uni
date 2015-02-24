using System.Collections.Generic;
using System.Configuration;
using System.IO;
using System.Linq;
using System.Text;
using System.Web;

namespace Frontend
{
	public class Document
	{
		public Document(string documentId)
		{
			var documentPath = Path.Combine(ConfigurationManager.AppSettings["collectionDir"], documentId);
			lines = File.ReadAllLines(documentPath, Encoding.GetEncoding(1251));
			Title = lines.First();
			Content = lines.Skip(1);
		}

		public string Title { get; set; }
		public IEnumerable<string> Content { get; set; }

		private readonly string[] lines;
	}
}