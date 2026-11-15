using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;

namespace PPTService.Ink.Core.Diagnostics.Trace
{
	
	public interface ILogTracer : ITracer
	{
		DebugTags EnabledDebugTags { get; set; }

		TraceLevel TraceLevel { get; set; }

		IReadOnlyCollection<string> TracerTags { get; }

		void Debug(string message, params string[] tags);

		void Debug(Func<string> messageFactory, params string[] tags);

		void Info(string message, params string[] tags);

		void Info(Func<string> messageFactory, params string[] tags);

		void Warning(string message, params string[] tags);

		void Warning(Func<string> messageFactory, params string[] tags);

		void Warning(Exception exception, params string[] tags);

		void Error(string message, params string[] tags);

		void Error(Exception exception, params string[] tags);

		ILogTracer BuildLogTracer(params string[] tags);
	}
}
