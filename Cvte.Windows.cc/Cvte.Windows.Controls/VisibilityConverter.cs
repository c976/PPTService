using System;
using System.Globalization;
using System.Windows;
using System.Windows.Data;

namespace Cvte.Windows.Controls
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
			if (7 != 0 && 0 == 0)
			{
				bool flag = object.Equals(value, Visible);
				do
				{
					if (flag)
					{
						return Visibility.Visible;
					}
					flag = object.Equals(value, Hidden);
				}
				while (6 == 0);
				if (!flag)
				{
					if (0 == 0)
					{
						if (3 == 0)
						{
							goto IL_0024;
						}
						if (object.Equals(value, Collapsed))
						{
							return Visibility.Collapsed;
						}
					}
					return DefaultVisibility;
				}
			}
			goto IL_0024;
			IL_0024:
			return Visibility.Hidden;
		}

		public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
		{
			Visibility visibility = (Visibility)value;
			Visibility visibility2;
			do
			{
				visibility2 = visibility;
			}
			while (false);
			if (7 != 0)
			{
				switch (visibility2)
				{
				case Visibility.Visible:
					if (6 == 0)
					{
						goto default;
					}
					if (0 == 0)
					{
						return Visible;
					}
					goto case Visibility.Collapsed;
				case Visibility.Collapsed:
					return Collapsed;
				default:
					if (false)
					{
						break;
					}
					throw new ArgumentException($"不支持指定值 {value} 的转换。");
				case Visibility.Hidden:
					break;
				}
			}
			return Hidden;
		}
	}
}
