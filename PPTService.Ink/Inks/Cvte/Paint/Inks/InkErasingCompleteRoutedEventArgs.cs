using System.Collections.Generic;
using System.Windows;
using Cvte.Paint.Framework;

namespace Cvte.Paint.Inks
{
	public class InkErasingCompleteRoutedEventArgs : InkErasingRoutedEventArgs
	{
		public IReadOnlyList<Element> DeleteInks { get; }

		public IReadOnlyList<Element> NewInks { get; }

		public InkErasingCompleteRoutedEventArgs(RoutedEvent routedEvent, Slide attachedSlide, EraserHandler currentEraser, IReadOnlyList<Element> deleteInks, IReadOnlyList<Element> newInks)
			: base(routedEvent, attachedSlide, currentEraser)
		{
			DeleteInks = deleteInks;
			NewInks = newInks;
		}

		public InkErasingCompleteRoutedEventArgs(RoutedEvent routedEvent, object source, Slide attachedSlide, EraserHandler currentEraser, IReadOnlyList<Element> deleteInks, IReadOnlyList<Element> newInks)
			: base(routedEvent, source, attachedSlide, currentEraser)
		{
			DeleteInks = deleteInks;
			NewInks = newInks;
		}
	}
}
