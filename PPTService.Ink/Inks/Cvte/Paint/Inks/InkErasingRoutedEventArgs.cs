using System.Windows;
using Cvte.Paint.Framework;

namespace Cvte.Paint.Inks
{
	public class InkErasingRoutedEventArgs : ErasingRoutedEventArgs
	{
		public Slide AttachedSlide { get; }

		public EraserHandler CurrentEraser { get; }

		public InkErasingRoutedEventArgs(RoutedEvent routedEvent, Slide attachedSlide, EraserHandler currentEraser)
			: base(routedEvent)
		{
			AttachedSlide = attachedSlide;
			CurrentEraser = currentEraser;
		}

		public InkErasingRoutedEventArgs(RoutedEvent routedEvent, object source, Slide attachedSlide, EraserHandler currentEraser)
			: base(routedEvent, source)
		{
			AttachedSlide = attachedSlide;
			CurrentEraser = currentEraser;
		}
	}
}
