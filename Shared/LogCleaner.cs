using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;

namespace PPTService.Shared
{
	/// <summary>
	/// 按日期命名日志（yyyy-MM-dd.log）的自动清理：仅处理传入目录内、文件名是合法日期的 .log，
	/// 不递归子目录、不影响 %APPDATA%\Seewo 下的第三方日志（如 smartpen）。
	/// 规则：文件名日期早于保留天数（7 天）即删除；或该目录内此类日志总量超过 30MB 时，
	/// 从最早的开始删到阈值以内。当天文件永远保留，避免删掉正在写入的日志。
	/// </summary>
	public static class LogCleaner
	{
		/// <summary>保留最近 7 个自然日（含当天）。</summary>
		public const int KeepDays = 7;

		/// <summary>单个程序（主程序 / 守护程序各自计算）日期日志的总量上限：30MB。</summary>
		public const long MaxTotalBytes = 30L * 1024 * 1024;

		private const string DateFormat = "yyyy-MM-dd";

		private static readonly object Locker = new object();

		private static DateTime _lastClean = DateTime.MinValue;

		/// <summary>取当前日期对应的日志全路径。</summary>
		public static string FilePath(string directory)
		{
			return Path.Combine(directory, DateTime.Now.ToString(DateFormat, CultureInfo.InvariantCulture) + ".log");
		}

		/// <summary>
		/// 清理目录内符合 yyyy-MM-dd.log 的旧日志。每次写入时调用，但内部按分钟节流，
		/// 避免高频写日志时反复扫描目录。
		/// </summary>
		public static void Clean(string directory)
		{
			if (string.IsNullOrEmpty(directory))
			{
				return;
			}
			lock (Locker)
			{
				try
				{
					DateTime now = DateTime.Now;
					if (_lastClean != DateTime.MinValue && (now - _lastClean).TotalMinutes < 1.0)
					{
						return;
					}
					_lastClean = now;

					if (!Directory.Exists(directory))
					{
						return;
					}
					DateTime oldest = now.Date.AddDays(-(KeepDays - 1));
					List<Entry> entries = new List<Entry>();
					foreach (string path in Directory.GetFiles(directory, "????-??-??.log", SearchOption.TopDirectoryOnly))
					{
						DateTime date;
						if (!DateTime.TryParseExact(Path.GetFileNameWithoutExtension(path), DateFormat,
							CultureInfo.InvariantCulture, DateTimeStyles.None, out date))
						{
							continue;
						}
						entries.Add(new Entry(path, date, SafeLength(path)));
					}
					entries.Sort(delegate(Entry a, Entry b) { return a.Date.CompareTo(b.Date); });

					long total = 0L;
					foreach (Entry entry in entries)
					{
						total += entry.Length;
					}

					foreach (Entry entry in entries)
					{
						// 当天日志正在写入，始终保留；其余按「过期」或「超量」从最早开始删。
						if (entry.Date >= now.Date)
						{
							continue;
						}
						if (entry.Date >= oldest && total <= MaxTotalBytes)
						{
							break;
						}
						if (TryDelete(entry.Path))
						{
							total -= entry.Length;
						}
					}
				}
				catch
				{
					// 清理失败不影响日志写入与主流程。
				}
			}
		}

		private static long SafeLength(string path)
		{
			try
			{
				return new FileInfo(path).Length;
			}
			catch
			{
				return 0L;
			}
		}

		private static bool TryDelete(string path)
		{
			try
			{
				File.Delete(path);
				return true;
			}
			catch
			{
				return false;
			}
		}

		private sealed class Entry
		{
			public readonly string Path;
			public readonly DateTime Date;
			public readonly long Length;

			public Entry(string path, DateTime date, long length)
			{
				Path = path;
				Date = date;
				Length = length;
			}
		}
	}
}
