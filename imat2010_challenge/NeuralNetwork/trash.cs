using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using AForge.Neuro;
using AForge.Neuro.Learning;
using System.IO;
using System.Globalization;

namespace AnnAppro
{
	class Program
	{
		const double BASE = 115;
		const int INPUTS = 12;
		const int OUTPUTS = 36-INPUTS;
		const double MEAN = 70;
		const double DEV2 = 50*50;

		static double There(double x)
		{
			return (x - 70) / DEV2;
		}

		static double Here(double x)
		{
			return x * DEV2 + 70;
		}

		static void Main(string[] args)
		{
			
			const string data = @"..\..\..\data\";
			
			foreach(var file in Directory.GetFiles(data).Where(x => !x.EndsWith("ans") && !x.EndsWith("txt")))
			{
				var outPath = Path.Combine(Path.Combine(data, "zzz"),Path.GetFileName(file).Split('_')[0] + ".txt");
				File.WriteAllText(outPath,"");
				var zzz = File.ReadAllLines(file)
				.Select(x => new Line(x))
				.GroupBy(x => x.Time)
				.ToDictionary(x => x.Key, x => x.OrderBy(y => y.Day).ToArray());

				var end = new DateTime(2010, 4,2,21,50,0);
				for(var dt = new DateTime(2010, 4,2,16,0,0); dt <= end; dt += TimeSpan.FromMinutes(10))
				{
					var line = dt.ToShortTimeString() + " ";
					for (int i = 11; i <= 40; i++)
					{
						if ((i + 1) % 7 >= 5) continue;
						if (i == 34) continue;

						line += zzz[dt].ElementAt(i - 11).Velocity + " ";
					}
					File.AppendAllText(outPath, line + Environment.NewLine, Encoding.ASCII);
				}
			}

			return;
			/*string edge_data_txt = Path.Combine(data, "edge_data.txt");
			var edgeData = File.ReadAllLines(edge_data_txt).Where(x => !string.IsNullOrEmpty(x)).Select(x => x.Split())
				.ToDictionary(x => Convert.ToInt32(x[0]), x => Convert.ToDouble(x[1], CultureInfo.InvariantCulture));

			var inputsA = new List<double[]>();
			var outputsA = new List<double[]>();

			const int road = 805272;
			var inputFile = File.ReadAllLines(Path.Combine(data, road+"_10_34"));
			var outputFile = File.ReadAllLines(Path.Combine(data, road+"_10_34_ans"));

			var days = inputFile
				.Select(x => new Line(x))
				.GroupBy(x => x.Day)
				.ToDictionary(x => x.Key, x => x.OrderBy(y => y.Time).ToArray());

			double mean = days.SelectMany(x => x.Value).Select(x => x.Velocity).Average();

			double[] day34 = null;
			for (int i = 11; i <= 40; i++)
			{
				if((i + 1) % 7 >= 5) continue;

				var ip = days[i].Take(INPUTS).Select(x => There(x.Velocity)).ToArray();
				if (i == 34)
				{
					day34 = ip;
					continue;
				}

				inputsA.Add(ip);
				outputsA.Add(days[i].Skip(INPUTS).Select(x => There(x.Velocity)).ToArray());
			}

			var inputs = inputsA.ToArray();
			var outputs = outputsA.ToArray();

			var ann = new ActivationNetwork(new BipolarSigmoidFunction(), INPUTS, 5, OUTPUTS);
			ann.Randomize();
			var teacher = new BackPropagationLearning(ann) { LearningRate = 0.1};

			const int iterations = 100000;
			const int reportFreq = iterations / 20;

			var zzz = outputFile.Select(x => new Line(x)).ToArray();
			
			for (int i = 0; i < iterations; i++)
			{
				var e = teacher.RunEpoch(inputs, outputs);
				if (i % reportFreq == 0)
				{
					Console.Write("Training. Error rate: " + e / inputs.Length * DEV2);
					var res = ann.Compute(day34);
					double err = res.Select((x, j) => Math.Abs(Here(x) - zzz[j].Velocity) * edgeData[road] / 120.0 * (1 + 0.1 * 10 * j / 4)).Sum();
					Console.Write(" Estimate: {0:F2}", + err / OUTPUTS);

 					double err2 = res.Select((x, j) => Math.Abs(mean - zzz[j].Velocity) * edgeData[road] / 120.0 * (1 + 0.1 * 10 * j / 4)).Sum();
					Console.WriteLine(" Mean estimate: {0:F2}", + err2 / OUTPUTS);
				}
				//if (Math.Abs(e) < 1e-2)
				//	break;
			}
			var res2 = ann.Compute(day34);
			File.WriteAllLines(@"..\..\xyu.txt",res2.Select(x => Here(x).ToString()).ToArray());*/
		}
	}
}
