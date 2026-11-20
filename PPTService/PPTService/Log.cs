using System;
using System.IO;
using System.Runtime.CompilerServices;
using System.Text;

namespace PPTService
{
	public static class Log
	{
		public static readonly string ErrorFilePath = Path.Combine(AppPath.LogPath(), "error.txt");

		public static readonly string DebugFilePath = Path.Combine(AppPath.LogPath(), "debug.txt");

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
			}
		}
	}
}
