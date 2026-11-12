using System.Windows;
using System.Windows.Media;

namespace Cvte.Paint.Framework
{
	public static class SlideStyle
	{
		public static readonly DependencyProperty BaseBackgroundProperty = DependencyProperty.RegisterAttached("BaseBackground", typeof(Brush), typeof(SlideStyle), new PropertyMetadata((object)null));

		public static void SetBaseBackground(DependencyObject element, Brush value)
		{
			element.SetValue(BaseBackgroundProperty, value);
		}

		public static Brush GetBaseBackground(DependencyObject element)
		{
			return (Brush)element.GetValue(BaseBackgroundProperty);
		}
	}
}
