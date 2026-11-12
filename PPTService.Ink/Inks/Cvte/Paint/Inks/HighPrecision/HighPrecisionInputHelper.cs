using System.Linq;
using System.Windows;
using System.Windows.Input;
using Cvte.Paint.Framework;
using Cvte.Paint.Inks.HighPrecision.Inking;

namespace Cvte.Paint.Inks.HighPrecision
{
	public static class HighPrecisionInputHelper
	{
		public static HighPrecisionDoublePenInputType GetDoublePenInputTypeByStylusPointForFlagFrogTouchFrame(StylusPoint stylusPoint)
		{
			StylusPointDescription description = stylusPoint.Description;
			if (description != null && description.HasProperty(StylusPointProperties.Width))
			{
				StylusPointDescription description2 = stylusPoint.Description;
				if (description2 != null && description2.HasProperty(StylusPointProperties.Height))
				{
					int propertyValue = stylusPoint.GetPropertyValue(StylusPointProperties.Width);
					int propertyValue2 = stylusPoint.GetPropertyValue(StylusPointProperties.Height);
					int num = propertyValue;
					int num2 = propertyValue2;
					switch (num)
					{
					case 20:
						if (num2 != 20)
						{
							break;
						}
						return HighPrecisionDoublePenInputType.ThinPen;
					case 40:
						if (num2 != 40)
						{
							break;
						}
						return HighPrecisionDoublePenInputType.ThickPen;
					case 80:
						if (num2 != 80)
						{
							break;
						}
						return HighPrecisionDoublePenInputType.Finger;
					case 100:
						if (num2 != 100)
						{
							break;
						}
						return HighPrecisionDoublePenInputType.SimulatePressure;
					}
					if (propertyValue >= 500 && propertyValue2 > 500)
					{
						return HighPrecisionDoublePenInputType.LargeObject;
					}
					return HighPrecisionDoublePenInputType.Unknown;
				}
			}
			return HighPrecisionDoublePenInputType.MissingProperties;
		}

		public static bool PredefinedCanManipulationFuncForFlagFrogTouchFrame(Slide slide, IInputElement element, StylusDownEventArgs args)
		{
			Board board = slide.Parent as Board;
			if (board != null)
			{
				HighPrecisionMixInputMode highPrecisionMixInputMode = board.Mode as HighPrecisionMixInputMode;
				if (highPrecisionMixInputMode != null && highPrecisionMixInputMode.IsSmartMode && GetDoublePenInputTypeByStylusPointForFlagFrogTouchFrame(args.GetStylusPoints(element).First()) != HighPrecisionDoublePenInputType.Finger)
				{
					return false;
				}
			}
			return true;
		}
	}
}
