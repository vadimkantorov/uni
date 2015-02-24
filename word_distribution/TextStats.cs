using System;
using System.Collections.Generic;
using System.Linq;

namespace Sln
{
	public class TextStats
	{
		void Inc(IDictionary<string, int> xxx, string key)
		{
			int val;
			xxx.TryGetValue(key, out val);
			xxx[key] = val + 1;
		}
		
		public void UpdateStats(IEnumerable<Word> doc)
		{
			var wordArray = doc.ToArray();
			var bag = new HashSet<string>();

			for (int i = 0; i < wordArray.Length; i++)
			{
				var word = wordArray[i];
				
				var currentLemma = word.FirstLexeme.Lemma;
				Inc(WordFrequency, currentLemma);
				if(!bag.Contains(currentLemma))
				{
					bag.Add(currentLemma);
					Inc(DocumentFrequency, currentLemma);
					TotalWords++;
				}

				if(i +1 < wordArray.Length && !word.HasFollowingPunctuation)
				{
					var nextWord = wordArray[i + 1];
					var nextLemma = nextWord.FirstLexeme.Lemma;

					if(IsBigram(word, nextWord))
					{
						Inc(BigramsByFirstWord, currentLemma);
						Inc(BigramsBySecondWord, nextLemma);
						Inc(BigramCount, FormBigram(currentLemma, nextLemma));
						TotalBigrams++;
					}
				}
			}
		}

		string FormBigram(string b1, string b2)
		{
			return b1 + " " + b2;
		}

		bool IsBigram(Word w1, Word w2)
		{
			return false;
		}

		public IDictionary<string, int> WordFrequency = new Dictionary<string, int>();
		public IDictionary<string, int> DocumentFrequency = new Dictionary<string, int>();
		public IDictionary<string, int> BigramCount = new Dictionary<string, int>();
		public IDictionary<string, int> BigramsByFirstWord = new Dictionary<string, int>();
		public IDictionary<string, int> BigramsBySecondWord = new Dictionary<string, int>();
		public int TotalBigrams;
		public int TotalWords;
	}
}