using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace IndexingService
{
	public class Index
	{
		public Index(string[] terms, IndexEntry[] entries)
		{
			this.terms = terms;
			this.entries = entries;
		}

		public void Save(string dirName)
		{
			File.WriteAllLines(GetTermListFile(dirName), terms);
			using(var wr = new BinaryWriter(File.Create(GetIndexFile(dirName))))
			{
				wr.Write(terms.Length);
				foreach(IndexEntry entry in entries)
				{
					int[] postingIds = entry.PostingIds;
					wr.Write(postingIds.Length);
					wr.Write(entry.IDF);
					for(int i = 0; i < postingIds.Length; i++)
					{
						wr.Write(postingIds[i]);
						wr.Write(entry.TFs[i]);
					}
				}
			}
		}

		public static Index Load(string dirName)
		{
			IndexEntry[] indexEntries;
			using(var rdr = new BinaryReader(File.OpenRead(GetIndexFile(dirName))))
			{
				indexEntries = new IndexEntry[rdr.ReadInt32()];
				for(int i = 0; i < indexEntries.Length; i++)
				{
					var postings = new int[rdr.ReadInt32()];
					var tfs = new float[postings.Length];
					float idf = rdr.ReadSingle();
					for(int j = 0; j < postings.Length; j++)
					{
						postings[j] = rdr.ReadInt32();
						tfs[j] = rdr.ReadSingle();
					}
					indexEntries[i] = new IndexEntry {PostingIds = postings, TFs = tfs, IDF = idf};
				}
			}
			return new Index(File.ReadAllLines(GetTermListFile(dirName)), indexEntries);
		}

		public IEnumerable<WeightedPosting> GetPostingsByQuery(ParsedQuery query)
		{
			Func<int, bool> isValidTermId = x => x >= 0;
			Func<string, int> lookupTermId = x => Array.BinarySearch(terms, x);
			Func<string[], int[]> lookupMultipleTermIds = x => x.Select(lookupTermId).ToArray();
			var listOfTermIds = query.TermsWithSynonyms.Select(lookupMultipleTermIds);
			if(!listOfTermIds.Any() || listOfTermIds.Any(x => x.All(y => y < 0)))
				return Enumerable.Empty<WeightedPosting>();
			var postingSets =
				listOfTermIds.Select(termIds =>
					termIds.Where(isValidTermId).Select(tid =>
						entries[tid]
						.PostingIds
						.Select(pid =>
							new PostingWithFoundTerm {PostingId = pid, Term = new FoundTerm {TermId = tid, IsExactMatch = tid == termIds.First()}})
						.ToArray())
					.Aggregate(Union));
			var suitablePostings = postingSets
				.Skip(1)
				.Aggregate(
					postingSets
						.First()
						.Select(x => new PostingWithFoundListOfTerms {PostingId = x.PostingId, Terms = new List<FoundTerm> {x.Term}})
						.ToArray(),
					Intersect);
			Func<FoundTerm, IndexEntry> entryByTerm = x => entries[x.TermId];
			return suitablePostings
				.Select(
				posting =>
				new WeightedPosting
					{
						PostingId = posting.PostingId,
						ExactWordMatchCount = posting.Terms.Count(x => x.IsExactMatch),
						TFs =
							posting.Terms
							.Select(term => entryByTerm(term).TFs[Array.BinarySearch(entryByTerm(term).PostingIds, posting.PostingId)])
							.ToArray(),
						IDFs =
							posting.Terms
							.Select(term => entryByTerm(term).IDF)
							.ToArray(),
					});
		}

		private static PostingWithFoundListOfTerms[] Intersect(PostingWithFoundListOfTerms[] a, PostingWithFoundTerm[] b)
		{
			int aIdx = 0;
			int bIdx = 0;
			int oldTermCount = a.First().Terms.Count;
			while(aIdx < a.Length && bIdx < b.Length)
			{
				PostingWithFoundListOfTerms aItem = a[aIdx];
				PostingWithFoundTerm bItem = b[bIdx];
				if(aItem.PostingId < bItem.PostingId)
					aIdx++;
				else if(bItem.PostingId < aItem.PostingId)
					bIdx++;
				else
				{
					aItem.Terms.Add(bItem.Term);
					aIdx++;
					bIdx++;
				}
			}
			return a.Where(x => x.Terms.Count > oldTermCount).ToArray();
		}

		private static PostingWithFoundTerm[] Union(PostingWithFoundTerm[] list1, PostingWithFoundTerm[] list2)
		{
			var result = new List<PostingWithFoundTerm>();
			int i1 = 0;
			int i2 = 0;
			while(i1 < list1.Length && i2 < list2.Length)
			{
				PostingWithFoundTerm item1 = list1[i1];
				PostingWithFoundTerm item2 = list2[i2];
				if(item1.PostingId < item2.PostingId)
				{
					result.Add(item1);
					i1++;
				}
				else if(item2.PostingId < item1.PostingId)
				{
					result.Add(item2);
					i2++;
				}
				else
				{
					result.Add(item1.Term.IsExactMatch ? item1 : item2);
					i1++;
					i2++;
				}
			}
			while(i1 < list1.Length)
			{
				result.Add(list1[i1]);
				i1++;
			}
			while(i2 < list2.Length)
			{
				result.Add(list2[i2]);
				i2++;
			}
			return result.ToArray();
		}

		private static string GetTermListFile(string dirName)
		{
			return Path.Combine(dirName, "termList.txt");
		}

		private static string GetIndexFile(string dirName)
		{
			return Path.Combine(dirName, "index");
		}

		private readonly IndexEntry[] entries;
		private readonly string[] terms;

		private class FoundTerm
		{
			public int TermId;
			public bool IsExactMatch;
		}

		private class PostingWithFoundTerm
		{
			public int PostingId;
			public FoundTerm Term;
		}

		private class PostingWithFoundListOfTerms
		{
			public int PostingId;
			public List<FoundTerm> Terms;
		}
	}
}