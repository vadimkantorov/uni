using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using Okulobot.Tactics;

namespace Okulobot.Strategies
{
	public class BaseStrategy : Strategy
	{
		public BaseStrategy()
		{
			AvailableTactics.Add(new DestroyTactics());
			AvailableTactics.Add(new DeadlyTactics());
			//AvailableTactics.Add(new ChargeTactics());
			//AvailableTactics.Add(new RollbackTactics(this));

			DefaultTactics = new DefaultTactics();
			AvailableTactics.Add(DefaultTactics);
		}

		protected DefaultTactics DefaultTactics { get; private set; }

		static Dictionary<string, string> hc = new Dictionary<string, string>()
		                                       	{
		                                       		{"bob/main",
@"0 10 4.71
6 17 4.89
7 25 6.11
16 27 6.11
21 32 4.35
21 39 5.24
25 48 4.54
23 57 4.54
20 66 5.41
23 71 0
33 71 5.93
41 74 0.52
33 78 0.52"},

 {"bob/second",
@"0 10 4.71
6 17 4.89
7 25 6.11
16 27 6.11
21 32 4.35
21 39 2.62
25 48 4.54
23 57 4.54
20 66 5.41
23 71 0
33 71 5.93
41 74 0.52
33 78 0.52"},

		   {"bob/defensive",
@"0 10 4.71
6 17 4.89
7 25 6.11
16 27 6.11
21 32 4.35
24 39 2.79
24 33 6.11
29 41 4.89
32 49 3.84
23 53 0.17
23 57 4.54
20 66 5.41
23 71 0
33 71 5.93
41 74 0.52
33 78 0.52"},

		   {"jack.txt",
@"0 10 4.71
6 6 5.41
13 13 0
23 13 6.11
27 14 4.71
27 23 0
37 23 0
46 23 5.76
54 28 5.76
62 34 5.76
71 36 4.89
71 46 4.89
72 55 4.89
73 57 0.35
77 57 3.49
77 54 6.11"},
			{"mark.txt",
@"0 10 4.89
6 7 4.89
6 17 4.89
7 26 6.11
16 27 6.11
20 27 4.71
20 37 4.71
17 46 4.71
17 55 5.06
21 64 5.06
21 70 6.11
31 70 6.11
39 74 0.35
48 71 0.35
57 67 0.52
65 62 0.52
73 58 1.75
77 56 3.49
77 57 3.38"},
		   {"gordon/main",
@"0 10 5.76
6 7 5.76
16 13 5.93
26 14 5.59
28 22 5.93
39 23 0
50 19 0
61 18 0.52
66 8 0.52
73 5 6.11
84 7 6.11
94 8 0.7
106 8 0.7
112 8 0.7
123 5 6.28
134 8 5.06
136 19 5.59
147 22 5.41
152 32 5.24
153 43 5.24
155 52 5.93
167 52 0
176 59 5.41
179 70 3.49
167 70 3.84
161 74 4.89
168 82 4.54
166 93 3.32
155 94 3.14
143 94 3.49
132 98 3.67
125 107 4.19
124 118 4.01
116 126 2.79
113 120 3.31
102 122 1.57
102 122 1.57
90 122 1.22
90 122 1.22
80 120 3.31
80 120 3.31
70 120 1.39
70 120 1.39
63 111 3.31
63 111 3.31
61 107 5.58
54 107 2.27
54 107 2.27
45 100 0.87
35 94 3.66
35 94 3.66
32 85 5.58
33 77 6.11
41 76 2.96
33 69 4.01
33 69 4.01
22 71 1.92
19 63 1.39
17 52 5.58
11 42 0.35
16 33 5.58
21 33 2.09
16 25 3.83
8 25 1.74
8 14 6.28
6 7 5.76"
                                                                                                                                                                                                                                   	
	},
	{"gordon/defensive",
	@"0 10 5.76
6 7 5.76
16 13 5.93
26 14 5.59
28 22 5.93
39 23 0
50 19 4.89
57 23 2.45
57 12 5.59
67 12 1.92
67 4 0.35
77 4 5.41
88 8 5.94
100 8 5.94
111 8 0.87
122 4 6.11
133 8 4.89
133 11 4.19
125 18 5.94
125 12 3.84
137 12 5.24
148 16 3.67
158 22 5.76
167 24 0.7
173 19 1.57
171 11 5.59
183 11 5.59
185 14 0.53
190 21 2.45
194 28 2.45
182 28 2.45
178 32 5.41
172 39 2.45
167 42 5.42
167 54 5.77
175 62 1.75
180 62 4.02
180 72 3.5
170 72 2.28
160 72 5.42
165 80 2.62
174 87 3.32
166 95 5.59
162 106 3.85
150 106 4.89
139 110 2.97
128 112 4.54
120 120 3.67
108 120 1.93
96 120 1.23
84 120 3.67
72 120 1.4
60 120 2.28
55 110 3.67
45 105 2.1
33 105 1.23
33 93 1.23
33 83 3.85
33 77 5.77
40 77 1.93
33 69 4.02
26 72 2.28
19 72 0.88
22 63 1.93
22 51 5.59
32 45 2.28
26 38 2.62
17 33 4.89
21 32 2.1
10 28 2.45
5 22 1.05
13 21 2.45
13 13 5.94
16 13 5.93"}, {"lenny.txt",
		   @"0 10 5.06
6 6 5.06
7 20 5.06
7 27 6.28
19 27 4.71
21 41 5.24
27 54 4.19
20 66 5.41
22 71 6.11
36 71 5.24
40 75 0.7
52 68 0.17
65 61 0.17
73 57 1.92
71 45 1.05
73 31 0.87
85 22 5.76
88 26 5.24
89 34 0.7
92 30 1.05
93 26 5.76
107 26 5.06
107 33 0.52
110 32 1.05
112 22 5.76
126 25 5.76
140 30 0.17
153 36 4.71
154 50 6.11
168 51 1.05
170 37 6.11
179 34 1.22
181 26 6.28
193 26 1.4
193 14 3.14
179 12 3.14
172 13 5.06
172 23 3.49
157 23 3.14
143 19 2.62
134 11 1.92
133 11 1.92
132 5 2.79
117 5 3.49
110 7 3.32
96 8 3.32
82 7 2.97
71 5 3.84
64 10 4.71
64 19 3.49
50 22 3.49
37 23 3.49
27 23 1.4
27 13 2.97
12 13 4.01
9 17 4.01
7 20 5.06"
	   	
	}
													
	};

		protected static IEnumerable<WayPoint> ImportWayPoints(string filePath)
		{
			var list = new List<WayPoint>();
			foreach (var s in new StringReader(hc[filePath]).ReadToEnd().Split(new [] {Environment.NewLine},StringSplitOptions.RemoveEmptyEntries))
			{
				var xs = s.Split();
				var wp = new WayPoint{Coordinates = new Point
             	{
             		X = Convert.ToInt32(xs[0]),
             		Y = Convert.ToInt32(xs[1])
             	}};

				if (xs.Length == 3)
					wp.Angle = Math.PI*2 - Convert.ToDouble(xs[2], CultureInfo.InvariantCulture) + 2*Math.PI;

				list.Add(wp);
			}

			for (int i = 0; i < list.Count; i++)
			{
				if (list[i].Angle < 2*Math.PI)
					list[i].Angle = AngleUtils.GetDirection(list[i].Coordinates, list[(i + 1)%list.Count].Coordinates);
				list[i].Angle = AngleUtils.Normalize(list[i].Angle);
			}
			return list;
		}
	}
}