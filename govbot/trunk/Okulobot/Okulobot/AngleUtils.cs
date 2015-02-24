using System;

namespace Okulobot
{
	public static class AngleUtils
	{
		public static double ToRadians(this int x)
		{
			return x*Math.PI/180.0;
		}

		public static int ToDegrees(this double x)
		{
			return (int)(x*180/Math.PI)%360;
		}

		public static double Reflect(double angle)
		{
			return Normalize((Math.PI - Normalize(angle)));
		}

		public static double Normalize(double angle)
		{
			const double bipi = 2 * Math.PI;

			while (angle < 0)
				angle += bipi;

			while (angle > bipi)
				angle -= bipi;

			return angle;
		}

		public static double GetDirection(Point from, Point to)
		{
			return Normalize(Math.Atan2(to.Y - from.Y, to.X - from.X));
		}
	}
}