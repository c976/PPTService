namespace Cvte.Paint.Inks.HighPrecision.Inking
{
	public class SoftPenPressureFactorByGeometryWritingMode : HighPrecisionInputFromBoardWritingMode, IDynamicPressureFactorConverter
	{
		public SoftPenPressureFactorByGeometryInfo SoftPenPressureFactorByGeometryInfo { get; set; }

		public float ConvertPressureFactor(float originPressureFactor)
		{
			return SoftPenPressureFactorByGeometryInfo?.ConvertPressureFactor(originPressureFactor) ?? originPressureFactor;
		}
	}
}
