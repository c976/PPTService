using System;
using System.ComponentModel;
using System.Globalization;
using System.Windows.Data;

namespace PPTServiceUltra.Ink.Core.Windows.Data
{
	[Obsolete("类名错误，改用 InverseBooleanConverter")]
	[EditorBrowsable(EditorBrowsableState.Never)]
	public class BooleanReverseConverter : IValueConverter
	{
		public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
		{
			return !(value is bool) || !(bool)value;
		}

		public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
		{
			return !(value is bool) || !(bool)value;
		}
	}
}
