using System;
using System.IO;
using System.Runtime.CompilerServices;
using System.Text;
using PPTServiceUltra.Shared;

namespace PPTServiceUltra
{
	public static class Log
	{
		/// <summary>主程序日志：%APPDATA%\PPTServiceUltra\Log\yyyy-MM-dd.log（仅清理本目录内的日期日志）。</summary>
		public static string ErrorFilePath
		{
			get { return LogCleaner.FilePath(AppPath.LogPath()); }
		}

		public static string DebugFilePath
		{
			get { return LogCleaner.FilePath(AppPath.LogPath()); }
		}

		private static readonly object Locker = new object();

		// 调用方信息由 C# 编译器在调用点内联填入，无需 PDB / 无需改动任何调用处。
		// sourceFile 只取文件名（不含完整路径），避免日志里出现编译机绝对路径。
		public static void WriteError(string error,
			[CallerFilePath] string sourceFile = "",
			[CallerLineNumber] int sourceLine = 0)
		{
			string location = string.IsNullOrEmpty(sourceFile)
				? ""
				: $" [{Path.GetFileName(sourceFile)}:{sourceLine}]";
			Write(ErrorFilePath, location + "  " + error);
		}

		public static void WriteDebug(string info)
		{
			// 保持空实现（原版行为）：调试轨迹仅排查问题时临时开启，平时不落盘。
		}

		private static void Write(string filePath, string str)
		{
			lock (Locker)
			{
				StringBuilder stringBuilder = new StringBuilder();
				stringBuilder.AppendLine($"{DateTime.Now:G}  {str}");
				File.AppendAllText(filePath, stringBuilder.ToString());
				LogCleaner.Clean(Path.GetDirectoryName(filePath));
			}
		}
	}
}
