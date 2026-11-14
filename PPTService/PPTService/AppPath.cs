using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;

namespace PPTService
{
	public class AppPath
	{
		/// <summary>本程序所有运行时数据（配置 / 日志 / 临时缩略图 / 二维码 / 事件缓存）的统一根目录名，
		/// 位于 %LOCALAPPDATA%（Local）下，不再散落在 Roaming\Seewo\PPTService 或安装目录。</summary>
		public static readonly string AppFolderName = "PPTServiceUltra";

		private static readonly string Log = "Log";

		private static readonly string Temporary = "Temp";

		private static readonly string Document = "Document";

		/// <summary>%LOCALAPPDATA%\PPTServiceUltra，不存在时自动创建。</summary>
		public static string LocalRoot()
		{
			string text = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData), AppFolderName);
			if (!Directory.Exists(text))
			{
				Directory.CreateDirectory(text);
			}
			return text;
		}

		public static string DocumentPath()
		{
			string text = Path.Combine(LocalRoot(), Document);
			if (!Directory.Exists(text))
			{
				Directory.CreateDirectory(text);
			}
			return text;
		}

		public static string LogPath()
		{
			string text = Path.Combine(LocalRoot(), Log);
			if (!Directory.Exists(text))
			{
				Directory.CreateDirectory(text);
			}
			return text;
		}

		public static string TemPath()
		{
			string text = Path.Combine(LocalRoot(), Temporary);
			if (!Directory.Exists(text))
			{
				Directory.CreateDirectory(text);
			}
			return text;
		}

		/// <summary>保留原有方法名（ConfigHelper 的 app.config 用它），实现改为统一根目录。</summary>
		public static string RootPath()
		{
			return LocalRoot();
		}

		public static void ClearTempFolder()
		{
			try
			{
				Task.Factory.StartNew(() =>
				{
					ClearDirectory(TemPath());
				});
			}
			catch
			{
			}
		}

		private static void ClearDirectory(string directory)
		{
			try
			{
				Stack<string> stack = new Stack<string>();
				stack.Push(directory);
				while (stack.Any())
				{
					string path = stack.Pop();
					string[] files = Directory.GetFiles(path);
					foreach (string path2 in files)
					{
						File.SetAttributes(path2, FileAttributes.Normal);
						File.Delete(path2);
					}
					files = Directory.GetDirectories(path);
					foreach (string item in files)
					{
						stack.Push(item);
					}
				}
			}
			catch
			{
			}
			try
			{
				Directory.Delete(directory, true);
				Directory.CreateDirectory(directory);
			}
			catch
			{
			}
		}
	}
}
