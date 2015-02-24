using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;

namespace AnnAppro
{
	public class Line
	{
		public int Street;
		public int Day;
		public DateTime Time;
		public double? Velocity;

		public Line()
		{ }

		public Line(string s)
		{
			var x = s.Split();
			Street = Convert.ToInt32(x[0]);
			Day = Convert.ToInt32(x[1]);
			Time = DateTime.Parse(x[2]);
			
			if(x[3] != "??")
				Velocity = Convert.ToDouble(x[3], CultureInfo.InvariantCulture);
		}

		public override string ToString()
		{
			return Street + " " + Day + " " + Time.ToShortTimeString() +" " + (Velocity == null
			       	? "??"
			       	: Velocity.Value.ToString("F2", CultureInfo.InvariantCulture));
		}

		public static void OutputLines(string path, IEnumerable<Line> jams)
		{
			using (var fs = File.CreateText(path))
			{
				jams.Each(x => fs.WriteLine(x));
			}
		}

		public Line SelectClosest(IEnumerable<Line> lines)
		{
			int bd = int.MaxValue;
			Line best = null;

			foreach (var l in lines)
			{
				int td = (int)Math.Abs((l.Time - Time).TotalMinutes);
				if (td < bd)
				{
					bd = td;
					best = l;
				}
			}
			return best;
		}
	}
}