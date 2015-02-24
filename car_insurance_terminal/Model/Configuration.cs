using System;
using System.Collections.Generic;
using System.IO;
using System.Runtime.Serialization;

namespace Insurance
{
	public class Configuration
	{
		public Dictionary<int, int> BasePrices { get; set; }

		public int AgePercent { get; set; }

		public int ExperiencePercent { get; set; }

		public int WithoutLimitationsPercent { get; set; }

		public int WithoutCrashesPercent { get; set; }

		public static Configuration Read()
		{
			try
			{
				var dcs = new DataContractSerializer(typeof(Configuration));
				using (var fs = File.Open(configPath, FileMode.OpenOrCreate))
					return (Configuration)dcs.ReadObject(fs);
			}
			catch
			{
				var cfg = new Configuration();
				cfg.Save();
				return cfg;
			}
		}

		public void Save()
		{
			var dcs = new DataContractSerializer(typeof (Configuration));
			using (var fs = File.Open(configPath, FileMode.OpenOrCreate))
				dcs.WriteObject(fs, this);
		}

		public Configuration()
		{
			WithoutCrashesPercent = 10;
			WithoutLimitationsPercent = 70;
			AgePercent = 20;
			ExperiencePercent = 30;
			BasePrices = new Dictionary<int, int>
				{
					{0, 3000},
					{100, 4000},
					{150, 6000},
					{200, 8000},
					{250, 10000}
				};
		}

		const string configPath = "config.xml";
	}
}