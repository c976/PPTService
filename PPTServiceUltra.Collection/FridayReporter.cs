using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Globalization;
using System.IO;

namespace PPTServiceUltra.Collection
{
	/// <summary>
	/// 埋点收集门面：保留原有的事件收集语义（立即 / 延迟缓存），但不再向服务器发送任何数据。
	/// 收集到的事件写入 %APPDATA%\PPTServiceUltra\Logs\Events\yyyy-MM-dd.log，
	/// 并按 7 天 / 总量 30MB 自动清理（从最早的开始），不会长期占用磁盘。
	/// </summary>
	public static class FridayReporter
	{
		private const int KeepDays = 7;
		private const long MaxTotalBytes = 30L * 1024 * 1024;
		private const string DateFormat = "yyyy-MM-dd";

		public static bool IsInitialize;

		public static readonly object InitializeLocker = new object();

		private static readonly ConcurrentDictionary<string, string> EventCache = new ConcurrentDictionary<string, string>();

		private static readonly object WriteLocker = new object();

		private static DateTime _lastClean = DateTime.MinValue;

		private static string EventsDirectory
		{
			get
			{
				return Path.Combine(
					Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData),
					"PPTServiceUltra", "Logs", "Events");
			}
		}

		public static bool IsReportEnabled
		{
			get { return IsInitialize; }
		}

		public static void Initialize(string appName, string appVersion)
		{
			lock (InitializeLocker)
			{
				if (IsInitialize)
				{
					return;
				}
				IsInitialize = true;
				// 不再构造 EventReporter（它会产生网络上报）。保留调用以维持初始化语义。
				foreach (KeyValuePair<string, string> item in EventCache)
				{
					WriteEvent(item.Key, item.Value);
				}
				EventCache.Clear();
			}
		}

		public static void ReportEventImmediately(string eventId)
		{
			WriteEvent(eventId, null);
		}

		public static void ReportEventImmediately(string eventId, string eventValue)
		{
			WriteEvent(eventId, eventValue);
		}

		public static void ReportEventDelayed(string eventId)
		{
			ReportEventImmediately(eventId);
		}

		public static void ReportEventDelayed(string eventId, string eventValue)
		{
			ReportEventImmediately(eventId, eventValue);
		}

		private static void WriteEvent(string eventId, string eventValue)
		{
			if (string.IsNullOrWhiteSpace(eventId))
			{
				return;
			}
			try
			{
				lock (WriteLocker)
				{
					string directory = EventsDirectory;
					if (!Directory.Exists(directory))
					{
						Directory.CreateDirectory(directory);
					}
					DateTime now = DateTime.Now;
					string line = "[" + now.ToString("yyyy-MM-dd HH:mm:ss") + "] " + eventId.Trim();
					if (!string.IsNullOrEmpty(eventValue))
					{
						line += " : " + eventValue;
					}
					File.AppendAllText(
						Path.Combine(directory, now.ToString(DateFormat, CultureInfo.InvariantCulture) + ".log"),
						line + Environment.NewLine);
					Clean(directory, now);
				}
			}
			catch
			{
				// 收集失败不影响主流程。
			}
		}

		/// <summary>清理过期的事件日志：保留 7 天，或总量超 30MB 时从最早开始删；当天文件始终保留。</summary>
		private static void Clean(string directory, DateTime now)
		{
			try
			{
				if (_lastClean != DateTime.MinValue && (now - _lastClean).TotalMinutes < 1.0)
				{
					return;
				}
				_lastClean = now;

				List<FileInfo> files = new List<FileInfo>();
				DateTime oldest = now.Date.AddDays(-(KeepDays - 1));
				long total = 0L;
				foreach (string path in Directory.GetFiles(directory, "????-??-??.log", SearchOption.TopDirectoryOnly))
				{
					DateTime date;
					if (!DateTime.TryParseExact(Path.GetFileNameWithoutExtension(path), DateFormat,
						CultureInfo.InvariantCulture, DateTimeStyles.None, out date))
					{
						continue;
					}
					FileInfo info = new FileInfo(path);
					files.Add(info);
					total += info.Length;
				}
				files.Sort(delegate(FileInfo a, FileInfo b) { return a.CreationTime.CompareTo(b.CreationTime); });

				foreach (FileInfo info in files)
				{
					DateTime date;
					if (!DateTime.TryParseExact(Path.GetFileNameWithoutExtension(info.FullName), DateFormat,
						CultureInfo.InvariantCulture, DateTimeStyles.None, out date) || date >= now.Date)
					{
						continue; // 当天文件始终保留
					}
					if (date >= oldest && total <= MaxTotalBytes)
					{
						break;
					}
					try
					{
						long length = info.Length;
						info.Delete();
						total -= length;
					}
					catch
					{
						// 单个文件删除失败不影响其它文件。
					}
				}
			}
			catch
			{
				// 清理失败不影响事件收集。
			}
		}
	}
}
