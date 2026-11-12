using System.Windows;

namespace Cvte.Paint.Framework.ElementLockedActions
{
	public class ElementIsLockedChangedRoutedEventArgs : RoutedEventArgs
	{
		public Element Element { get; }

		public Slide Slide { get; }

		public ValueChangeEventArgs<bool> IsLockedArgs { get; }

		public ElementIsLockedChangedRoutedEventArgs(RoutedEvent routedEvent, Slide slide, Element element, ValueChangeEventArgs<bool> isLockedArgs)
			: base(routedEvent, slide)
		{
			RoutedEvent = routedEvent;
			Slide = slide;
			Element = element;
			IsLockedArgs = isLockedArgs;
		}
	}
}
