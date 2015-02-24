using System;
using System.IO;
using System.Linq;
using IndexingService;

namespace Indexer
{
	class Program
	{
		static int Main(string[] args)
		{
			if(args.Length != 1)
			{
				Console.WriteLine("Usage: Indexer.exe <collection directory>");
				return 1;
			}
			var collectionDir = args.First();
			var index = new IndexBuilder(collectionDir, OnIndexedPosting).BuildIndex();
			var indexDir = Path.Combine(Directory.GetParent(collectionDir).FullName, "index");
			if(!Directory.Exists(indexDir))
				Directory.CreateDirectory(indexDir);
			index.Save(indexDir);
			return 0;
		}

		private static void OnIndexedPosting(int postingId)
		{
			if(postingId % 100 == 0)
				Console.WriteLine(postingId);
		}
	}
}
