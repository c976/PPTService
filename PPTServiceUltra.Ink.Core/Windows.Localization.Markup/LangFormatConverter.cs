using System;
using System.ComponentModel;
using System.Globalization;
using System.Windows;
using System.Windows.Data;
using System.Windows.Markup;

namespace PPTServiceUltra.Ink.Core.Windows.Localization.Markup
{
	[ValueConversion(typeof(string), typeof(string))]
	public sealed class LangFormatConverter : MarkupExtension, IValueConverter, IMultiValueConverter
	{
		[ConstructorArgument("key")]
		public string Key { get; set; }

		public LangFormatConverter()
		{
		}

		public LangFormatConverter(string key)
		{
			Key = key;
		}

		object IValueConverter.Convert(object value, Type targetType, object parameter, CultureInfo culture)
		{
			if (Key == null && DesignerProperties.GetIsInDesignMode(new DependencyObject()))
			{
				return null;
			}
			return Lang.Get(Key, value);
		}

		object IValueConverter.ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
		{
			throw new NotSupportedException();
		}

		object IMultiValueConverter.Convert(object[] values, Type targetType, object parameter, CultureInfo culture)
		{
			if (Key == null && DesignerProperties.GetIsInDesignMode(new DependencyObject()))
			{
				return null;
			}
			return Lang.Get(Key, values);
		}

		object[] IMultiValueConverter.ConvertBack(object value, Type[] targetTypes, object parameter, CultureInfo culture)
		{
			throw new NotSupportedException();
		}

		public override object ProvideValue(IServiceProvider serviceProvider)
		{
			return this;
		}
	}
}
