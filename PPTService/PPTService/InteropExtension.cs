using System.Drawing;
using System.Windows;

namespace PPTService
{
	internal static class InteropExtension
	{
		public static System.Drawing.Point ToWinFormPoint(this System.Windows.Point point)
		{
			return new System.Drawing.Point((int)(point.X + 0.5), (int)(point.Y + 0.5));
		}

		public static Rect ToRect(this Rectangle rectangle)
		{
			return new Rect(rectangle.X, rectangle.Y, rectangle.Width, rectangle.Height);
		}

		public static Rect ToRect(this Rectangle rectangle, double dpi)
		{
			double num = 96.0 / dpi;
			return new Rect((double)rectangle.X * num, (double)rectangle.Y * num, (double)rectangle.Width * num, (double)rectangle.Height * num);
		}
	}
}
