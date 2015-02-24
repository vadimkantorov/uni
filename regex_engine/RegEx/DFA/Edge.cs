namespace RegEx
{
	public class Edge
	{
		public static EdgeBuilder<TState> From<TState>(TState state)
		{
			return new EdgeBuilder<TState>().
				From(state);
		}
	}
}