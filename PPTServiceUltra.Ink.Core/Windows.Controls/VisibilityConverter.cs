using System;
using System.Globalization;
using System.Windows;
using System.Windows.Data;

namespace PPTServiceUltra.Ink.Core.Windows.Controls
{
	public class VisibilityConverter : IValueConverter
	{
		public object Visible { get; set; }

		public object Hidden { get; set; }

		public object Collapsed { get; set; }

		public Visibility DefaultVisibility { get; set; }

		public VisibilityConverter()
		{
			DefaultVisibility = Visibility.Collapsed;
		}

		public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
		{
			if (object.Equals(value, Visible))
			{
				return Visibility.Visible;
			}
			if (object.Equals(value, Hidden))
			{
				return Visibility.Hidden;
			}
			if (object.Equals(value, Collapsed))
			{
				return Visibility.Collapsed;
			}
			return DefaultVisibility;
		}

		public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
		{
			if (value == null)
			{
				throw new ArgumentNullException();
			}
			switch ((Visibility)value)
			{
			case Visibility.Visible:
				return Visible;
			case Visibility.Hidden:
				return Hidden;
			case Visibility.Collapsed:
				return Collapsed;
			default:
				throw new ArgumentException($"不支持指定值 {value} 的转换。");
			}
		}
	}
}
