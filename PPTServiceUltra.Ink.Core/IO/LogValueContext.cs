using System;
using System.Diagnostics;
using System.Runtime.CompilerServices;
using System.Threading;

namespace PPTServiceUltra.Ink.Core.IO
{
	
	
	
	public struct LogValueContext
	{
		public LogLevel LogLevel { get; }

		public string LogMessage { get; }

		public Exception Exception { get; }

		public DateTime DateTime { get; }

		public int ThreadId { get; }

		public string ThreadName { get; }

		[DebuggerStepThrough]
		public LogValueContext(LogLevel logLevel, string logMessage, Exception exception = null)
		{
			LogLevel = logLevel;
			LogMessage = logMessage;
			Exception = exception;
			DateTime = DateTime.Now;
			ThreadId = Thread.CurrentThread.ManagedThreadId;
			ThreadName = Thread.CurrentThread.Name;
		}

		
		public override string ToString()
		{
			return string.Format("[{0:yyyy-MM-dd HH:mm:ss,fff}][{1}][{2}:{3}] {4}{5}", DateTime, DefaultLogLevelConverter.LogLevelToText(LogLevel), ThreadName, ThreadId, LogMessage, (Exception != null) ? (" " + Exception) : string.Empty);
		}
	}
}
