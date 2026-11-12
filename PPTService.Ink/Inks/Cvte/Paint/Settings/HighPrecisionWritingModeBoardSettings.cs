using Cvte.Diagnostics.Annotations;
using Cvte.Paint.Framework.Settings;
using Cvte.Paint.Inks.PointToGeometry;

namespace Cvte.Paint.Settings
{
	public class HighPrecisionWritingModeBoardSettings : BoardSettings
	{
		public bool NeedCollectStrokeDetails { get; set; }

		[CanBeNull]
		public InkStrokeRenderSettings InkStrokeRenderSettings { get; set; }
	}
}
