using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;

namespace PPTServiceUltra.Ink.Core.IO
{
	
	
	public class LoggerConfiguration
	{
		
		public const string LastFatalLoggerFileName = "last_fatal.txt";

		
		public const string AllFatalLoggerFileName = "all_fatal.txt";

		
		public const string LastMessageLoggerFileName = "last_message.txt";

		
		public const string AllMessageLoggerFileName = "all_message.txt";

		
		public const string LastInfoLoggerFileName = "last_info.txt";

		
		public const string AllInfoLoggerFileName = "all_info.txt";

		private string _lastFatalLoggerFile;

		private string _allFatalLoggerFile;

		private string _lastMessageLoggerFile;

		private string _allMessageLoggerFile;

		private string _lastInfoLoggerFile;

		private string _allInfoLoggerFile;

		
		
		public List<ILoggerFileWriter> LoggerFileWriterList
		{
			
			get;
			
			set;
		}

		public ILogValuesFormatter CommonLogValuesFormatter { get; private set; }

		public string LogFolder { get; private set; }

		public string LastFatalLoggerFile
		{
			get
			{
				return _lastFatalLoggerFile;
			}
			set
			{
				if (!string.IsNullOrEmpty(value))
				{
					value = Path.GetFullPath(value);
				}
				_lastFatalLoggerFile = value;
			}
		}

		public string AllFatalLoggerFile
		{
			get
			{
				return _allFatalLoggerFile;
			}
			set
			{
				if (!string.IsNullOrEmpty(value))
				{
					value = Path.GetFullPath(value);
				}
				_allFatalLoggerFile = value;
			}
		}

		public ILogValuesFormatter FatalLogValuesFormatter { get; set; }

		public string LastMessageLoggerFile
		{
			get
			{
				return _lastMessageLoggerFile;
			}
			set
			{
				if (!string.IsNullOrEmpty(value))
				{
					value = Path.GetFullPath(value);
				}
				_lastMessageLoggerFile = value;
			}
		}

		public string AllMessageLoggerFile
		{
			get
			{
				return _allMessageLoggerFile;
			}
			set
			{
				if (!string.IsNullOrEmpty(value))
				{
					value = Path.GetFullPath(value);
				}
				_allMessageLoggerFile = value;
			}
		}

		public ILogValuesFormatter MessageLogValuesFormatter { get; set; }

		public string LastInfoLoggerFile
		{
			get
			{
				return _lastInfoLoggerFile;
			}
			set
			{
				if (!string.IsNullOrEmpty(value))
				{
					value = Path.GetFullPath(value);
				}
				_lastInfoLoggerFile = value;
			}
		}

		public string AllInfoLoggerFile
		{
			get
			{
				return _allInfoLoggerFile;
			}
			set
			{
				if (!string.IsNullOrEmpty(value))
				{
					value = Path.GetFullPath(value);
				}
				_allInfoLoggerFile = value;
			}
		}

		public ILogValuesFormatter InfoLogValuesFormatter { get; set; }

		public long DefaultMaximumFileSize { get; set; } = -1L;

		public int DefaultMaxSizeRollBackups { get; set; } = 5;

		
		public Encoding DefaultEncoding
		{
			
			get
			{
				return Encoding.UTF8;
			}
		}

		
		
		internal List<Action<ILoggerDisposer>> LoggerDisposerConfigurationList
		{
			
			get;
		} = new List<Action<ILoggerDisposer>>();

