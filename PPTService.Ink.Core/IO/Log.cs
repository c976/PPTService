using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
using System.Threading.Tasks;
using dotnetCampus.Threading;

namespace PPTService.Ink.Core.IO
{
	
	
	public static class Log
	{
		
		private class LoggerDisposer : ILoggerDisposer
		{
			private bool _isNotifyDisposed;

			
			public async Task NotifyAndWaitLoggerDisposedAsync()
			{
				if (_isNotifyDisposed)
				{
					throw new ObjectDisposedException("不能多次调用 NotifyAndWaitLoggerDisposedAsync 方法");
				}
				_isNotifyDisposed = true;
				DoubleBufferLazyInitializeTask<LogValueContext> doubleBufferTask = DoubleBufferTask;
				doubleBufferTask.Finish();
				await doubleBufferTask.WaitAllTaskFinish().ConfigureAwait(false);
				DoubleBufferTask = new DoubleBufferLazyInitializeTask<LogValueContext>(OnWriteLog);
				_loggerDisposer = null;
				_isConfigured = false;
				LoggerFileWriterList = null;
			}
		}

		private static string _logFolder = "";

		[Obsolete("此属性毫无作用，请默默删除")]
		[EditorBrowsable(EditorBrowsableState.Never)]
		public static readonly LogContextProperties CustomProperties = new LogContextProperties();

		private static bool _isConfigured;

		private static LoggerDisposer _loggerDisposer = null;

		[Obsolete("此属性毫无作用，请默默删除")]
		[EditorBrowsable(EditorBrowsableState.Never)]
		public static string Session
		{
			get
			{
				return string.Empty;
			}
			set
			{
				throw new InvalidOperationException("此属性毫无作用");
			}
		}

		[Obsolete("此属性毫无作用，请默默删除")]
		[EditorBrowsable(EditorBrowsableState.Never)]
		public static string LogFolder
		{
			get
			{
				return _logFolder;
			}
			set
			{
			}
		}

		[Obsolete("此属性毫无作用，请默默删除")]
		[EditorBrowsable(EditorBrowsableState.Never)]
		public static string AppVersion
		{
			get
			{
				return string.Empty;
			}
			set
			{
			}
		}

		[Obsolete("此属性毫无作用，请默默删除")]
		[EditorBrowsable(EditorBrowsableState.Never)]
		public static bool IsDebug => false;

		[Obsolete("此属性毫无作用，请默默删除")]
		[EditorBrowsable(EditorBrowsableState.Never)]
		public static bool UseEasiLogger
		{
			get
			{
				return false;
			}
			set
			{
			}
		}

		[Obsolete("此属性毫无作用，请默默删除")]
		[EditorBrowsable(EditorBrowsableState.Never)]
		public static string EasiLoggerName
		{
			get
			{
				return string.Empty;
			}
			set
			{
			}
		}

		[Obsolete("此属性毫无作用，请默默删除")]
		[EditorBrowsable(EditorBrowsableState.Never)]
		public static string EasiDebugLoggerName
		{
			get
			{
				return string.Empty;
			}
			set
			{
			}
		}

		public static bool IsEnabled { get; set; } = true;

		private static DoubleBufferLazyInitializeTask<LogValueContext> DoubleBufferTask { get; set; } = new DoubleBufferLazyInitializeTask<LogValueContext>(OnWriteLog);

		private static List<ILoggerFileWriter> LoggerFileWriterList { get; set; } = null;

		public static void Fatal(Exception e)
		{
			if (IsEnabled)
			{
				LogValueContext context = new LogValueContext(LogLevel.Critical, null, e);
				WriteLog(ref context);
			}
		}

		public static void Fatal(string fatalDescription, Exception e)
		{
			if (IsEnabled)
			{
				LogValueContext context = new LogValueContext(LogLevel.Critical, fatalDescription, e);
				WriteLog(ref context);
			}
		}

