using System;
using System.Collections.Generic;

namespace RegEx
{
	public class StateMachine<TState>
	{
		public TState CurrentState { get; private set; }

		public HashSet<TState> AcceptingStates { get; private set; }

		public Dictionary<TState, Dictionary<char, TState>> TransitionTable { get; private set; }

		public StateMachine(Dictionary<TState, Dictionary<char, TState>> transitionTable,
			TState currentState,
			HashSet<TState> acceptingStates)
		{
			CurrentState = currentState;
			TransitionTable = transitionTable;
			AcceptingStates = acceptingStates;
		}

		public StateMachine<TState> MoveBy(char character)
		{
			TState newState;
			
			if(!TransitionTable[CurrentState].TryGetValue(character, out newState))
				throw new InvalidTransitionException(CurrentState, character);
			
			return new StateMachine<TState>(TransitionTable, newState, AcceptingStates);
		}

		public StateMachine<TState> MoveBy(string line)
		{
			StateMachine<TState> cur = this;
			
			foreach (char c in line)
				cur = cur.MoveBy(c);
			
			return cur;
		}

		public bool Accepts()
		{
			return AcceptingStates.Contains(CurrentState);
		}

		public override string ToString()
		{
			string res = "Current state: " + CurrentState.GetHashCode() + Environment.NewLine +
			       "Accepting states: {";
			foreach (var state in AcceptingStates)
				res += state.GetHashCode() + ", ";

			res += "}" + Environment.NewLine + Environment.NewLine;
			res += "States:" + Environment.NewLine;

			foreach (var state in TransitionTable.Keys)
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