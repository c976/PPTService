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
			// 诊断用：WriteDebug 历史上是空实现。放映态显示问题（工具栏偶发不显示、
			// 右键才出现）排查需要这些状态轨迹，故改为真正落盘；调用点均做了节流，
			// 只在状态变化或关键事件时写，不会刷屏。
			Write(DebugFilePath, info);
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
