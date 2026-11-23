using System;
using PPTServiceUltra.Ink.Core.Diagnostics.Trace;

namespace PPTServiceUltra.Paint.Inks.Hardware.RawTouchEngine
{
	internal static class TouchDebugger
	{
		public const string TouchTag = "TouchSDK";

		private static readonly Lazy<ILogTracer> _logTracer = new Lazy<ILogTracer>(() => PPTServiceUltra.Ink.Core.Diagnostics.Trace.Tracer.BuildLogTracer("TouchSDK"));

		public static ILogTracer LogTracer => _logTracer.Value;

		public static void Log(string message)
		{
			LogTracer.Debug(message);
		}
	}
}
