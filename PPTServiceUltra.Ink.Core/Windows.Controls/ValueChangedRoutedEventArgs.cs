using System;
using System.Windows;

namespace PPTServiceUltra.Ink.Core.Windows.Controls
{
	public class ValueChangedRoutedEventArgs<T> : RoutedEventArgs
	{
		public T OldValue { get; private set; }

		public T NewValue { get; private set; }

		public ValueChangedRoutedEventArgs(RoutedEvent routedEvent, T oldValue, T newValue)
			: base(routedEvent)
		{
			OldValue = oldValue;
			NewValue = newValue;
		}

		public ValueChangedRoutedEventArgs(RoutedEvent routedEvent, T oldValue, T newValue, object source)
			: base(routedEvent, source)
		{
			OldValue = oldValue;
			NewValue = newValue;
		}

		protected override void InvokeEventHandler(Delegate genericHandler, object genericTarget)
		{
			((ValueChangedRoutedEventHandler<T>)genericHandler)(genericTarget, this);
		}
	}
}
