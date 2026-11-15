using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using PPTService.Ink.Core.ComponentModel;

namespace PPTService.Ink.Core.Diagnostics.Trace
{
	
	
	public static class Tracer
	{
		
		private class OutputTracer : ITracer
		{
			private static ITracer TracerListener => TraceListener.Instance;

			public void Trace(string eventId, string eventValue = null, params string[] tag)
			{
				TracerListener.Trace(eventId, eventValue, tag);
			}

			public void Trace(string eventId,  IEnumerable<KeyValuePair<string, string>> eventParams, params string[] tag)
			{
				TracerListener.Trace(eventId, eventParams, tag);
			}

			public void Trace(Exception exception, string type = null, params string[] tag)
			{
				TracerListener.Trace(exception, type, tag);
			}
		}

		public static DebugTags EnabledDebugTags
		{
			get
			{
				return LogTracer.EnabledDebugTags;
			}
			set
			{
				LogTracer.EnabledDebugTags = value;
			}
		}

		public static TraceLevel TraceLevel
		{
			get
			{
				return LogTracer.TraceLevel;
			}
			set
			{
				LogTracer.TraceLevel = value;
			}
		}

		private static ILogTracer LogTracer { get; }

		static Tracer()
		{
			TraceLevel traceLevel = TraceLevel.Error;
			if (DebuggingProperties.IsDebug)
			{
				traceLevel = TraceLevel.Info;
			}
			LogTracer = new LogTracer(new OutputTracer(), new string[0], traceLevel, new DebugTags());
		}

		public static ILogTracer BuildLogTracer(params string[] tags)
		{
			return LogTracer.BuildLogTracer(tags);
		}

		public static void Debug(string message, params string[] tags)
		{
			LogTracer.Debug(message, tags);
		}

		public static void Debug(Func<string> getMessage, params string[] tags)
		{
			LogTracer.Debug(getMessage, tags);
		}

		public static void Error(string message, params string[] tags)
		{
			LogTracer.Error(message, tags);
		}

		public static void Error(Exception exception, params string[] tags)
		{
			LogTracer.Error(exception, tags);
		}

		public static void Info(string message, params string[] tags)
		{
			LogTracer.Info(message, tags);
		}

		public static void Info(Func<string> getMessage, params string[] tags)
		{
			LogTracer.Info(getMessage, tags);
		}

		public static void Warning(string message, params string[] tags)
		{
			LogTracer.Warning(message, tags);
		}

		public static void Warning(Func<string> getMessage, params string[] tags)
		{
			LogTracer.Warning(getMessage, tags);
		}

		public static void Warning(Exception exception, params string[] tags)
		{
			LogTracer.Warning(exception, tags);
		}
	}
}
