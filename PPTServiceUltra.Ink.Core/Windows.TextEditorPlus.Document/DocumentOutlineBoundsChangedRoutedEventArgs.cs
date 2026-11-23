using System;
using System.Windows;
using PPTServiceUltra.Ink.Core.Windows.Controls;

namespace PPTServiceUltra.Ink.Core.Windows.TextEditorPlus.Document
{
	public class DocumentOutlineBoundsChangedRoutedEventArgs : ValueChangedRoutedEventArgs<Rect>
	{
		public DocumentOutlineBoundsChangedRoutedEventArgs(RoutedEvent routedEvent, Rect oldValue, Rect newValue)
			: base(routedEvent, oldValue, newValue)
		{
		}

		public DocumentOutlineBoundsChangedRoutedEventArgs(RoutedEvent routedEvent, Rect oldValue, Rect newValue, object source)
			: base(routedEvent, oldValue, newValue, source)
		{
		}

		protected override void InvokeEventHandler(Delegate genericHandler, object genericTarget)
		{
			((DocumentOutlineBoundsChangedRoutedEventHandler)genericHandler)(genericTarget, this);
		}
	}
}
