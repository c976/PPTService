using System;
using Cvte.Paint.Framework;
using Cvte.Paint.Framework.Settings;

namespace Cvte.Paint.Inks
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
