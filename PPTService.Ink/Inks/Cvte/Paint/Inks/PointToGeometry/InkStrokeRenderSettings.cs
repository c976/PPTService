using PPTService.Ink.Core.Diagnostics.Annotations;

namespace Cvte.Paint.Inks.PointToGeometry
{
	public class InkStrokeRenderSettings
	{
		public bool IsInDebugMode { get; set; }

		[CanBeNull]
		public IPressureFactorToScaleConverter PressureFactorToScaleConverter { get; set; }
	}
}
