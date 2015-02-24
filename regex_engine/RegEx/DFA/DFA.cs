using System;
using System.Collections.Generic;

namespace RegEx
{
	public class DFA<TState>
	{
		public TState StartState { get; private set; }

		public HashSet<TState> AcceptingStates { get; private set; }

		public HashSet<TState> AllStates { get; private set; }

		public Dictionary<TState, Dictionary<char, TState>> TransitionTable { get; private set; }

		public DFA(TState startState, HashSet<TState> allStates, HashSet<TState> acceptingStates, Dictionary<TState, Dictionary<char, TState>> transitionTable)
		{
			StartState = startState;
			AcceptingStates = acceptingStates;
			AllStates = allStates;
			TransitionTable = transitionTable;
		}

		public override string ToString()
		{
			string res = "Current state: " + StartState.GetHashCode() + Environment.NewLine +
			       "Accepting states: {";
			foreach (var state in AcceptingStates)
				res += state.GetHashCode() + ", ";

			res += "}" + Environment.NewLine + Environment.NewLine;
			res += "States:" + Environment.NewLine;

			foreach (var state in AllStates)
				res += state.GetHashCode() + Environment.NewLine;

			res += Environment.NewLine;

			res += "Transition table:" + Environment.NewLine;

			foreach (var state in TransitionTable.Keys)
			{
				foreach (var kvp in TransitionTable[state])
				{
					res += state.GetHashCode() + " --[" + kvp.Key + "]--> " + kvp.Value.GetHashCode() + Environment.NewLine;
				}
			}

			return res;
		}
	}
}