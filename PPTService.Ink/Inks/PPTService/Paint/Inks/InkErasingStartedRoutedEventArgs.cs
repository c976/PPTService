using System.Windows;
using PPTService.Paint.Framework;

namespace PPTService.Paint.Inks
{
	public class InkErasingStartedRoutedEventArgs : InkErasingRoutedEventArgs
	{
		public InkErasingStartedRoutedEventArgs(RoutedEvent routedEvent, Slide attachedSlide, EraserHandler currentEraser)
			: base(routedEvent, attachedSlide, currentEraser)
		{
		}

		public InkErasingStartedRoutedEventArgs(RoutedEvent routedEvent, object source, Slide attachedSlide, EraserHandler currentEraser)
			: base(routedEvent, source, attachedSlide, currentEraser)
		{
		}
	}
}
