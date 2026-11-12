using System.Collections.Generic;
using System.Windows;

namespace Cvte.Paint.Framework
{
	public class SlidesRoutedEventArgs : RoutedEventArgs
	{
		public IEnumerable<Slide> Slides { get; private set; }

		public SlidesRoutedEventArgs(RoutedEvent routedEvent, IEnumerable<Slide> slides)
			: base(routedEvent)
		{
			Slides = slides;
		}

		public SlidesRoutedEventArgs(IEnumerable<Slide> slides)
		{
			Slides = slides;
		}
	}
}
