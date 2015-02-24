using System;
using System.Collections.Generic;
using System.Linq;
using Insurance.Controllers;

namespace Insurance
{
	public class PriceCalculator
	{
		readonly Configuration cfg;
		readonly ClientService service;

		int CalcualateBasePrice(IDictionary<int, int> basePrices, int horsePower)
		{
			var sorted = new SortedDictionary<int, int>(basePrices);
			foreach (KeyValuePair<int, int> kvp in sorted.Reverse())
			{
				if (horsePower > kvp.Key)
					return kvp.Value;
			}
			return -1;
		}

		public PriceCalculator(Configuration cfg, ClientService service)
		{
			this.cfg = cfg;
			this.service = service;
		}

		public int Calculate(InsuranceInfo info)
		{
			int basePrice = CalcualateBasePrice(cfg.BasePrices, info.CarDetails.HorsePower);

			int percent = 100;
			if (info.InsuranceDetails.WithLimitations)
			{
				if (info.PersonalDetails.Age < 21)
					percent += cfg.AgePercent;
				if (info.PersonalDetails.Experience < 3)
					percent += cfg.ExperiencePercent;
			}
			else
				percent += cfg.WithoutLimitationsPercent;

			if(service.WasWithoutCrashes(info))
				percent -= cfg.WithoutCrashesPercent;

			return (int)Math.Ceiling(percent*basePrice/100.0);
		}
	}
}