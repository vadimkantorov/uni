using System;
using System.IO;
using System.Linq;
using IndexingService;

namespace IndexStats
{
	class MainClass
	{
		public static void Main (string[] args)
		{
			if(args.Length < 2)
			{
				Console.WriteLine("Usage: IndexStats index-dir action [args]");
				Environment.Exit(1);
			}
			var index = Index.Load(args[0]);
			var action = args[1];
			if(action == "termDocs")
			{
				var term = args[2];
				var query = new ParsedQuery { TermsWithSynonyms = new[] {new[]{term}}};
				Console.WriteLine(string.Join(" ", index.GetPostingsByQuery(query).Select(x => x.PostingId.ToString()).ToArray()));
			}
			else if(action == "playGame")
			{
				var urls = File.ReadAllLines("urls.txt");
				using(var output = new StreamWriter("ranking.txt"))
				{
					var reqNum = 0;
					foreach(var req in File.ReadAllLines("requests.txt"))
					{
						var parsedQuery = RawQueryParser.Parse(req, null, null, null, new MorphologyAnalyzer("127.0.0.1"));
						var results = index
							.GetPostingsByQuery(parsedQuery)
							.Rank()
							.Take(10);
						var pos = 0;
						foreach(var res in results)
						{
							output.WriteLine("{0} {1} {2}", reqNum, pos, urls[res.PostingId - 1]);
							pos++;
						}
						reqNum++;
					}
				}
			}
		}
	}
}
