using System;
using Cvte.Diagnostics.Trace;

namespace Cvte.Paint.Inks.Hardware.RawTouchEngine
{
	internal static class TouchDebugger
	{
		public const string TouchTag = "TouchSDK";

		private static readonly Lazy<ILogTracer> _logTracer = new Lazy<ILogTracer>(() => InkTracer.LogTracer.BuildLogTracer("TouchSDK"));

		public static ILogTracer LogTracer => _logTracer.Value;

		public static void Log(string message)
		{
			LogTracer.Debug(message);
		}
	}
}
