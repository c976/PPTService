using PPTServiceUltra.Ink.Core.Diagnostics.Annotations;
using PPTServiceUltra.Paint.Framework.Settings;
using PPTServiceUltra.Paint.Inks.PointToGeometry;

namespace PPTServiceUltra.Paint.Settings
{
	public class HighPrecisionWritingModeBoardSettings : BoardSettings
	{
		public bool NeedCollectStrokeDetails { get; set; }

		[CanBeNull]
		public InkStrokeRenderSettings InkStrokeRenderSettings { get; set; }
	}
}
