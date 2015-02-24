using System;
using System.Linq;
using System.Collections.Generic;

namespace RegEx
{
	public class RegularLanguageState : IEquatable<RegularLanguageState>
	{
		readonly Dictionary<char, List<ConstantExpression>> positions = 
			new Dictionary<char, List<ConstantExpression>>();
		bool containsTerminalPosition;
		
		public RegularLanguageState(bool containsTerminalPosition)
		{
			this.containsTerminalPosition = containsTerminalPosition;
		}

		public RegularLanguageState() : this(false)
		{}

		public void AddPosition(ConstantExpression expr)
		{
			GetOrCreateGroup(expr.Character).Add(expr);
		}

		public void UnionWith(RegularLanguageState state)
		{
			containsTerminalPosition |= state.ContainsTerminalPosition();

			foreach (char group in state.GetPositionGroups())
			{
				GetOrCreateGroup(group).AddRange(state.GetPositions(group));
			}
		}

		public bool IsEmpty
		{
			get
			{
				return positions.Count == 0 && containsTerminalPosition == false;
			}
		}

		public static RegularLanguageState Union(RegularLanguageState a, RegularLanguageState b)
		{
			var res = new RegularLanguageState();
			
			res.UnionWith(a);
			res.UnionWith(b);

			return res;
		}

		public ICollection<char> GetPositionGroups()
		{
			return positions.Keys;
		}

		public ICollection<ConstantExpression> GetPositions(char group)
		{
			return positions[group];
		}

		public bool ContainsTerminalPosition()
		{
			return containsTerminalPosition;
		}

		public override bool Equals(object other)
		{
			if (ReferenceEquals(null, other)) return false;
			if (ReferenceEquals(this, other)) return true;
			if (other.GetType() != typeof (RegularLanguageState)) return false;
			return Equals((RegularLanguageState) other);
		}

		// TODO(dfyz): invent a cleaner way to compare two dictionaries
		public bool Equals(RegularLanguageState other)
		{
			if (ReferenceEquals(null, other)) return false;
			if (ReferenceEquals(this, other)) return true;
			if (containsTerminalPosition != other.containsTerminalPosition) return false;
			if (positions.Count != other.positions.Count) return false;
			foreach (var key in positions.Keys)
			{
				if (!other.positions.ContainsKey(key)) return false;
				if (!EqualLists(positions[key], other.positions[key])) return false;
			}
			return true;
		}

		public override int GetHashCode()
		{
			int hash = containsTerminalPosition.GetHashCode();
			foreach (char group in GetPositionGroups())
			{
				foreach (ConstantExpression position in GetPositions(group))
				{
					hash ^= group.GetHashCode() + position.GetHashCode();
				}
			}
			return hash;
		}

		public override string ToString()
		{
			return GetHashCode().ToString();
		}

		private List<ConstantExpression> GetOrCreateGroup(char group)
		{
			if(!positions.ContainsKey(group))
				positions.Add(group, new List<ConstantExpression>());

			return positions[group];
		}

		private bool EqualLists(List<ConstantExpression> list1, List<ConstantExpression> list2)
		{
			var set1 = new HashSet<ConstantExpression>(list1);
			var set2 = new HashSet<ConstantExpression>(list2);
			return set1.SetEquals(set2);
		}
	}
}