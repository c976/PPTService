using System;
using System.Globalization;
using System.Linq;
using System.Windows.Data;

namespace PPTService.Ink.Core.Windows
{
	internal class MinValueConverter : IMultiValueConverter
	{
		public static readonly MinValueConverter Instance = new MinValueConverter();

		public object Convert(object[] values, Type targetType, object parameter, CultureInfo culture)
		{
			return values.OfType<double>().Min();
		}

		public object[] ConvertBack(object value, Type[] targetTypes, object parameter, CultureInfo culture)
		{
			throw new NotSupportedException();
		}
	}
}
