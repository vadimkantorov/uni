using System;
using System.Collections.Generic;
using System.Linq;

namespace Sln
{
	public static class WordFilter
	{
		public static IEnumerable<Word> Filter(IEnumerable<Word> words)
		{
			return words.Where(w => !IsBad(w));
		}

		static bool IsBad(Word word)
		{
			return word.Lexemes.Any(x => x.Lemma.EndsWith("?")) || word.Lexemes.Select(x => x.PartOfSpeech).Distinct().Count() > 1;
		}
	}
}