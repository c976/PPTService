using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;

namespace PPTService
{
	public class AppPath
	{
		private static readonly string Company = "Seewo";

		private static readonly string AppName = "PPTService";

		private static readonly string Log = "Log";

		private static readonly string Temporary = "Temp";

		private static readonly string Document = "Document";

		public static string DocumentPath()
		{
			string text = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData), Company, AppName, Document);
			if (!Directory.Exists(text))
			{
				Directory.CreateDirectory(text);
			}
			return text;
		}

		public static string LogPath()
		{
			string text = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData), Company, AppName, Log);
			if (!Directory.Exists(text))
			{
				Directory.CreateDirectory(text);
			}
			return text;
		}

		public static string TemPath()
		{
			string text = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData), Company, AppName, Temporary);
			if (!Directory.Exists(text))
			{
				Directory.CreateDirectory(text);
			}
			return text;
		}

		public static string RootPath()
		{
			string text = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData), Company, AppName);
			if (!Directory.Exists(text))
			{
				Directory.CreateDirectory(text);
			}
			return text;
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
