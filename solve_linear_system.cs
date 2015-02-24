/*
Problem (2):
1,23 3,14 1,00 7,90
2,35 5,97 0,00 14,28
3,46 2,18 2,80 8,89
LUD:
1,23 2,55 0,81 6,42
2,35 -0,02 95,00 40,50
3,46 -6,64 630,80 0,41
LUD solution (2,14 1,55 0,41)
Diff: -0,01 0,00 -3,04
Gauss solution (0,99 2,00 0,34)
Diff: -0,02 0,02 0,06

Problem (4):
1,2345 3,1415 1,0000 7,8975
2,3456 5,9690 0,0000 14,2836
3,4567 2,1828 2,8000 8,8863
LUD:
1,2345 2,5448 0,8100 6,3973
2,3456 -0,0001 18999,0000 7219,0000
3,4567 -6,6138 125655,5863 0,3799
LUD solution (2,8325 1,2799 0,3799)
Diff: 0,0001 0,0000 -4,7623
Gauss solution (1,0001 2,0000 0,3804)
Diff: -0,0001 -0,0002 -0,0004

Problem (6):
1,234500 3,141500 1,000000 7,897500
2,345600 5,969000 0,000000 14,283600
3,456700 2,182800 2,800000 8,886300
LUD:
1,234500 2,544755 0,810045 6,397327
2,345600 0,000023 -82610,521739 -31390,000000
3,456700 -6,613655 -546357,490235 0,380000
LUD solution (1,004425 1,998261 0,380000)
Diff: 0,000000 0,000001 -0,011500
Gauss solution (0,999999 2,000000 0,379994)
Diff: -0,000002 0,000003 0,000007
*/

using System;
using System.Linq;

class Solution
{
	class SmartDecimal
	{
		public static int Places { get; set; }

		public SmartDecimal(decimal value)
		{
			inner = Math.Round(value, Places, MidpointRounding.AwayFromZero);
		}

		public override string ToString()
		{
			return inner.ToString("F" + Places);
		}

		public static SmartDecimal operator*(SmartDecimal lhs, SmartDecimal rhs)
		{
			return new SmartDecimal(lhs.inner * rhs.inner);
		}

		public static SmartDecimal operator/(SmartDecimal lhs, SmartDecimal rhs)
		{
			return new SmartDecimal(lhs.inner / rhs.inner);
		}

		public static SmartDecimal operator+(SmartDecimal lhs, SmartDecimal rhs)
		{
			return new SmartDecimal(lhs.inner + rhs.inner);
		}

		public static SmartDecimal operator -(SmartDecimal lhs, SmartDecimal rhs)
		{
			return new SmartDecimal(lhs.inner - rhs.inner);
		}

		public static explicit operator decimal(SmartDecimal x)
		{
			return x.inner;
		}

		readonly decimal inner;
	}


	SmartDecimal[,] LU_Decomposition(SmartDecimal[,] a)
	{
		int n = a.GetLength(0);
		var d = new SmartDecimal[n, n+1];
		for(int j = 0; j < n; j++)
		{
			for(int i = j; i < n; i++)
			{
				d[i, j] = a[i, j] - Enumerable.Range(0, j).Aggregate(new SmartDecimal(0), (acc, k) => acc +d[i, k]*d[k, j]);
			}

			for(int i = j+1; i < n +1; i++)
			{
				d[j, i] = (a[j, i] - Enumerable.Range(0, j).Aggregate(new SmartDecimal(0), (acc, k) => acc + d[j, k] * d[k, i])) / d[j, j];
			}
		}
		return d;
	}

	double[,] original = new[,]
		{
			{1.2345, 3.1415, 1, 7.8975},
			{2.3456, 5.9690, 0, 14.2836},
			{3.4567, 2.1828, 2.8, 8.8863}
		};

	SmartDecimal[,] PrepareProblem(int places)
	{
		SmartDecimal.Places = places;
		var res = new SmartDecimal[original.GetLength(0),original.GetLength(1)];
		for(int i = 0; i < original.GetLength(0); i++)
			for(int j = 0; j < original.GetLength(1); j++)
				res[i, j] = new SmartDecimal((decimal)original[i,j]);
		return res;
	}

	SmartDecimal[] GaussSolve(SmartDecimal[,] a)
	{
		int n = a.GetLength(0);

		Func<int, int> findMaxElemInd = i =>
		                                	{
		                                		int maxElemInd = -1;
		                                		for (int k = i; k < n; k++)
		                                			if (maxElemInd == -1 ||
		                                			    ((decimal) a[k, i] != 0m &&
		                                			     Math.Abs((decimal) a[k, i]) > Math.Abs((decimal) a[maxElemInd, i])))
		                                				maxElemInd = k;
		                                		return maxElemInd;
		                                	};
		Action<int, int> swapLines = (i, j) =>
		                             	{
		                             		for(int k = 0; k <= n; k++)
		                             		{
		                             			var tmp = a[i, k];
		                             			a[i, k] = a[j, k];
		                             			a[j, k] = tmp;
		                             		}
		                             	};

		Action<int, int, SmartDecimal> subtractLine = (j, i, x) =>
		                                         	{
														for(int k = 0; k <= n; k++)
															a[j, k] -= a[i, k]*x;
		                                         	};

		for (int i = 0; i < n; i++)
		{
			int maxElem = findMaxElemInd(i);
			swapLines(maxElem, i);
			for(int j = i+1; j < n; j++)
				subtractLine(j, i, a[j,i] / a[i,i]);
		}

		for(int i = n-1; i >= 0; i--)
		{
			for(int j = i-1; j >= 0; j--)
				subtractLine(j, i, a[j, i] / a[i, i]);
		}

		return Enumerable.Range(0, n).Select(i => a[i, n]/a[i, i]).ToArray();
	}

	void Run()
	{
		Action<SmartDecimal[,]> print = x =>
		                                	{
		                                		for (int i = 0; i < x.GetLength(0); i++)
		                                		{
		                                			for (int j = 0; j < x.GetLength(1); j++)
		                                				Console.Write(x[i, j] + " ");
		                                			Console.WriteLine();
		                                		}
		                                	};

		Action<string, SmartDecimal[], SmartDecimal[,]> printSolution = (str, x, p) =>
		                                               	{
		                                               		Console.WriteLine("{0} ({1} {2} {3})", str, x[0], x[1], x[2]);
															Console.Write("Diff: ");
															for (int i = 0; i < 3; i++)
																Console.Write(
																	p[i, 3] - Enumerable.Range(0, 3).Aggregate(new SmartDecimal(0m), (acc, j) => acc + x[j]*p[i, j])
																	 + " ");
		                                               		Console.WriteLine();
		                                               	};
		
		for (int i = 2; i <= 6; i += 2)
		{
			var problem = PrepareProblem(i);
			var lud = LU_Decomposition(problem);
			var gaussized = new[,]
				{
					{new SmartDecimal(1m),				lud[0,1],			lud[0,2], lud[0,3] },
					{new SmartDecimal(0m), new SmartDecimal(1m),			lud[1,2], lud[1,3] },
					{new SmartDecimal(0m), new SmartDecimal(0m), new SmartDecimal(1m), lud[2,3] },
				};

			var lud_xs = GaussSolve(gaussized);
			Console.WriteLine("Problem ({0}):", i);
			print(problem);

			Console.WriteLine("LUD:");
			print(lud);

			printSolution("LUD solution", lud_xs, problem);
			printSolution("Gauss solution", GaussSolve(problem), problem);
			Console.WriteLine();
		}
	}

	static void Main()
	{
		new Solution().Run();
	}
}