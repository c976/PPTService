using PPTService.Ink.Core.Diagnostics.Annotations;
using PPTService.Paint.Framework.Settings;
using PPTService.Paint.Inks.PointToGeometry;

namespace PPTService.Paint.Settings
{
	public class HighPrecisionWritingModeBoardSettings : BoardSettings
	{
		public bool NeedCollectStrokeDetails { get; set; }

		[CanBeNull]
		public InkStrokeRenderSettings InkStrokeRenderSettings { get; set; }
	}
}
