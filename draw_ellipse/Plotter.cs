using System;

namespace Ellipse
{
	class Plotter
	{
		readonly int XRadius;
		readonly int YRadius;
		readonly int CX;
		readonly int CY;

		public Plotter(int a, int b, int dx, int dy)
		{
			XRadius = a;
			YRadius = b;
			CX = dx;
			CY = dy;
		}

		public void DrawEllipse(Action<int, int> rawDrawPixel)
		{
			Action<int, int> plot4EllipsePoints = (x, y) =>
			{
				rawDrawPixel(CX + x, CY + y);
				rawDrawPixel(CX - x, CY + y);
				rawDrawPixel(CX - x, CY - y);
				rawDrawPixel(CX + x, CY - y);
			};



			int TwoASquare = 2 * XRadius * XRadius;
			int TwoBSquare = 2 * YRadius * YRadius;

			int X = XRadius;
			int Y = 0;
			int XChange = YRadius * YRadius * (1 - 2 * XRadius);
			int YChange = XRadius * XRadius;
			int EllipseError = 0;
			int StoppingX = TwoBSquare * XRadius;
			int StoppingY = 0;

			int maxX = 0;
			int minX = XRadius;
			int maxY = 0;
			int minY = YRadius;

			while (StoppingX >= StoppingY)
			{
				plot4EllipsePoints(X, Y);
				minX = Math.Min(X, minX);
				maxY = Math.Max(Y, maxY);
				Y++;
				StoppingY += TwoASquare;
				EllipseError += YChange;
				YChange += TwoASquare;
				if (2 * EllipseError + XChange > 0)
				{
					X--;
					StoppingX -= TwoBSquare;
					EllipseError += XChange;
					XChange += TwoBSquare;
				}
			}

			X = 0;
			Y = YRadius;
			XChange = YRadius * YRadius;
			YChange = XRadius * XRadius * (1 - 2 * YRadius);
			EllipseError = 0;
			StoppingX = 0;
			StoppingY = TwoASquare * YRadius;
			while (StoppingX <= StoppingY)
			{
				plot4EllipsePoints(X, Y);
				maxX = Math.Max(X, maxX);
				minY = Math.Min(Y, minY);
				X++;
				StoppingX += TwoBSquare;
				EllipseError += XChange;
				XChange += TwoBSquare;
				if (2 * EllipseError + YChange > 0)
				{
					Y--;
					StoppingY -= TwoASquare;
					EllipseError += YChange;
					YChange += TwoASquare;
				}
			}

			for (X = maxX + 1; X < minX; X++)
				plot4EllipsePoints(X, minY);
			for (Y = maxY + 1; Y < minY; Y++)
				plot4EllipsePoints(minX, Y);
		}
	}
}