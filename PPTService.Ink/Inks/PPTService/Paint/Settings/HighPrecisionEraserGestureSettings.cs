using System;
using System.Linq;
using System.Windows.Input;
using PPTService.Paint.Framework;
using PPTService.Paint.Framework.Settings;
using PPTService.Paint.Inks.HighPrecision.Erasing;

namespace PPTService.Paint.Settings
{
	public class HighPrecisionEraserGestureSettings : BoardFilterSettings, IUpdateBoardSettings
	{
		public Predicate<Mode> OnCanErase { get; set; }

		public Predicate<ModeInputEventArgs> IsEraserGesture { get; set; } = (ModeInputEventArgs args) =>
		{
			StylusPoint stylusPoint = args.GetStylusPoints().FirstOrDefault();
			if (stylusPoint.HasProperty(StylusPointProperties.Width) && stylusPoint.HasProperty(StylusPointProperties.Height))
			{
				int propertyValue = stylusPoint.GetPropertyValue(StylusPointProperties.Width);
				int propertyValue2 = stylusPoint.GetPropertyValue(StylusPointProperties.Height);
				if (propertyValue >= 500)
				{
					return propertyValue2 >= 500;
				}
				return false;
			}
			return false;
		};

		public Func<HighPrecisionEraserMode> EraserModeCreator { get; set; }

		public void UpdateBoardSettings(BoardSettings boardSettings)
		{
		}
	}
}
