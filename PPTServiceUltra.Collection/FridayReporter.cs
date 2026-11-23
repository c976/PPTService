using System;
using System.Collections.Generic;
using System.IO;
using System.Text;

namespace PPTServiceUltra.Collection
{
	/// <summary>
	/// 埋点收集门面：保留原有的事件收集语义（立即 / 延迟缓存），但事件只缓存在内存中，
	/// 不会自动写盘，也不会联网。只有用户在「设置 → 生成事件收集日志」里手动导出时，
	/// 才把当前缓存写入其指定的文件。
	/// </summary>
	public static class FridayReporter
	{
		// 环形缓存上限：避免长期运行占用内存；超出后丢弃最早的事件。
		private const int MaxBufferedEvents = 2000;

		public static bool IsInitialize;

		public static readonly object InitializeLocker = new object();

		private static readonly object BufferLocker = new object();

		private static readonly Queue<string> EventBuffer = new Queue<string>();

		public static bool IsReportEnabled
		{
			get { return IsInitialize; }
		}

		public static void Initialize(string appName, string appVersion)
		{
			lock (InitializeLocker)
			{
				// 不再构造 EventReporter（它会产生网络上报），也不再写事件文件。
				// 保留调用以维持初始化语义；后续事件直接进内存缓存。
				IsInitialize = true;
			}
		}

		public static void ReportEventImmediately(string eventId)
		{
			BufferEvent(eventId, null);
		}

		public static void ReportEventImmediately(string eventId, string eventValue)
		{
			BufferEvent(eventId, eventValue);
		}

		public static void ReportEventDelayed(string eventId)
		{
			BufferEvent(eventId, null);
		}

		public static void ReportEventDelayed(string eventId, string eventValue)
		{
			BufferEvent(eventId, eventValue);
		}

		private static void BufferEvent(string eventId, string eventValue)
		{
			if (string.IsNullOrWhiteSpace(eventId))
			{
				return;
			}
			try
			{
				string line = "[" + DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss") + "] " + eventId.Trim();
				if (!string.IsNullOrEmpty(eventValue))
				{
					line += " : " + eventValue;
				}
				lock (BufferLocker)
				{
					EventBuffer.Enqueue(line);
					while (EventBuffer.Count > MaxBufferedEvents)
					{
						EventBuffer.Dequeue();
					}
				}
			}
			catch
			{
				// 收集失败不影响主流程。
			}
		}

		/// <summary>
		/// 手动导出：由设置窗口的按钮触发，把当前内存缓存一次性写入用户指定的文件。
		/// 不传路径时不写任何文件，也不会在后台自动创建目录。
		/// </summary>
		public static bool TryExportEvents(string filePath, out int eventCount)
		{
			eventCount = 0;
			if (string.IsNullOrWhiteSpace(filePath))
			{
				return false;
			}
			string[] lines;
			lock (BufferLocker)
			{
				lines = EventBuffer.ToArray();
			}
			try
			{
				string directory = Path.GetDirectoryName(filePath);
				if (!string.IsNullOrEmpty(directory) && !Directory.Exists(directory))
				{
					Directory.CreateDirectory(directory);
				}
				using (StreamWriter writer = new StreamWriter(filePath, false, new UTF8Encoding(true)))
				{
					writer.WriteLine("# PPTServiceUltra 事件收集日志");
					writer.WriteLine("# 生成时间: " + DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss"));
					writer.WriteLine("# 内容: 事件名称、发生时间和可选结果值；不含 PPT 内容、文件路径或设备标识，不联网发送。");
					writer.WriteLine("# 本次导出事件数: " + lines.Length + "（仅保留最近 " + MaxBufferedEvents + " 条）");
					writer.WriteLine();
					foreach (string line in lines)
					{
						writer.WriteLine(line);
					}
				}
				eventCount = lines.Length;
				return true;
			}
			catch
			{
				return false;
			}
		}
	}
}
