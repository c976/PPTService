using System.Linq;
using Cvte.Paint.Framework;
using Cvte.Paint.Inks.HighPrecision.Erasing.CircleErasing;
using Cvte.Paint.Inks.HighPrecision.Erasing.CornerRadiusRectangleErasing;

namespace Cvte.Paint.Inks.HighPrecision.Erasing
{
	public class LargeObjectOrFingerSensitiveEraserModeOperation : HighPrecisionEraserModeOperation
	{
		protected override IHighPrecisionEraserHandler GetOrCreatorEraserInkHandler(ModeInputEventArgs e)
		{
			IHighPrecisionEraserHandler value;
			if (!EraserInkHandlerList.TryGetValue(e.Id, out value))
			{
				IHighPrecisionEraserHandler highPrecisionEraserHandler;
				switch (HighPrecisionInputHelper.GetDoublePenInputTypeByStylusPointForFlagFrogTouchFrame(e.GetStylusPoints().First()))
				{
				case HighPrecisionDoublePenInputType.LargeObject:
					highPrecisionEraserHandler = new CornerRadiusRectangleEraserHandler(e.Id, CurrentSlide, EraserSettings, GetBoard(), ErasingSynchronizer);
					break;
				case HighPrecisionDoublePenInputType.MissingProperties:
					highPrecisionEraserHandler = new CornerRadiusRectangleEraserHandler(e.Id, CurrentSlide, EraserSettings, GetBoard(), ErasingSynchronizer);
					break;
				default:
					highPrecisionEraserHandler = new CircleEraserHandler(e.Id, CurrentSlide, EraserSettings, GetBoard(), ErasingSynchronizer);
					break;
				}
				value = highPrecisionEraserHandler;
				EraserInkHandlerList[e.Id] = value;
			}
			return value;
		}
	}
}
