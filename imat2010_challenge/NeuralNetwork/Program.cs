using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Text;
using AForge.Neuro;
using AForge.Neuro.Learning;

namespace AnnAppro
{
	public class Program
	{
		static void Main()
		{
			//new DataPreparer().Prepare();
			new Program().Run();
		}

		IList<Line> EvaluateWholeEvening(Network ann, Track track, INormalizer velocityNormalizer, Line[] morning)
		{
			const int hvc = AnnInput.HistoricalVelocitiesCount;
			var lastLinesOfMorning =
				morning.Where(x => x.Time <= TimeConstants.T18).Reverse().Take(hvc+1).Reverse().ToArray();
			var input = new AnnInput
				{
					HistoricalVelocities = lastLinesOfMorning.Take(hvc).Select(x => x.Velocity.Value).ToArray(),
					Time = lastLinesOfMorning.Last().Time
				};
			while(!Close10(input.Time, TimeConstants.T18))
			{
				double next = ann.Compute(input)[0];
				input = input.Shift(next, input.Time.AddMinutes(TimeConstants.Interval));
			}
			
			var res = new List<Line>();
			for (int i = TimeConstants.EarlyEvening; i < TimeConstants.LastMeasurement; i++)
			{
				double next = ann.Compute(input)[0];
				res.Add(new Line {Day = track.Day, Street = track.Street, Time = TimeConstants.EveningTime(i), Velocity = velocityNormalizer.Denormalize(next) });

				input = input.Shift(next, TimeConstants.EveningTime(i + 1));
			}

			return res;
		}

		IList<Line> EvaluateWholeEveningTime(Network ann, Track track, INormalizer velocityNormalizer, Line[] morning)
		{
			const int hvc = AnnInput.HistoricalVelocitiesCount;
			var lastLinesOfMorning =
				morning.Where(x => x.Time <= TimeConstants.T18).Reverse().Take(hvc + 1).Reverse().ToArray();
			var input = new TimeAnnInput
			{
				Time = lastLinesOfMorning.Last().Time
			};
			while (!Close10(input.Time, TimeConstants.T18))
			{
				double next = ann.Compute(input)[0];
				input = input.Shift(next, input.Time.AddMinutes(TimeConstants.Interval));
			}

			var res = new List<Line>();
			for (int i = TimeConstants.EarlyEvening; i < TimeConstants.LastMeasurement; i++)
			{
				double next = ann.Compute(input)[0];
				res.Add(new Line { Day = track.Day, Street = track.Street, Time = TimeConstants.EveningTime(i), Velocity = velocityNormalizer.Denormalize(next) });

				input = input.Shift(next, TimeConstants.EveningTime(i + 1));
			}

			return res;
		}

		void Run()
		{
			var tracks = Track.ReadTracks(Paths.Jams_wo36_txt);
			var tracksTarget = Track.ReadTracks(Paths.Task_36_txt);

			//var mean = tracks.SelectMany(x => x.Lines).Select(x => x.Velocity.Value).Average();
			//var velocityNormalizer = new BipolarNormalizer(60, 15);
			var velocityNormalizer = new LinearNormalizer(new Statistics{MaxValue = 150});

			Track.NormalizeTracks(tracks, velocityNormalizer);
            var trackLookup = tracks.ToLookup(x => x.Street);

			var res = new List<Line>();
			foreach (var track in tracksTarget)
			{
				var annFilePath = "Anns\\" + track.Street + ".txt";
				var trainingSet = trackLookup[track.Street].ToArray();
				
				/*var ann = BuildApproximator(trainingSet);
                ann.Save(annFilePath);*/

				if (File.Exists(annFilePath))
				{
					var ann = Network.Load(annFilePath);
					var wholeEvening = EvaluateWholeEvening(ann, track, velocityNormalizer, trainingSet.First(x => x.Day == 41).Lines);

					foreach (var line in track.Lines)
					{
						var best = line.SelectClosest(wholeEvening);
						res.Add(new Line {Day = line.Day, Street = line.Street, Time = line.Time, Velocity = best.Velocity});
					}
				}
				else
				{
					foreach (var line in track.Lines)
					{
						res.Add(new Line { Day = line.Day, Street = line.Street, Time = line.Time, Velocity = 0 });
					}
				}
			}

			Line.OutputLines(Paths.Answer_Txt, res);
		}

