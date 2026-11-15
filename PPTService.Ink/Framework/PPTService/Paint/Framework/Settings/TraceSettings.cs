using PPTService.Ink.Core.Diagnostics.Trace;

namespace PPTService.Paint.Framework.Settings
{
	public class TraceSettings : BoardSettings
	{
		public const string TracerTag = "Paint";

		public ITracer Tracer { get; set; }

		public ILogTracer LogTracer { get; set; }

		public TraceSettings()
		{
			LogTracer = PPTService.Ink.Core.Diagnostics.Trace.Tracer.BuildLogTracer("Paint");
			Tracer = LogTracer;
		}
	}
}
