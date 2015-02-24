using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Drawing;

namespace Task1
{
	public class Quantizer
	{
		public Quantizer(Bitmap image, IDitherer ditherer)
		{
			this.image = image;
			this.ditherer = ditherer;
		}

		public Bitmap Quantize(int colorCount)
		{
			var initialColors = new HashSet<Color>();
			for (int x = 0; x < image.Width; x++)
				for (int y = 0; y < image.Height; y++)
					initialColors.Add(image.GetPixel(x, y));
			pq.Insert(new Box(initialColors));
			while (pq.Count < colorCount)
			{
				var longestBox = pq.Min();
				pq.DeleteMin();
				longestBox.Sort();
				int n = longestBox.Colors.Count;
				if (n <= 1)
					break;
				int k = n/2;
				pq.Insert(new Box(longestBox.Colors.Take(k)));
				pq.Insert(new Box(longestBox.Colors.Skip(k)));
			}
			var nearestColors = new Dictionary<Color, Color>();
			foreach (var box in pq)
			{
				var rgb = new int[3];
				foreach (var color in box.Colors)
				{
					rgb[0] += color.R;
					rgb[1] += color.G;
					rgb[2] += color.B;
				}
				for (int i = 0; i < rgb.Length; i++)
					rgb[i] /= box.Colors.Count;
				var average = Color.FromArgb(rgb[0], rgb[1], rgb[2]);
				Console.WriteLine("{0} {1} {2}", average.R, average.G, average.B);
				foreach (var color in box.Colors)
					nearestColors[color] = average;
			}
			var res = new Bitmap(image.Width, image.Height);
			for (int x = 0; x < image.Width; x++)
				for (int y = 0; y < image.Height; y++)
				{
					var oldColor = image.GetPixel(x, y);
					var substitute = nearestColors[oldColor];
					res.SetPixel(x, y, substitute);
				}
			return ditherer.Dither(image, res);
		}

		private readonly Bitmap image;
		private readonly PriorityQueue<Box> pq = new PriorityQueue<Box>();
		private readonly IDitherer ditherer;

		private class Box : IComparable<Box>
		{
			public readonly List<Color> Colors;

			public Box(IEnumerable<Color> colors)
			{
				Colors = new List<Color>(colors);
				minColor = Colors.Aggregate((a, b) => Color.FromArgb(Math.Min(a.R, b.R), Math.Min(a.G, b.G), Math.Min(a.B, b.B)));
				maxColor = Colors.Aggregate((a, b) => Color.FromArgb(Math.Max(a.R, b.R), Math.Max(a.G, b.G), Math.Max(a.B, b.B)));
				id = nextId++;
			}

			public void Sort()
			{
				var diffs = GetDiffArray();
				var extractors = new Converter<Color, byte>[] { c => c.R, c => c.G, c => c.B };
				Array.Sort(diffs, extractors);
				var bestExtractor = extractors[2];
				Colors.Sort((c1, c2) => bestExtractor(c1).CompareTo(bestExtractor(c2)));
			}

			public int CompareTo(Box other)
			{
				int res = -GetDiffArray().Max().CompareTo(other.GetDiffArray().Max());
				if (res != 0)
					return res;
				return id.CompareTo(other.id);
			}

			private int[] GetDiffArray()
			{
				return new int[] { maxColor.R - minColor.R, maxColor.G - minColor.G, maxColor.B - minColor.B };
			}

			private readonly Color minColor;
			private readonly Color maxColor;
			private readonly int id;
			private static int nextId;
		}
	}
}
