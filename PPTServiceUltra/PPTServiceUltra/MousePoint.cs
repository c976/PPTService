using System.Windows;

namespace PPTServiceUltra
{
	public struct MousePoint
	{
		public int X;

		public int Y;

		public MousePoint(double x, double y)
		{
			X = (int)x;
			Y = (int)y;
		}

		public static implicit operator Point(MousePoint mousePoint)
		{
			return new Point(mousePoint.X, mousePoint.Y);
		}
	}
}