		public static void Fatal(string fatalDescription)
		{
			if (IsEnabled)
			{
				LogValueContext context = new LogValueContext(LogLevel.Critical, fatalDescription);
				WriteLog(ref context);
			}
		}

		public static void Error(Exception e)
		{
			if (IsEnabled)
			{
				LogValueContext context = new LogValueContext(LogLevel.Error, null, e);
				WriteLog(ref context);
			}
		}

		public static void Error(string errorDescription, Exception e)
		{
			if (IsEnabled)
			{
				LogValueContext context = new LogValueContext(LogLevel.Error, errorDescription, e);
				WriteLog(ref context);
			}
		}

		public static void Error(string errorDescription)
		{
			if (IsEnabled)
			{
				LogValueContext context = new LogValueContext(LogLevel.Error, errorDescription);
				WriteLog(ref context);
			}
		}

		public static void Warn(Exception e)
		{
			if (IsEnabled)
			{
				LogValueContext context = new LogValueContext(LogLevel.Warning, null, e);
				WriteLog(ref context);
			}
		}

		public static void Warn(string warnDescription, Exception e)
		{
			if (IsEnabled)
			{
				LogValueContext context = new LogValueContext(LogLevel.Warning, warnDescription, e);
				WriteLog(ref context);
			}
		}

		public static void Warn(string warnDescription)
		{
			if (IsEnabled)
			{
				LogValueContext context = new LogValueContext(LogLevel.Warning, warnDescription);
				WriteLog(ref context);
			}
		}

		public static void Info(Exception e)
		{
			if (IsEnabled)
			{
				LogValueContext context = new LogValueContext(LogLevel.Information, null, e);
				WriteLog(ref context);
			}
		}

		public static void Info(string infoDescription, Exception e)
		{
			if (IsEnabled)
			{
				LogValueContext context = new LogValueContext(LogLevel.Information, infoDescription, e);
				WriteLog(ref context);
			}
		}

		public static void Info(string infoDescription)
		{
			if (IsEnabled)
			{
				LogValueContext context = new LogValueContext(LogLevel.Information, infoDescription);
				WriteLog(ref context);
			}
		}

		public static void Debug(Exception e)
		{
			if (IsEnabled)
			{
				LogValueContext context = new LogValueContext(LogLevel.Debug, null, e);
				WriteLog(ref context);
			}
		}

		public static void Debug(string debugDescription, Exception e)
		{
			if (IsEnabled)
			{
				LogValueContext context = new LogValueContext(LogLevel.Debug, debugDescription, e);
				WriteLog(ref context);
			}
		}

		public static void Debug(string debugDescription)
		{
			if (IsEnabled)
			{
				LogValueContext context = new LogValueContext(LogLevel.Debug, debugDescription);
				WriteLog(ref context);
			}
		}

		private static void WriteLog([In] ref LogValueContext context)
		{
			DoubleBufferTask.AddTask(context);
		}

		public static void Configure(LoggerConfiguration configuration)
		{
			if (_isConfigured)
			{
				throw new InvalidOperationException("禁止多次配置日志");
			}
			_isConfigured = true;
			LoggerFileWriterList = configuration.Build();
			_loggerDisposer = new LoggerDisposer();
			foreach (Action<ILoggerDisposer> loggerDisposerConfiguration in configuration.LoggerDisposerConfigurationList)
			{
				loggerDisposerConfiguration(_loggerDisposer);
			}
			_logFolder = configuration.LogFolder ?? "";
			DoubleBufferTask.OnInitialized();
		}

		private static async Task OnWriteLog(List<LogValueContext> logValueContextList)
		{
			foreach (ILoggerFileWriter loggerFileWriter in LoggerFileWriterList)
			{
				try
				{
					await loggerFileWriter.LogAsync(logValueContextList).ConfigureAwait(false);
				}
				catch (Exception)
				{
				}
			}
		}
	}
}
