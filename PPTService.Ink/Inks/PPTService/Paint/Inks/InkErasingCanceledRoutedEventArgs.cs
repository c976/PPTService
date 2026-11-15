using System.Windows;
using PPTService.Paint.Framework;

namespace PPTService.Paint.Inks
{
	public class InkErasingCanceledRoutedEventArgs : InkErasingRoutedEventArgs
	{
		public InkErasingCanceledRoutedEventArgs(RoutedEvent routedEvent, Slide attachedSlide, EraserHandler currentEraser)
			: base(routedEvent, attachedSlide, currentEraser)
		{
		}

		public InkErasingCanceledRoutedEventArgs(RoutedEvent routedEvent, object source, Slide attachedSlide, EraserHandler currentEraser)
			: base(routedEvent, source, attachedSlide, currentEraser)
		{
		}
	}
}
