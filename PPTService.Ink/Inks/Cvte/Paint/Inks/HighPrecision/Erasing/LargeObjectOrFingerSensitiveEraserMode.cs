namespace Cvte.Paint.Inks.HighPrecision.Erasing
{
	public class LargeObjectOrFingerSensitiveEraserMode : HighPrecisionEraserMode, IEraserMode
	{
		public LargeObjectOrFingerSensitiveEraserMode(bool isElementInteractionEnabled = false)
			: base(new LargeObjectOrFingerSensitiveEraserModeOperation(), isElementInteractionEnabled)
		{
		}
	}
}
