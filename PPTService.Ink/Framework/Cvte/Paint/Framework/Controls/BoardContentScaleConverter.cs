using System;
using System.Globalization;
using System.Windows;
using System.Windows.Data;

namespace Cvte.Paint.Framework.Controls
{
	public class BoardContentScaleConverter : IValueConverter
	{
		public static readonly BoardContentScaleConverter Instance = new BoardContentScaleConverter();

		object IValueConverter.Convert(object value, Type targetType, object parameter, CultureInfo culture)
		{
			double num = (double)value;
			if (targetType == typeof(GridLength))
			{
				return new GridLength((double)parameter / num);
			}
			if (targetType == typeof(Thickness))
			{
				if (parameter is double)
				{
					return new Thickness((double)parameter / num);
				}
				Thickness thickness = (Thickness)parameter;
				return new Thickness(thickness.Left / num, thickness.Top / num, thickness.Right / num, thickness.Bottom / num);
			}
			if (targetType == typeof(CornerRadius))
			{
				if (parameter is double)
				{
					return new CornerRadius((double)parameter / num);
				}
				CornerRadius cornerRadius = (CornerRadius)parameter;
				return new CornerRadius(cornerRadius.TopLeft / num, cornerRadius.TopRight / num, cornerRadius.BottomRight / num, cornerRadius.BottomLeft / num);
			}
			if (parameter is double)
			{
				return (double)parameter / (double)value;
			}
			return 0.0;
		}

		object IValueConverter.ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
		{
			throw new NotSupportedException();
		}
	}
}
