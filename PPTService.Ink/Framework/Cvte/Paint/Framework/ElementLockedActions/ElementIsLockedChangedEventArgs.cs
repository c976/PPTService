using System;

namespace Cvte.Paint.Framework.ElementLockedActions
{
	public class ElementIsLockedChangedEventArgs : EventArgs
	{
		public Element Element { get; }

		public Slide Slide { get; }

		public ValueChangeEventArgs<bool> IsLockedArgs { get; }

		public ElementIsLockedChangedEventArgs(Slide slide, Element element, ValueChangeEventArgs<bool> isLockedArgs)
		{
			Slide = slide;
			Element = element;
			IsLockedArgs = isLockedArgs;
		}

		public static ElementIsLockedChangedEventArgs FromRoutedEventArgs(ElementIsLockedChangedRoutedEventArgs e)
		{
			return new ElementIsLockedChangedEventArgs(e.Slide, e.Element, e.IsLockedArgs);
		}
	}
}
