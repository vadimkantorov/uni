using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace Okulobot
{
	public class IOUtils
	{
		private readonly TextReader input;
		private readonly TextWriter output;

		public IOUtils(TextReader input, TextWriter output)
		{
			this.input = input;
			this.output = output;
		}

		public void PrintNewbornFighter(Fighter fighter)
		{
			output.WriteLine(fighter.Properties.TypeName);
		}

		private void PrintPoint(Point p, bool reflect)
		{
			if (reflect)
				p = Reflect(p);

			output.WriteLine(p.X);
			output.WriteLine(p.Y);
		}

		public Point Reflect(Point move)
		{
			return new Point
           	{
           		X = 199 - move.X,
				Y = move.Y
           	};
		}

		public void PrintNewMove(MoveInfo move, bool reflect)
		{
			if (move.FireTarget != null)
			{
				output.WriteLine(1);
				PrintPoint(move.FireTarget.Value, reflect);
			}
			else
			{
				output.WriteLine(0);
				PrintPoint(new Point {X = -1, Y = -1}, reflect);
			}

			PrintPoint(move.Destination, reflect);
            PrintAngle(move.Angle, reflect);
			output.WriteLine(move.Message);

			output.Flush();
		}

		private int ReadInt()
		{
			return Convert.ToInt32(ReadString());
		}

		private string ReadString()
		{
			return input.ReadLine().Trim();
		}

		private Point ReadPoint(bool reflect)
		{
			int a = ReadInt();
			int b = ReadInt();
			var res = new Point
			{
				X = a,
				Y = b
			};
			if (reflect)
				res = Reflect(res);
			return res;
		}

		private double ReadAngle(bool reflect)
		{
			double angle = ReadInt().ToRadians();
			if (reflect)
				angle = AngleUtils.Reflect(angle);
			return angle;
		}

		private void PrintAngle(double angle, bool reflect)
		{
			output.WriteLine((reflect ? AngleUtils.Reflect(angle) : angle).ToDegrees());
		}

		public InitData ScanInitData()
		{
			return new InitData
	       	{
	       		Team = ReadInt(),
	       		Number = ReadInt()
	       	};
		}

		private void EnsureInValues(int val, string parameterName, params int[] allowed)
		{
			if (Array.IndexOf(allowed, val) == -1)
			{
				var message = string.Format("Параметр '{0}' имеет значение не из [{1}]", 
					parameterName, 
					string.Join(", ", allowed.Select(x=>x.ToString()).ToArray())
				);
				throw new GameProtocolException(message);
			}
		}

		public Reality ScanReality(Fighter currentFighter, bool reflect)
		{
			var sync = ReadString();
			
			if (sync != "Begin")
				throw new GameProtocolException("Synchronization fail");

			var reality = new Reality();
			reality.ActiveFighter = currentFighter;

			int liveliness = ReadInt();
			EnsureInValues(liveliness, "живость", 0, 1);

			currentFighter.IsAlive = liveliness == 1;
			if (!currentFighter.IsAlive)
				return reality;


			try
			{
				currentFighter.Coordinates = ReadPoint(reflect);
				currentFighter.Angle = ReadAngle(reflect);
				currentFighter.Health = ReadInt();
				currentFighter.Mana = ReadInt();
				

				//System.Diagnostics.Debugger.Break();

				int unitCount = ReadInt();

				for (int i = 0; i < unitCount; i++)
				{
					var coords = ReadPoint(reflect);
					var team = ReadInt();
					EnsureInValues(team, "СВОЙ-ЧУЖОЙ..ZZZ!", 0,1);
					
					var f = new Unit(ReadString()[0]);
					f.Health = ReadInt();
					f.Coordinates = coords;

					if (team == 0)
						reality.Friends.Add(f);
					else
						reality.Enemies.Add(f);
				}

				int friendCount = ReadInt();
				for (int i = 0; i < friendCount; i++)
					reality.Messages.Add(ReadString());
			}
			catch (Exception e)
			{
				throw new GameProtocolException("Protocol missmatch in coordinates: " + e.Message);
			}

			return reality;
		}
	}
}