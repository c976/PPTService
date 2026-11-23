using PPTServiceUltra.Ink.Core.Windows.Native;

namespace PPTServiceUltra.Ink.Core.Windows.Input
{
	public static class WindowsCursorsManager
	{
		public static void ShowCursor()
		{
			while (ShowCursor(true) < 0)
			{
			}
		}

		public static void HideCursor()
		{
			while (ShowCursor(false) >= 0)
			{
			}
		}

		private static int ShowCursor(bool bShow)
		{
			return Win32.User32.ShowCursor(bShow);
		}
	}
}
