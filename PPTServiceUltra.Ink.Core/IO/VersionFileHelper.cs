using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.CompilerServices;

namespace PPTServiceUltra.Ink.Core.IO
{
	
	
	internal static class VersionFileHelper
	{
		public static void ControlFileVersion(string logFilePath, int maxRollBackupFileCount)
		{
			string fileName = Path.GetFileName(logFilePath);
			List<ValueTuple<int, string>> list = ParserLogFileNameList(Directory.GetFiles(Path.GetDirectoryName(logFilePath), fileName + ".*")).ToList();
			list.Sort( ( ValueTuple<int, string> versionFile1,  ValueTuple<int, string> versionFile2) => versionFile1.Item1.CompareTo(versionFile2.Item1) * -1);
			foreach (var item3 in list)
			{
				int item = item3.Item1;
				string item2 = item3.Item2;
				try
				{
					if (item >= maxRollBackupFileCount)
					{
						File.Delete(item2);
					}
					else
					{
						File.Move(item2, $"{logFilePath}.{item + 1}");
					}
				}
				catch (Exception)
				{
				}
			}
			File.Move(logFilePath, logFilePath + ".1");
		}

		
		
		private static IEnumerable<ValueTuple<int, string>> ParserLogFileNameList(string[] logFileList)
		{
			foreach (string text in logFileList)
			{
				int num = text.LastIndexOf(".", StringComparison.Ordinal);
				int result;
				if (num >= 0 && num != text.Length - 1 && int.TryParse(text.Substring(num + 1), out result))
				{
					yield return new ValueTuple<int, string>(result, text);
				}
			}
		}
	}
}
