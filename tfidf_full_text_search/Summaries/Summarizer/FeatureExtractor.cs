using System.Collections.Generic;
using System.Linq;

namespace Summarizer
{
	class FeatureExtractor
	{
		public FeatureExtractor(CorpusStats corpusStats)
		{
			this.corpusStats = corpusStats;
		}

		public IEnumerable<double[]> ExtractFeatures(string fileName)
		{
			var lines = new LineTokenizer(fileName).Tokenize().ToArray();
			for (int i = 0; i < lines.Length; i++)
			{
				var line = lines[i];
				var accepted = line.StartsWith("+") ? 1 : -1;
				var relPos = (double) i/lines.Length;
				var tfIdf = new WordTokenizer(line).Tokenize().Sum(x => corpusStats.TfIdf(x, fileName));
				yield return new[] {accepted, relPos, tfIdf};
			}
		}

		readonly CorpusStats corpusStats;
	}
}