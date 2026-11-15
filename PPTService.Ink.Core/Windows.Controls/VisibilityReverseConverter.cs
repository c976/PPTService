using System;
using System.Globalization;
using System.Windows;
using System.Windows.Data;

namespace PPTService.Ink.Core.Windows.Controls
{
	public class VisibilityReverseConverter : IValueConverter
	{
		public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
		{
			return Reverse(value);
		}

		public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
		{
			return Reverse(value);
		}

		private static object Reverse(object value)
		{
			if (value is Visibility)
			{
				return ((Visibility)value == Visibility.Visible) ? Visibility.Collapsed : Visibility.Visible;
			}
			return DependencyProperty.UnsetValue;
		}
	}
}
