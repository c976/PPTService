using System;
using System.ComponentModel;
using System.Globalization;
using System.Windows;
using System.Windows.Data;

namespace Cvte.Windows
{
	internal class ResourceKeyToResourceConverter : IMultiValueConverter
	{
		[DefaultValue(null)]
		public object ConverterParameter { get; set; }

		[DefaultValue(null)]
		public IValueConverter ResourceKeyConverter { get; set; }

		[DefaultValue(null)]
		public string StringFormat { get; set; }

		public Func<FrameworkElement, object, object> FindResource { get; set; }

		public object Convert(object[] values, Type targetType, object parameter, CultureInfo culture)
		{
			if (values.Length < 2)
			{
				return null;
			}
			FrameworkElement frameworkElement = values[0] as FrameworkElement;
			object result;
			do
			{
				object obj = values[1];
				if (frameworkElement == null)
				{
					return null;
				}
				if (ResourceKeyConverter != null)
				{
					obj = ResourceKeyConverter.Convert(obj, targetType, ConverterParameter, culture);
				}
				else if (StringFormat != null)
				{
					obj = string.Format(StringFormat, obj);
				}
				result = ((FindResource == null) ? frameworkElement.TryFindResource(obj) : FindResource(frameworkElement, obj));
			}
			while (4 == 0);
			return result;
		}

		public object[] ConvertBack(object value, Type[] targetTypes, object parameter, CultureInfo culture)
		{
			throw new NotSupportedException("不支持资源到资源 Key 的转换。");
		}
	}
}
