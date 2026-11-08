using System.Windows;

namespace PPTService
{
	public static class ScreenExtensions
	{
		public static Point ToWpfPoint(this Point screenPoint)
		{
			return new Point(screenPoint.X / (double)ScreenHelper.DpiX * 96.0, screenPoint.Y / (double)ScreenHelper.DpiY * 96.0);
		}

		public static Point ToScreenPoint(this Point wpfPoint)
		{
			return new Point(wpfPoint.X * (double)ScreenHelper.DpiX / 96.0, wpfPoint.Y * (double)ScreenHelper.DpiY / 96.0);
		}
	}
}
