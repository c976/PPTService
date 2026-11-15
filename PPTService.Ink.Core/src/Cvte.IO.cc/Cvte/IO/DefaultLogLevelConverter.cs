using System.Runtime.CompilerServices;

namespace PPTService.Ink.Core.IO
{
	public static class DefaultLogLevelConverter
	{
		
		public static string LogLevelToText(LogLevel logLevel)
		{
			switch (logLevel)
			{
			case LogLevel.Critical:
				return "Fatal";
			case LogLevel.Error:
				return "Error";
			case LogLevel.Warning:
				return "Warn";
			case LogLevel.Information:
				return "Info";
			case LogLevel.Debug:
				return "Debug";
			case LogLevel.Trace:
				return "Trace";
			case LogLevel.None:
				return "None";
			default:
				return string.Empty;
			}
		}
	}
}
