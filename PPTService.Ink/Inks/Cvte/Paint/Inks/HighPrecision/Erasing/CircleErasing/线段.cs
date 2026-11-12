using System.Windows;

namespace Cvte.Paint.Inks.HighPrecision.Erasing.CircleErasing
{
	internal class 线段
	{
		public Point A { get; }

		public Point B { get; }

		public 线段(Point a, Point b)
		{
			A = a;
			B = b;
		}
	}
}
