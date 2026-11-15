using System;
using PPTService.Paint.Framework;
using PPTService.Paint.Framework.Settings;

namespace PPTService.Paint.Inks
{
	internal static class EraserSizeTracer
	{
		private static DateTime LastUploadTime { get; set; }

		public static void Trace(Slide slide, string message)
		{
			DateTime now = DateTime.Now;
			if (!((now - LastUploadTime).TotalSeconds <= 100.0))
			{
				LastUploadTime = now;
				slide.GetSettings<TraceSettings>().Tracer?.Trace("EraserSize", message);
			}
		}
	}
}
