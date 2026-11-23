using PPTServiceUltra.Paint.Inks.PointToGeometry;

namespace PPTServiceUltra.Paint.Inks.HighPrecision.Inking
{
	internal class HighPrecisionPressureFactorToScaleConverter : IPressureFactorToScaleConverter
	{
		public float PressureFactorToScalePressureFactor(float stylusPointPressureFactor)
		{
			return stylusPointPressureFactor + 0.001f;
		}
	}
}