		Network BuildApproximator(Track[] tracks)
		{
			const int Epochs = 10000;
			const int ProgressReportFrequency = Epochs / 20;
			const double TrainingSetRatio = 0.4;

			var ann = new ActivationNetwork(new SigmoidFunction(), AnnInput.InputCount, 7, 1);
			var teacher = new BackPropagationLearning(ann) { Momentum = 0.4 };

			var trainingSet = BuildTrainingSet(tracks.Choice(TrainingSetRatio));

			var inputList = trainingSet.Select(x => (double[])x.Input).ToArray();
			var outputList = trainingSet.Select(x => new[] {x.Output}).ToArray();

			Console.WriteLine("Training...");
			for (int i = 0; i < Epochs; i++)
			{
				teacher.LearningRate = Math.Pow(i, -1/3.0);
				var avErr = teacher.RunEpoch(inputList, outputList);// / inputList.Length;
				if (i % ProgressReportFrequency == 0)
					Console.WriteLine(avErr);
				if (avErr < 1)
					break;
			}
			return ann;
		}

		IList<TrainingPrecedent<TimeAnnInput>> BuildTrainingSetTime(IEnumerable<Track> tracks)
		{
			var res = new List<TrainingPrecedent<TimeAnnInput>>();
			foreach (var track in tracks)
			{
				if (track.Lines.Length < AnnInput.HistoricalVelocitiesCount + 1)
					continue;

				var currentInput = new TimeAnnInput
				{
					Time = track.Lines[AnnInput.HistoricalVelocitiesCount].Time
				};
				var newLines = track.Lines.Skip(AnnInput.HistoricalVelocitiesCount).ToArray();

				for (int i = 0; i < newLines.Length - 1; i++)
				{
					var l = newLines[i];
					var ln = newLines[i + 1];

					if (ln.Time >= TimeConstants.T18 || !Close10(l.Time, ln.Time))
						break;

					var precedent = new TrainingPrecedent<TimeAnnInput> { Output = l.Velocity.Value, Input = currentInput };
					res.Add(precedent);

					currentInput = currentInput.Shift(l.Velocity.Value, ln.Time);
				}
			}
			return res;
		}

		IList<TrainingPrecedent<AnnInput>> BuildTrainingSet(IEnumerable<Track> tracks)
		{
			var res = new List<TrainingPrecedent<AnnInput>>();
			foreach(var track in tracks)
			{
				if (track.Lines.Length < AnnInput.HistoricalVelocitiesCount + 1)
					continue;

				var currentInput = new AnnInput
					{
						HistoricalVelocities = track.Lines.Take(AnnInput.HistoricalVelocitiesCount).Select(x => x.Velocity.Value).ToArray(),
						Time = track.Lines[AnnInput.HistoricalVelocitiesCount].Time
					};
				var newLines = track.Lines.Skip(AnnInput.HistoricalVelocitiesCount).ToArray();

				for (int i = 0; i < newLines.Length-1; i++ )
				{
					var l = newLines[i];
					var ln = newLines[i+1];

					if (ln.Time >= TimeConstants.T18 || !Close10(l.Time, ln.Time))
						break;

					var precedent = new TrainingPrecedent<AnnInput> { Output = l.Velocity.Value, Input = currentInput };
					res.Add(precedent);

					currentInput = currentInput.Shift(l.Velocity.Value, ln.Time);
				}
			}
			return res;
		}


		bool Close10(DateTime a, DateTime b)
		{
			return Math.Abs((b - a).TotalMinutes) <= 10;
		}
	}
}