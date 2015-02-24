using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;

namespace Summarizer
{
	class CorpusStats
	{
		public CorpusStats(string corpusDir)
		{
			foreach (var fileName in Directory.GetFiles(corpusDir))
				ProcessDocument(fileName);
		}

		public double TfIdf(string term, string fileName)
		{
			return Tf(term, fileName)*Idf(term);
		}

		void ProcessDocument(string fileName)
		{
			if (!documents2Ids.ContainsKey(fileName))
				documents2Ids.Add(fileName, documents2Ids.Count);
			var documentId = documents2Ids[fileName];
			var allTerms = Tokenize(fileName).ToArray();
			foreach (string term in allTerms)
			{
				if (!term2documents.ContainsKey(term))
					term2documents.Add(term, new List<DocumentIdWithCount>());
				var postings = term2documents[term];
				if (postings.Count == 0 || postings.Last().DocumentId != documentId)
					postings.Add(new DocumentIdWithCount { DocumentId = documentId });
				postings.Last().TermCount++;
			}
			termCounts.Add(documentId, allTerms.Length);
			documentCount++;
		}

		double Tf(string term, string fileName)
		{
			var documentId = documents2Ids[fileName];
			var document = term2documents[term].FirstOrDefault(x => x.DocumentId == documentId);
			if (document == null)
				return 0.0;
			return (double)document.TermCount/termCounts[documentId];
		}

		double Idf(string term)
		{
			return Math.Log((double)documentCount/term2documents[term].Count);
		}

		static IEnumerable<string> Tokenize(string fileName)
		{
			return new WordTokenizer(new LineTokenizer(fileName).Tokenize()).Tokenize();
		}

		readonly IDictionary<string, List<DocumentIdWithCount>> term2documents = new Dictionary<string, List<DocumentIdWithCount>>();
		readonly IDictionary<int, int> termCounts = new Dictionary<int, int>();
		readonly IDictionary<string, int> documents2Ids = new Dictionary<string, int>();
		int documentCount;

		private class DocumentIdWithCount
		{
			public int DocumentId { get; set; }
			public int TermCount { get; set; }
		}
	}
}