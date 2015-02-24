using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;

namespace IndexingService
{
	public class IndexBuilder
	{
		public IndexBuilder(string directory, Action<int> onIndexedPosting)
		{
			fileNames = Directory.GetFiles(directory).OrderBy(x => x).ToArray();
			this.onIndexedPosting = onIndexedPosting;
		}

		public Index BuildIndex()
		{
			int postingCount = fileNames.Length;
			for(int i = 1; i <= postingCount; i++)
				BuildIndexForPosting(i, fileNames[i - 1]);
			var entries =
				from pids in term2postings.Values
				let idf = Math.Log((double)postingCount / pids.Count)
				let tfs = pids.Select(x => (float)x.TermCount / termCounts[x.PostingId - 1])
				select
					new IndexEntry
						{
							PostingIds = pids.Select(x => x.PostingId).ToArray(),
							TFs = tfs.ToArray(),
							IDF = (float)idf,
						};
			return new Index(term2postings.Keys.ToArray(), entries.ToArray());
		}

		private void BuildIndexForPosting(int postingId, string fileName)
		{
			var allTerms = new LowercasingTokenizer(File.ReadAllText(fileName, Encoding.GetEncoding(1251))).Tokenize().ToArray();
			foreach(string term in allTerms)
			{
				if(!term2postings.ContainsKey(term))
					term2postings.Add(term, new List<PostingIdWithCount>());
				var postings = term2postings[term];
				if(postings.Count == 0 || postings.Last().PostingId != postingId)
					postings.Add(new PostingIdWithCount {PostingId = postingId});
				postings.Last().TermCount++;
			}
			termCounts.Add(allTerms.Length);
			onIndexedPosting(postingId);
		}

		private readonly IDictionary<string, List<PostingIdWithCount>> term2postings = new SortedDictionary<string, List<PostingIdWithCount>>();
		private readonly IList<int> termCounts = new List<int>();
		private readonly string[] fileNames;
		private readonly Action<int> onIndexedPosting;

		private class PostingIdWithCount
		{
			public int PostingId { get; set; }
			public int TermCount { get; set; }
		}
	}
}