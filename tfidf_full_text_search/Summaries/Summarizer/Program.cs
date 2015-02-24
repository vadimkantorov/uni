using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Threading;

namespace Summarizer
{
	class Program
	{
		static void Main(string[] args)
		{
			Thread.CurrentThread.CurrentCulture = CultureInfo.InvariantCulture;
			ParseCorpus(args[0], @"..\train.txt");
			ParseCorpus(args[1], @"..\..\test.txt");
		}

		static void ParseCorpus(string corpusDir, string outFile)
		{
			var corpusStats = new CorpusStats(corpusDir);
			var featureExtractor = new FeatureExtractor(corpusStats);
			File.WriteAllLines(Path.Combine(corpusDir, outFile),
				Directory.GetFiles(corpusDir)
				.SelectMany(featureExtractor.ExtractFeatures)
				.Select(x => x.First().ToString("F0") + " " + FeaturesToString(x.Skip(1))));
		}

		static string FeaturesToString(IEnumerable<double> features)
		{
			return string.Join(" ", Enumerable.Range(1, features.Count()).Zip(features, (i, f) => i + ":" + f.ToString()));
		}
	}
}
