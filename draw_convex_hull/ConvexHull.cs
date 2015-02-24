using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Drawing;

namespace ConvexHull
{
	class ConvexHullBuilder
	{
		static int CrossProduct(Point p1, Point p2, Point p3)
		{
			return (p2. X - p1.X) * (p3.Y - p1.Y) - (p2.Y - p1.Y) * (p3.X - p1.X);
		}
		
		public IList<Point> BuildConvexHull(IList<Point> ps)
		{
			var pivot = ps.OrderBy(p => p.Y).ThenBy(p => p.X).First();
			var sortedByPolarAngle = ps.OrderBy(p => Math.Atan2(p.Y - pivot.Y, p.X - pivot.X)).ToArray();
			
			var stack = new List<Point>(sortedByPolarAngle.Take(3));
			for (int i = 3; i < sortedByPolarAngle.Length; i++)
			{
				while (CrossProduct(stack[stack.Count - 2], stack[stack.Count - 1], sortedByPolarAngle[i]) <= 0)
					stack.RemoveAt(stack.Count - 1);
				stack.Add(sortedByPolarAngle[i]);
			}

			return stack;
		}

		public int PointsOnHull(IList<Point> ps, IList<Point> hull)
		{
			int res = 0;
			foreach (var p in ps)
			{
				for (int i = 0; i < hull.Count; i++)
				{
					if (CrossProduct(hull[i], hull[(i + 1)%hull.Count], p) == 0)
					{
						res++;
						break;
					}
				}
			}
			return res;
		}
	}
}
