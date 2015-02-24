using System;
using System.Drawing;

namespace Horizon
{
	public class Plotter
	{
		public double Scale { get; set; }
		public double Step { get; set; }
		public double FrontY { get; set; }
		public double BackY { get; set; }

		public Plotter()
		{
			FrontY = 4;
			BackY = -4;
			Scale = 50;
			Step = (FrontY - BackY)/200;
		}

		public void DrawSurface(Func<double, double, double> Z, int wx, int wy, Graphics g)
		{
			var sqrt2 = Math.Sqrt(2);
			loHorizon = new int[wx];
			hiHorizon = new int[wx];

			for(int i = 0; i < wx; i++)
			{
				hiHorizon[i] = -wy;
				loHorizon[i] = wy;
			}

			double scaledCenter = wx / (2 * Scale);
			int steps = (int)((FrontY - BackY) / Step);
			double y = FrontY;

			for (int i = 1; i <= steps; i++)
			{
				double yProection = y / sqrt2;
				lastIsHidden = true;
				for (int xx = 0; xx < wx; xx++)
				{
					double x = (xx / Scale) - scaledCenter + yProection;
					double z = Z(x, y);
					int yy = wx / 2 + (int)(Scale * y / sqrt2 - Scale * z);
					ConnectNewPoint(xx, yy, g);
				}
				y -= Step;
			}
		}

		private void ConnectNewPoint(int xx, int yy, Graphics g)
		{
			var curPoint = new Point(xx, yy);
			if (curPoint.X == 0)
			{
				prevPoint.X = xx;
				prevPoint.Y = yy;
			}

			if (hiHorizon[xx] > yy && yy > loHorizon[xx])
			{
				if (!lastIsHidden)
				{
					if (lastDrawnIsOnHhor)
					{
						curPoint.Y = hiHorizon[curPoint.X];
						//MyPen.Color = Color.Red;
						DrawLine(prevPoint, curPoint, g);

					}
					if (lastDrawnIsOnLhor)
					{
						curPoint.Y = loHorizon[curPoint.X];
						//MyPen.Color = Color.Black;
						DrawLine(prevPoint, curPoint, g);
					}
				}
				prevPoint = curPoint;
				lastDrawnIsOnHhor = false;
				lastDrawnIsOnLhor = false;
				lastIsHidden = true;
			}
			else
			{
				if (yy >= hiHorizon[xx])
				{
					lastDrawnIsOnHhor = true;
					lastDrawnIsOnLhor = false;
					//MyPen.Color = Color.Red;
					hiHorizon[xx] = yy;
				}
				if (yy <= loHorizon[xx])
				{
					lastDrawnIsOnHhor = false;
					lastDrawnIsOnLhor = true;
					//MyPen.Color = Color.Black;
					loHorizon[xx] = yy;
				}

				if (!lastIsHidden)
				{
					DrawLine(prevPoint, curPoint, g);
				}
				else
				{
					if (lastDrawnIsOnHhor)
					{
						prevPoint.Y = hiHorizon[prevPoint.X];
						//MyPen.Color = Color.Red;
						DrawLine(prevPoint, curPoint, g);

					}
					else
					{
						prevPoint.Y = loHorizon[prevPoint.X];
						//MyPen.Color = Color.Black;
						DrawLine(prevPoint, curPoint, g);
					}
				}

				lastIsHidden = false;
				prevPoint = curPoint;
			}
		}

		private void DrawLine(Point prevPoint, Point curPoint, Graphics g)
		{
			int minYy = Math.Min(curPoint.Y, prevPoint.Y);
			int maxYy = Math.Max(curPoint.Y, prevPoint.Y);
			int yyCenter = minYy + (maxYy - minYy) / 2;

			//PrevPoint = curPoint;
			g.DrawLine(Pens.Black, prevPoint.X, prevPoint.Y, curPoint.X, yyCenter);
			g.DrawLine(Pens.Black, curPoint.X, yyCenter, curPoint.X, curPoint.Y);


			if (curPoint.Y == loHorizon[curPoint.X])
			{
				if (prevPoint.Y >= curPoint.Y)
					loHorizon[prevPoint.X] = yyCenter;
				else
					loHorizon[curPoint.X] = yyCenter;
			}
			if (curPoint.Y == hiHorizon[curPoint.X])
			{
				if (prevPoint.Y >= curPoint.Y)
					hiHorizon[curPoint.X] = yyCenter;
				else
					hiHorizon[prevPoint.X] = yyCenter;
			}

		}

		int[] loHorizon;
		int[] hiHorizon;
		bool lastIsHidden;
		Point prevPoint;
		bool lastDrawnIsOnHhor;
		bool lastDrawnIsOnLhor;
	}
}