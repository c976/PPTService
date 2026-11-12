using Cvte.Diagnostics.Trace;

namespace Cvte.Paint.Framework.Settings
{
	public class TraceSettings : BoardSettings
	{
		public const string TracerTag = "Paint";

		public ITracer Tracer { get; set; }

		public ILogTracer LogTracer { get; set; }

		public TraceSettings()
		{
			LogTracer = Cvte.Diagnostics.Trace.Tracer.BuildLogTracer("Paint");
			Tracer = LogTracer;
		}
	}
}
