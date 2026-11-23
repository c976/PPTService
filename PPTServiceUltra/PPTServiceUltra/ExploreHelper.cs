using System;
using System.Diagnostics;

namespace PPTServiceUltra
{
	public static class ExploreHelper
	{
		public static void OpenLink(string sUrl)
		{
			try
			{
				Process.Start(sUrl);
			}
			catch (Exception)
			{
				try
				{
					Process.Start(new ProcessStartInfo("IExplore.exe", sUrl));
				}
				catch (Exception)
				{
				}
			}
		}
	}
}
