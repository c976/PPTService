using PPTServiceUltra.Ink.Core.Diagnostics.Annotations;

namespace PPTServiceUltra.Paint.Inks.PointToGeometry
{
	public class InkStrokeRenderSettings
	{
		public bool IsInDebugMode { get; set; }

		[CanBeNull]
		public IPressureFactorToScaleConverter PressureFactorToScaleConverter { get; set; }
	}
}
