using System.Diagnostics;
using System.IO;
using Microsoft.Win32;

namespace PPTServiceUltra
{
	public class Restart
	{
		private static string AppRestart = "Restart";

		public static void PPTServiceUltra()
		{
			string pPTServicePath = GetPPTServiceUltraPath();
			if (!string.IsNullOrEmpty(pPTServicePath) && File.Exists(pPTServicePath))
			{
				Process.Start(pPTServicePath, AppRestart);
			}
		}

		private static string GetPPTServiceUltraPath()
		{
			string text = null;
			RegistryKey registryKey = Registry.LocalMachine.OpenSubKey("SOFTWARE\\PPTServiceUltra", false);
			if (registryKey != null)
			{
				text = registryKey.GetValue("ExePath").ToString();
			}
			if (string.IsNullOrEmpty(text))
			{
				RegistryKey registryKey2 = Registry.LocalMachine.OpenSubKey("SOFTWARE\\Wow6432Node\\PPTServiceUltra", false);
				if (registryKey2 != null)
				{
					text = registryKey2.GetValue("ExePath").ToString();
				}
			}
			return text;
		}
	}
}
