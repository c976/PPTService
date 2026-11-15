using PPTService.Paint.Inks.PointToGeometry;

namespace PPTService.Paint.Inks.HighPrecision.Inking
{
	internal class HighPrecisionPressureFactorToScaleConverter : IPressureFactorToScaleConverter
	{
		public float PressureFactorToScalePressureFactor(float stylusPointPressureFactor)
		{
			return stylusPointPressureFactor + 0.001f;
		}
	}
}
