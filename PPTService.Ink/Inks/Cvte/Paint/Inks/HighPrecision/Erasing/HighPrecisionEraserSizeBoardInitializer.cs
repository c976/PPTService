using System;
using System.Linq;
using System.Windows;
using System.Windows.Input;
using Cvte.Paint.Framework;
using Cvte.Paint.Framework.Settings;
using Cvte.Paint.Settings;
using Cvte.Windows;
using Cvte.Windows.Input;

namespace Cvte.Paint.Inks.HighPrecision.Erasing
{
	public class HighPrecisionEraserSizeBoardInitializer : IPaintBoardInitializer
	{
		public void Initialize(Board board)
		{
			board.GetSettings<EraserGestureSettings>().GetEraserSizeAlgorithm = (DeviceInputEventArgs arg, Mode mode) =>
			{
				Size size = mode.CurrentSlide.GetMonitor().PixelBounds.Size;
				Size scalingRatioToDevice = mode.CurrentSlide.GetScalingRatioToDevice();
				StylusPoint stylusPoint = arg.GetStylusPoints().First();
				if (stylusPoint.HasProperty(StylusPointProperties.Width) && stylusPoint.HasProperty(StylusPointProperties.Height) && stylusPoint.HasProperty(StylusPointProperties.AzimuthOrientation))
				{
					StylusPointPropertyInfo propertyInfo = stylusPoint.Description.GetPropertyInfo(StylusPointProperties.Width);
					double num = (double)stylusPoint.GetPropertyValue(StylusPointProperties.Width) * size.Width / (double)propertyInfo.Maximum / scalingRatioToDevice.Width;
					StylusPointPropertyInfo propertyInfo2 = stylusPoint.Description.GetPropertyInfo(StylusPointProperties.Height);
					double num2 = (double)stylusPoint.GetPropertyValue(StylusPointProperties.Height) * size.Height / (double)propertyInfo2.Maximum / scalingRatioToDevice.Height;
					StylusPointPropertyInfo propertyInfo3 = stylusPoint.Description.GetPropertyInfo(StylusPointProperties.AzimuthOrientation);
					float num3 = (float)stylusPoint.GetPropertyValue(StylusPointProperties.AzimuthOrientation) / propertyInfo3.Resolution;
					double num4 = Math.Abs(Math.Cos(num3) * num) + Math.Abs(Math.Sin(num3) * num2);
					double num5 = Math.Abs(Math.Cos(num3) * num2) + Math.Abs(Math.Sin(num3) * num);
					if (num5 < 1.5 * num4)
					{
						num5 = 1.5 * num4;
					}
					else
					{
						num4 = num5 / 1.5;
					}
					Size value = new Size(EraserSettings.DefaultEraserSize.Width / board.ContentScaleX, EraserSettings.DefaultEraserSize.Height / board.ContentScaleY);
					if (num4 < value.Width)
					{
						return value;
					}
					return new Size(num4, num5);
				}
				return EraserSettings.DefaultEraserSize;
			};
			board.GetSettings<EraserGestureSettings>().OnIsEraserGestureByInputArgs = (DeviceInputEventArgs arg, Slide _) =>
			{
				StylusPoint stylusPoint = arg.GetStylusPoints().First();
				return stylusPoint.HasProperty(StylusPointProperties.Width) && stylusPoint.HasProperty(StylusPointProperties.Height) && stylusPoint.GetPropertyValue(StylusPointProperties.Width) >= 500 && stylusPoint.GetPropertyValue(StylusPointProperties.Height) >= 500;
			};
		}
	}
}
