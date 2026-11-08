using System;
using System.ComponentModel;
using System.Globalization;

namespace Cvte.Windows.Localization
{
	public class ReferencedLangConverter : TypeConverter
	{
		public override bool CanConvertFrom(ITypeDescriptorContext context, Type sourceType)
		{
			return sourceType == typeof(string);
		}

		public override object ConvertFrom(ITypeDescriptorContext context, CultureInfo culture, object value)
		{
			if (value == null)
			{
				return null;
			}
			return new ReferencedLang(value.ToString());
		}
	}
}
