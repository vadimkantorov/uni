using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text;

namespace Sln
{
	public class Mystem
	{
		public Mystem(string file)
		{
			this.file = file;
		}

		public IEnumerable<Word> ParseFile()
		{
			var tmpFile = Path.GetTempFileName();
			var arguments = string.Format("-nicl {0} {1}", file, tmpFile);
			var process = new Process {StartInfo = new ProcessStartInfo {FileName = MystemExe, Arguments = arguments}};
			process.Start();
			process.WaitForExit();
			using (var rdr = new StreamReader(tmpFile, Encoding.GetEncoding(1251)))
			{
				string line;
				while ((line = rdr.ReadLine()) != null)
				{
					var lexemes = line.Split('|').Select(x => x.Split('=').ToArray()).ToArray();
					string lastLemma = null;
					foreach (var l in lexemes)
					{
						if (l[0] == "")
							l[0] = lastLemma;
						lastLemma = l[0];
					}
					yield return new Word
						{
							HasFollowingPunctuation = (rdr.ReadLine() ?? " ").Any(x => x != '_'),
							Lexemes = lexemes.Select(x => new Lexeme
								{
									Lemma = x[0],
									PartOfSpeech = UntilFirstComma(x[1]),
									Case = UntilFirstComma(x[2]),
								})
						};
				}
			}
		}

		private static string UntilFirstComma(string s)
		{
			return s.Split(',')[0];
		}

		string file;
		const string MystemExe = @"H:\Projects\IpZip\Task1\mystem.exe";
	}
}