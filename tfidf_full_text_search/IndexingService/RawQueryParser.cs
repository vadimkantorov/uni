using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Text.RegularExpressions;
using IndexingService;

namespace IndexingService
{
	public static class RawQueryParser
	{
		public static ParsedQuery Parse(string rawQuery, List<Paradigm[]> paradigms, List<int> selectedParadigms, List<string> terms)
		{
			return Parse(rawQuery, paradigms, selectedParadigms, terms, new MorphologyAnalyzer(ConfigurationManager.AppSettings["serviceIp"]));
		}
		
		public static ParsedQuery Parse(string rawQuery, List<Paradigm[]> paradigms, List<int> selectedParadigms, List<string> terms, MorphologyAnalyzer morph)
		{
			var rawTerms = rawQuery.Split(new[] {' '}, StringSplitOptions.RemoveEmptyEntries);
			var paradigmIndices = new int[rawTerms.Length];
			for(int i = 0; i < rawTerms.Length; i++)
			{
				rawTerms[i] = Regex.Replace(rawTerms[i], @"\((?<pi>\d+)\)$", x =>
					{
						paradigmIndices[i] = int.Parse(x.Groups["pi"].Value) - 1;
						return "";
					});
			}
			if(terms != null) terms.AddRange(rawTerms);
			var paradigmsForTerms = Enumerable.Range(0, rawTerms.Length).Select(i =>
				{
					var ps = morph.Analyze(rawTerms[i]);
					if(paradigms != null) paradigms.Add(ps);
					if(paradigmIndices[i] < 0 || paradigmIndices[i] >= ps.Length)
						paradigmIndices[i] = 0;
					return ps[paradigmIndices[i]];
				});
			if(selectedParadigms != null) selectedParadigms.AddRange(paradigmIndices);
			return new ParsedQuery {TermsWithSynonyms = paradigmsForTerms.Select(x => x.Inflections).ToArray()};
		}
	}
}