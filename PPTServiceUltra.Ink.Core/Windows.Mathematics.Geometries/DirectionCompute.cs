using System.Windows;

namespace PPTServiceUltra.Ink.Core.Windows.Mathematics.Geometries
{
	public static class DirectionCompute
	{
		public static Direction JudgeDirection(this Point point1, Point point2)
		{
			if (Nearly.Equals(point1.X, point2.X) && point2.Y < point1.Y)
			{
				return Direction.Up;
			}
			if (Nearly.Equals(point1.X, point2.X) && point2.Y > point1.Y)
			{
				return Direction.Down;
			}
			if (point2.X > point1.X && Nearly.Equals(point1.Y, point2.Y))
			{
				return Direction.Left;
			}
			if (point2.X < point1.X && Nearly.Equals(point1.Y, point2.Y))
			{
				return Direction.Right;
			}
			if (point2.X > point1.X && point2.Y < point1.Y)
			{
				return Direction.LeftUp;
			}
			if (point2.X > point1.X && point2.Y > point1.Y)
			{
				return Direction.LeftDown;
			}
			if (point2.X < point1.X && point2.Y < point1.Y)
			{
				return Direction.RightUp;
			}
			if (point2.X < point1.X && point2.Y > point1.Y)
			{
				return Direction.RightDown;
			}
			return Direction.Nodirection;
		}
	}
}
