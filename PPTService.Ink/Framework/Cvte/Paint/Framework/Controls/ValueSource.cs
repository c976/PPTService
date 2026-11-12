using System.Windows;

namespace Cvte.Paint.Framework.Controls
{
	public class ValueSource : DependencyObject
	{
		public static readonly DependencyProperty SourceProperty = DependencyProperty.Register("Source", typeof(object), typeof(ValueSource), new PropertyMetadata((object)null));

		public static readonly DependencyProperty ValueProperty = DependencyProperty.Register("Value", typeof(object), typeof(ValueSource), new PropertyMetadata((object)null));

		public object Source
		{
			get
			{
				return GetValue(SourceProperty);
			}
			set
			{
				SetValue(SourceProperty, value);
			}
		}

		public object Value
		{
			get
			{
				return GetValue(ValueProperty);
			}
			set
			{
				SetValue(ValueProperty, value);
			}
		}
	}
}
