using System;

namespace Okulobot
{
    public class MoveInfo
    {
    	public Point Destination { get; set;}

    	public double Angle { get; set; }

    	public Point? FireTarget { get; set; }

    	public string Message { get; set; }

    	public MoveInfo(Point destination, double angle)
    	{
    		Destination = destination;
    		Angle = AngleUtils.Normalize(angle);
    		Message = "";
    	}

		public override string ToString()
		{
			return string.Format("[X: {0}, Y: {1}, A: {2}]", Destination.X, Destination.Y, Angle.ToDegrees());
		}
    }
}