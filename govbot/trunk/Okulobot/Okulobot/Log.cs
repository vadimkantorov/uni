using System;
using System.IO;

namespace Okulobot
{
	public class Log : ILog
	{
		private readonly string path;

		public Log(string path)
		{
			const string logDir = "Logs";

			if (!Directory.Exists(logDir))
				Directory.CreateDirectory(logDir);
			
			this.path = Path.Combine(logDir, path);
			File.WriteAllText(this.path,"");
		}

		public void Write(string s)
		{
			File.AppendAllText(path, s + Environment.NewLine);
		}
	}
}