namespace RegEx
{
	public class EdgeBuilder<TState>
	{
		public char Label { get; private set; }
		
		public TState Source { get; private set; }

		public TState Target { get; private set; }

		public EdgeBuilder<TState> From(TState from)
		{
			Source = from;
			
			return this;
		}

		public EdgeBuilder<TState> By(char character)
		{
			Label = character;
			
			return this;
		}

		public EdgeBuilder<TState> To(TState to)
		{
			Target = to;

			return this;
		}
	}
}