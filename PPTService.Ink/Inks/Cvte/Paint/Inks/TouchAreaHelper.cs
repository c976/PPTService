using System;
using System.Windows.Input;

namespace Cvte.Paint.Inks
{
	public static class TouchAreaHelper
	{
		public const double StandWidth = 140.0;

		public static StylusPointCollection GetPressurStylusPoints(StylusPointCollection points)
		{
			StylusPointCollection stylusPointCollection = new StylusPointCollection(points.Description);
			for (int i = 0; i < points.Count; i++)
			{
				StylusPoint stylusPoint = points[i];
				double stylusPointWidthOrHeight = GetStylusPointWidthOrHeight(stylusPoint, true);
				double stylusPointWidthOrHeight2 = GetStylusPointWidthOrHeight(stylusPoint, false);
				float num = (float)(Math.Max(stylusPointWidthOrHeight, stylusPointWidthOrHeight2) / 140.0);
				if (num > 1f)
				{
					num = 1f;
				}
				if (num < 0f)
				{
					num = 1E-05f;
				}
				stylusPoint.PressureFactor = num;
				stylusPointCollection.Add(stylusPoint);
			}
			return stylusPointCollection;
		}

		public static bool HasTouchAreaInfo(StylusPointCollection points)
		{
			return points.Description.HasProperty(StylusPointProperties.Width);
		}

		public static bool HasPressureInfo(StylusPointCollection points)
		{
			foreach (StylusPoint point in points)
			{
				if (Math.Abs((double)point.PressureFactor - 0.5) > 1E-05)
				{
					return true;
				}
			}
			return false;
		}

		public static double GetStylusPointWidthOrHeight(StylusPoint stylusPoint, bool isWidth)
		{
			StylusPointProperty stylusPointProperty = (isWidth ? StylusPointProperties.Width : StylusPointProperties.Height);
			double result = 0.0;
			if (stylusPoint.HasProperty(stylusPointProperty))
			{
				result = stylusPoint.GetPropertyValue(stylusPointProperty);
				StylusPointPropertyInfo propertyInfo = stylusPoint.Description.GetPropertyInfo(stylusPointProperty);
				result = (AreClose(propertyInfo.Resolution, 0.0) ? 0.0 : (result / (double)propertyInfo.Resolution));
				if (propertyInfo.Unit == StylusPointPropertyUnit.Centimeters)
				{
					result /= 2.54;
				}
				result *= 96.0;
			}
			return result;
		}

		private static bool AreClose(double value1, double value2)
		{
			if (value1 == value2)
			{
				return true;
			}
			double num = (Math.Abs(value1) + Math.Abs(value2) + 10.0) * 2.220446049250313E-16;
			double num2 = value1 - value2;
			if (0.0 - num < num2)
			{
				return num > num2;
			}
			return false;
		}
	}
}
