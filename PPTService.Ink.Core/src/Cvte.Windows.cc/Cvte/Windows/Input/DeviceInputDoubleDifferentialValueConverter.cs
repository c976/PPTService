using System;
using System.ComponentModel;
using System.Globalization;

namespace PPTService.Ink.Core.Windows.Input
{
	public class DeviceInputDoubleDifferentialValueConverter : TypeConverter
	{
		public override bool CanConvertFrom(ITypeDescriptorContext context, Type sourceType)
		{
			return sourceType == typeof(string);
		}

		public override object ConvertFrom(ITypeDescriptorContext context, CultureInfo culture, object value)
		{
			string[] array = ((string)value).Split(new char[2] { ' ', ',' }, StringSplitOptions.RemoveEmptyEntries);
			if (array.Length == 1)
			{
				return new DeviceInputDoubleDifferentialValue(double.Parse(array[0]));
			}
			if (array.Length == 3)
			{
				return new DeviceInputDoubleDifferentialValue(double.Parse(array[0]), double.Parse(array[1]), double.Parse(array[2]));
			}
			throw new FormatException("不符合预定的格式：1.0 或者 1.0,2.0,1.0 或者 1.0 2.0 1.0。");
		}

		public override bool CanConvertTo(ITypeDescriptorContext context, Type destinationType)
		{
			return destinationType == typeof(string);
		}

		public override object ConvertTo(ITypeDescriptorContext context, CultureInfo culture, object value, Type destinationType)
		{
			DeviceInputDoubleDifferentialValue deviceInputDoubleDifferentialValue = (DeviceInputDoubleDifferentialValue)value;
			if (value != null)
			{
				return $"{deviceInputDoubleDifferentialValue.Mouse},{deviceInputDoubleDifferentialValue.Touch},{deviceInputDoubleDifferentialValue.Stylus}";
			}
			return null;
		}
	}
}
