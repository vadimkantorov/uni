using System.Collections.Generic;

namespace RegEx
{
	public class TransitionTableBuilder<TState>
	{
		readonly List<EdgeBuilder<TState>> rawEdgeBuilders = new List<EdgeBuilder<TState>>();

		public TransitionTableBuilder<TState> Add(EdgeBuilder<TState> eb)
		{
			rawEdgeBuilders.Add(eb);
			
			return this;
		}

		public Dictionary<TState, Dictionary<char, TState>> Build()
		{
			var res = new Dictionary<TState, Dictionary<char, TState>>();
			
			foreach (var edgeBuilder in rawEdgeBuilders)
			{
				Dictionary<char, TState> edges;

				if(!res.TryGetValue(edgeBuilder.Source, out edges))
				{
					edges = new Dictionary<char, TState>();
					res[edgeBuilder.Source] = edges;
				}

				edges[edgeBuilder.Label] = edgeBuilder.Target;
			}

			return res;
		}
	}
}