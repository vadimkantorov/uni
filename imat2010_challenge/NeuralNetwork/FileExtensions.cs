using System.Collections.Generic;
using System.IO;

namespace AnnAppro
{
	public static class FileExtensions
	{
		public static IEnumerable<string> AllLines(string path)
		{
			using (var sr = File.OpenText(path))
			{
				string line;
				while ((line = sr.ReadLine()) != null)
					yield return line;
			}
		}
	}
}