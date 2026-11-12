using System.Windows;

namespace Cvte.Paint.Framework
{
	public class SlideChangeRoutedEventArgs : SlideRoutedEventArgs
	{
		public DependencyProperty Property { get; private set; }

		public SlideChangeRoutedEventArgs(RoutedEvent routedEvent, Slide slide, DependencyProperty property)
			: base(routedEvent, slide)
		{
			Property = property;
		}

		public SlideChangeRoutedEventArgs(Slide slide, DependencyProperty property)
			: base(slide)
		{
			Property = property;
		}
	}
}
