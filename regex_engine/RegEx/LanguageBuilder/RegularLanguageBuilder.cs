using System;
using System.Collections.Generic;
using System.Linq;

namespace RegEx
{
	public class RegularLanguageBuilder
	{
		readonly Expression root;
		readonly RegularExpressionTraverser traverser;

		class RegularExpressionTraverser
		{
			readonly Dictionary<Expression, RegularLanguageState> firstPos = 
				new Dictionary<Expression, RegularLanguageState>();

			readonly Dictionary<Expression, RegularLanguageState> lastPos =
				new Dictionary<Expression, RegularLanguageState>();

			readonly Dictionary<ConstantExpression, RegularLanguageState> followPos =
				new Dictionary<ConstantExpression, RegularLanguageState>();

			readonly Dictionary<Expression, bool> nullable = 
				new Dictionary<Expression, bool>();
			
			public RegularLanguageState GetFirstPos(Expression expr)
			{
				if (!firstPos.ContainsKey(expr))
					firstPos.Add(expr, new RegularLanguageState());

				return firstPos[expr];
			}

			public bool IsNullable(Expression expr)
			{
				if (!nullable.ContainsKey(expr))
					nullable.Add(expr, false);

				return nullable[expr];
			}

			public RegularLanguageState GetLastPos(Expression expr)
			{
				if (!lastPos.ContainsKey(expr))
					lastPos.Add(expr, new RegularLanguageState());
				
				return lastPos[expr];
			}

			public RegularLanguageState GetFollowPos(ConstantExpression expr)
			{
				if(!followPos.ContainsKey(expr))
					followPos.Add(expr, new RegularLanguageState());
				
				return followPos[expr];
			}

			private void DFS(Expression start)
			{
				foreach (Expression expr in start)
					DFS(expr);
				
				if(start is TerminalExpression)
				{
					var newState = new RegularLanguageState(true);
					
					firstPos[start] = newState;
					lastPos[start] = newState;
					nullable[start] = false;
				}
				else if(start is ConstantExpression)
				{
					var newState = new RegularLanguageState();
					newState.AddPosition((ConstantExpression)start);

					firstPos[start] = newState;
					lastPos[start] = newState;
					nullable[start] = false;
				}
				else if(start is ConcatenationExpression)
				{
					var ce = (ConcatenationExpression) start;

					bool leftIsNullable = nullable[ce.Left];
					bool rightIsNullable = nullable[ce.Right];

					if (leftIsNullable)
						firstPos[start] = RegularLanguageState.Union(firstPos[ce.Left], firstPos[ce.Right]);
					else
						firstPos[start] = firstPos[ce.Left];

					if (rightIsNullable)
						lastPos[start] = RegularLanguageState.Union(lastPos[ce.Left], lastPos[ce.Right]);
					else
						lastPos[start] = lastPos[ce.Right];
					
					nullable[start] = leftIsNullable && rightIsNullable;
					
					var leftLastPos = lastPos[ce.Left];
					var rightFirstPos = firstPos[ce.Right];
					
					foreach (char group in leftLastPos.GetPositionGroups())
					{
						foreach (ConstantExpression position in leftLastPos.GetPositions(group))
						{
							GetFollowPos(position).UnionWith(rightFirstPos);
						}
					}
				}
				else if(start is AlternationExpression)
				{
					var ce = (AlternationExpression)start;

					firstPos[start] = RegularLanguageState.Union(firstPos[ce.Left], firstPos[ce.Right]);
					lastPos[start] = RegularLanguageState.Union(lastPos[ce.Left], lastPos[ce.Right]);

					nullable[start] = nullable[ce.Left] || nullable[ce.Right];
				}
				else if(start is IterationExpression)
				{
					var ce = (IterationExpression) start;
					
					firstPos[start] = firstPos[ce.Inner];
					lastPos[start] = lastPos[ce.Inner];

					nullable[start] = true;

                    var leftLastPos = lastPos[ce.Inner];
					var rightFirstPos = firstPos[ce.Inner];

					foreach (char group in leftLastPos.GetPositionGroups())
					{
						foreach (ConstantExpression position in leftLastPos.GetPositions(group))
						{
							GetFollowPos(position).UnionWith(rightFirstPos);
						}
					}
				}
			}

			public RegularExpressionTraverser(Expression root)
			{
				DFS(root);
			}

			public string DebugPrint(Expression tree, string spaces)
			{
				string res = spaces + tree.GetType().Name + " (" + tree.GetHashCode() + ") "
					+ " Nullable: " + IsNullable(tree) + " " + "FirstPos: {";

				foreach (char group in GetFirstPos(tree).GetPositionGroups())
					foreach (ConstantExpression ce in GetFirstPos(tree).GetPositions(group))
						res += ce.GetHashCode() + ", ";
				res += "} LastPos: {";

				foreach (char group in GetLastPos(tree).GetPositionGroups())
					foreach (ConstantExpression ce in GetLastPos(tree).GetPositions(group))
						res += ce.GetHashCode() + ", ";
				res += "}" + Environment.NewLine;


				foreach (Expression expr in tree)
					res += DebugPrint(expr, spaces + spaces);

				return res;
			}
		}

		public RegularLanguageBuilder(Expression root)
		{
			this.root = new ConcatenationExpression(new TerminalExpression(), root);
			this.traverser = new RegularExpressionTraverser(this.root);
		}

		public DFA<RegularLanguageState> BuildLanguage()
		{
			var transitionTableBuilder = new TransitionTableBuilder<RegularLanguageState>();
			var acceptingStates = new HashSet<RegularLanguageState>();
			
			var visited = new HashSet<RegularLanguageState>();
			var toVisit = new HashSet<RegularLanguageState>();

			var startState = traverser.GetFirstPos(root);
			toVisit.Add(startState);
			
			while(toVisit.Count > 0)
			{
				var state = toVisit.First();
				
				toVisit.Remove(state);
                visited.Add(state);
				if (state.ContainsTerminalPosition())
					acceptingStates.Add(state);

				foreach(char group in state.GetPositionGroups())
				{
					var newState = new RegularLanguageState();
					foreach (ConstantExpression pos in state.GetPositions(group))
					{
						newState.UnionWith(traverser.GetFollowPos(pos));
					}

					if(!newState.IsEmpty)
					{
						transitionTableBuilder.Add(
							Edge.From(state).By(group).To(newState)
						);
						if (!visited.Contains(newState) && !toVisit.Contains(newState))
							toVisit.Add(newState);
					}
				}
			}

			return new DFA<RegularLanguageState>(startState,
			                                     visited,
			                                     acceptingStates, transitionTableBuilder.Build());
		}
	}
}