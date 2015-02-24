using System.Collections.Generic;
using System.Linq;

namespace AnnAppro
{
	public class Track
	{
		public int Street { get; private set; }
		public int Day { get; private set; }

		public Track(IEnumerable<Line> lines, int street, int day)
		{
			Street = street;
			Day = day;
			Lines = lines.ToArray();
		}

		public Line[] Lines;


		public static Track[] ReadTracks(string path)
		{
			return (from strLine in FileExtensions.AllLines(path)
					let line = new Line(strLine)
					orderby line.Time
					group line by new { line.Day, line.Street }
						into g
						select new Track(g, g.Key.Street, g.Key.Day)).ToArray();
		}

		public static Statistics BuildStats(Track[] tracks)
		{
			var vs = tracks.SelectMany(x => x.Lines).Select(x => x.Velocity.Value).ToArray();
			return new Statistics { MaxValue = vs.Max(), MinValue = vs.Min() };
		}

		public static void NormalizeTracks(Track[] tracks, INormalizer normalizer)
		{
			foreach (var track in tracks)
			{
				foreach (var line in track.Lines)
					line.Velocity = normalizer.Normalize(line.Velocity.Value);
			}
		}
	}
}