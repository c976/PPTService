using System.Windows;

namespace PPTServiceUltra.Paint.Framework
{
	public class SlideRoutedEventArgs : RoutedEventArgs
	{
		public Slide Slide { get; private set; }

		public SlideRoutedEventArgs(RoutedEvent routedEvent, Slide slide)
			: base(routedEvent)
		{
			Slide = slide;
		}

		public SlideRoutedEventArgs(Slide slide)
		{
			Slide = slide;
		}
	}
}
