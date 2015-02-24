namespace RegEx
{
	public class LanguageRecognizer<TState>
	{
		readonly DFA<TState> dfa;

		public LanguageRecognizer(DFA<TState> dfa)
		{
			this.dfa = dfa;
		}

		public bool Accepts(string word)
		{
			TState curState = dfa.StartState;
			foreach (char c in word)
			{
				TState newState;
				if (!dfa.TransitionTable.ContainsKey(curState))
					return false;
				if(!dfa.TransitionTable[curState].TryGetValue(c, out newState))
					return false;
				curState = newState;
			}
			return dfa.AcceptingStates.Contains(curState);
		}
	}
}