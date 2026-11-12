using System.Windows;

namespace Cvte.Paint.Framework
{
	public class SpecialSlide
	{
		public static readonly DependencyProperty KeepIndependentSizeProperty = DependencyProperty.RegisterAttached("KeepIndependentSize", typeof(bool), typeof(SpecialSlide), new PropertyMetadata(false));

		public static bool GetKeepIndependentSize(DependencyObject element)
		{
			return (bool)element.GetValue(KeepIndependentSizeProperty);
		}

		public static void SetKeepIndependentSize(DependencyObject element, bool value)
		{
			element.SetValue(KeepIndependentSizeProperty, value);
		}
	}
}
