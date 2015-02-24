using System;

namespace Okulobot
{
	public struct Point : IEquatable<Point>
	{
		public int X { get; set; }

		public int Y { get; set; }

		public bool Equals(Point other)
		{
			return other.X == X && other.Y == Y;
		}

		public override bool Equals(object obj)
		{
			if (ReferenceEquals(null, obj)) return false;
			if (obj.GetType() != typeof (Point)) return false;
			return Equals((Point) obj);
		}

		public override int GetHashCode()
		{
			unchecked
			{
				return (X*397) ^ Y;
			}
		}

		public static bool operator ==(Point left, Point right)
		{
			return left.Equals(right);
		}

		public static bool operator !=(Point left, Point right)
		{
			return !left.Equals(right);
		}

		public override string ToString()
		{
			return string.Format("[X: {0}, Y: {1}]", X, Y);
		}
	}
}