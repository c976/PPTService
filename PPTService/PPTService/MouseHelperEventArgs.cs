using System;
using System.Windows;

namespace PPTService
{
	public class MouseHelperEventArgs : EventArgs
	{
		public Point MousePoint { get; set; }

		public MouseHelperEventArgs(double x, double y)
		{
			x /= ScreenHelper.DpiFactor;
			y /= ScreenHelper.DpiFactor;
			MousePoint = new Point(x, y);
		}
	}
}
