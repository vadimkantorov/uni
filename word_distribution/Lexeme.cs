using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;

namespace Sln
{
	public class Lexeme
	{
		public string Lemma;
		public string PartOfSpeech;
		public string Case
		{
			get
			{
				Debug.Assert(PartOfSpeech == "S");
				return @case;
			}
			set
			{
				@case = value;
			}
		}

		private string @case;
	}

	public class Word
	{
		public Lexeme FirstLexeme
		{
			get { return Lexemes.First(); }
		}

		public IEnumerable<Lexeme> Lexemes;
		public bool HasFollowingPunctuation;
	}
}