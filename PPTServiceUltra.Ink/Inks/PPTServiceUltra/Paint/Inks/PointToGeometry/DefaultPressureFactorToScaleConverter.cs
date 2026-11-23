using PPTServiceUltra.Ink.Core.Internal.Ink;

namespace PPTServiceUltra.Paint.Inks.PointToGeometry
{
	internal class DefaultPressureFactorToScaleConverter : IPressureFactorToScaleConverter
	{
		public float PressureFactorToScalePressureFactor(float pressureFactor)
		{
			return StrokeNodeIterator.GetNormalizedPressureFactor(pressureFactor);
		}
	}
}
