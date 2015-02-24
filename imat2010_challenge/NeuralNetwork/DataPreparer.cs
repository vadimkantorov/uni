using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace AnnAppro
{
	public class DataPreparer
	{
		public void Prepare()
		{

			var jams1 =
				FileExtensions.AllLines(Paths.Jams_txt).Select(x => new Line(x)).GroupBy(x => x.Street);

			var okRoads = new HashSet<int>(jams1.Where(g => g.Count(x => x.Day == 36 && x.Time.Hour <= 18) >= 20 &&
				g.Count(x => x.Day == 41) >= 20).Select(x => x.Key));
			GC.Collect();
			/*var task = FileExtensions.AllLines(Paths.Task_txt).Select(x => new Line(x)).Select(x =>
			{
				if (!okRoads.Contains(x.Street)) x.Velocity = 0;
			return x;});*/
			
			//Line.OutputLines(Paths.Task_36_txt+ "xyu",task);
			var edges3000 = new HashSet<int>(File.ReadAllLines(Paths.Edges_3000_txt).Select(x => Convert.ToInt32(x)));

			var rawTracks =
				(from strLine in FileExtensions.AllLines(Paths.Jams_txt)
				 let line = new Line(strLine)
				 where edges3000.Contains(line.Street)
				 where (line.Day + 1) % 7 < 5
				 group line by new { line.Day, line.Street }
				 into g
				select Smoothed(new Track(g, g.Key.Street, g.Key.Day))).ToArray();
			GC.Collect();

			var ttt = rawTracks.Where(g => g.Day == 36 && g.Lines.Count(x => x.Time.Hour < 18) >= 20).ToArray();
			var task = ttt.SelectMany(x => x.Lines.Where(y => y.Time.Hour >= 18)).ToArray();

			var jams = rawTracks.SelectMany(x => x.Lines).ToArray();
			GC.Collect();

			Line.OutputLines(Paths.Jams_wo36_txt, jams);
			GC.Collect();
			Line.OutputLines(Paths.Task_36_txt, task);

		}

		Track Smoothed(Track track)
		{
			const int k = 5;
			var res = new List<Line>(track.Lines.Take(k));
			
			for(int i = k; i < track.Lines.Length-k; i++)
				res.Add(new Line
					{Day = track.Day, Street = track.Street, Time = track.Lines[i].Time, Velocity = AvVel(track.Lines, i, k)});

			if (track.Lines.Length - k >= 0)
			{
				for (int i = track.Lines.Length - k; i < track.Lines.Length; i++)
					res.Add(track.Lines[i]);
			}

			return new Track(res,track.Street, track.Day);
		}

		double AvVel(Line[] lines, int i, int k)
		{
			double res = lines[i].Velocity.Value;

			for(int j = 1; j<= k; j++)
				res += lines[i + j].Velocity.Value + lines[i - j].Velocity.Value;
			
			return res/(k*2 + 1);
		}
	}
}