using System;
using System.IO;
using System.Text;

namespace PPTService
{
	public static class Log
	{
		public static readonly string ErrorFilePath = Path.Combine(AppPath.LogPath(), "error.txt");

		public static readonly string DebugFilePath = Path.Combine(AppPath.LogPath(), "debug.txt");

		private static readonly object Locker = new object();

		public static void WriteError(string error)
		{
			Write(ErrorFilePath, error);
		}

		public static void WriteDebug(string info)
		{
		}

		private static void Write(string filePath, string str)
		{
			lock (Locker)
			{
				StringBuilder stringBuilder = new StringBuilder();
				stringBuilder.AppendLine($"{DateTime.Now:G}  {str}");
				File.AppendAllText(filePath, stringBuilder.ToString());
			}
		}
	}
}
