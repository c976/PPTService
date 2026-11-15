using PPTService.Ink.Core.Internal.Ink;

namespace Cvte.Paint.Inks.PointToGeometry
{
	internal class DefaultPressureFactorToScaleConverter : IPressureFactorToScaleConverter
	{
		public float PressureFactorToScalePressureFactor(float pressureFactor)
		{
			return StrokeNodeIterator.GetNormalizedPressureFactor(pressureFactor);
		}
	}
}