		public LoggerConfiguration()
		{
		}

		
		public LoggerConfiguration(string logFolder)
		{
			SetLogFolder(logFolder);
		}

		
		internal List<ILoggerFileWriter> Build()
		{
			CreateLogFileFolder(LastFatalLoggerFile, AllFatalLoggerFile, LastMessageLoggerFile, AllMessageLoggerFile, LastInfoLoggerFile, AllInfoLoggerFile);
			DeleteWhenFileExists(LastFatalLoggerFile, LastMessageLoggerFile, LastInfoLoggerFile);
			List<ILoggerFileWriter> list = new List<ILoggerFileWriter>();
			if (CommonLogValuesFormatter == null)
			{
				ILogValuesFormatter logValuesFormatter = (CommonLogValuesFormatter = new DefaultLogValuesFormatter());
			}
			SetCommonLogValuesFormatter(CommonLogValuesFormatter);
			if (IsNotAllNull(LastFatalLoggerFile, AllFatalLoggerFile))
			{
				ILoggerFileWriter item = new LoggerFileWriter(BuildFileWriter(LastFatalLoggerFile, AllFatalLoggerFile), (LogValueContext context) => context.LogLevel == LogLevel.Critical, FatalLogValuesFormatter);
				list.Add(item);
			}
			if (IsNotAllNull(LastMessageLoggerFile, AllMessageLoggerFile))
			{
				ILoggerFileWriter item2 = new LoggerFileWriter(BuildFileWriter(LastMessageLoggerFile, AllMessageLoggerFile), (LogValueContext context) => context.LogLevel == LogLevel.Error || context.LogLevel == LogLevel.Warning || context.LogLevel == LogLevel.Critical, MessageLogValuesFormatter);
				list.Add(item2);
			}
			if (IsNotAllNull(LastInfoLoggerFile, AllInfoLoggerFile))
			{
				ILoggerFileWriter item3 = new LoggerFileWriter(BuildFileWriter(LastInfoLoggerFile, AllInfoLoggerFile), (LogValueContext context) => context.LogLevel == LogLevel.Information, InfoLogValuesFormatter);
				list.Add(item3);
			}
			if (LoggerFileWriterList != null)
			{
				list.AddRange(LoggerFileWriterList);
			}
			return list;
		}

		
		private IFileWriter BuildFileWriter(string lastLogFilePath, string allLogFilePath)
		{
			return new DefaultFileWriter(lastLogFilePath, allLogFilePath, DefaultMaximumFileSize, DefaultMaxSizeRollBackups);
		}

		private static bool IsNotAllNull(string str1, string str2)
		{
			if (string.IsNullOrEmpty(str1))
			{
				return !string.IsNullOrEmpty(str2);
			}
			return true;
		}

		private static void DeleteWhenFileExists( params string[] logFilePathList)
		{
			foreach (string text in logFilePathList)
			{
				if (!string.IsNullOrEmpty(text) && File.Exists(text))
				{
					try
					{
						File.Delete(text);
					}
					catch (Exception)
					{
					}
				}
			}
		}

		private void CreateLogFileFolder( params string[] logFilePathList)
		{
			foreach (string item in logFilePathList.Where( (string temp) => temp != null).Select(Path.GetDirectoryName).Distinct()
				.ToList())
			{
				Directory.CreateDirectory(item);
			}
		}

		
		public LoggerConfiguration SetLogFolder(string logFolder)
		{
			LogFolder = (logFolder = Path.GetFullPath(logFolder));
			if (LastFatalLoggerFile == null)
			{
				string text = (LastFatalLoggerFile = Path.Combine(logFolder, "last_fatal.txt"));
			}
			if (AllFatalLoggerFile == null)
			{
				string text = (AllFatalLoggerFile = Path.Combine(logFolder, "all_fatal.txt"));
			}
			if (LastMessageLoggerFile == null)
			{
				string text = (LastMessageLoggerFile = Path.Combine(logFolder, "last_message.txt"));
			}
			if (AllMessageLoggerFile == null)
			{
				string text = (AllMessageLoggerFile = Path.Combine(logFolder, "all_message.txt"));
			}
			if (LastInfoLoggerFile == null)
			{
				string text = (LastInfoLoggerFile = Path.Combine(logFolder, "last_info.txt"));
			}
			if (AllInfoLoggerFile == null)
			{
				string text = (AllInfoLoggerFile = Path.Combine(logFolder, "all_info.txt"));
			}
			return this;
		}

		
		public LoggerConfiguration SetCommonLogValuesFormatter(ILogValuesFormatter logValuesFormatter)
		{
			CommonLogValuesFormatter = logValuesFormatter;
			if (FatalLogValuesFormatter == null)
			{
				ILogValuesFormatter logValuesFormatter2 = (FatalLogValuesFormatter = logValuesFormatter);
			}
			if (MessageLogValuesFormatter == null)
			{
				ILogValuesFormatter logValuesFormatter2 = (MessageLogValuesFormatter = logValuesFormatter);
			}
			if (InfoLogValuesFormatter == null)
			{
				ILogValuesFormatter logValuesFormatter2 = (InfoLogValuesFormatter = logValuesFormatter);
			}
			return this;
		}

		
		public LoggerConfiguration ConfigureLoggerDisposer(Action<ILoggerDisposer> action)
		{
			LoggerDisposerConfigurationList.Add(action);
			return this;
		}
	}
}
