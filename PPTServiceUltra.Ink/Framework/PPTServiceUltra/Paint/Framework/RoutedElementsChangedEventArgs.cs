using System.Collections.Generic;
using System.Windows;
using PPTServiceUltra.Paint.Framework.Operations;

namespace PPTServiceUltra.Paint.Framework
{
	public class RoutedElementsChangedEventArgs : RoutedCollectionChangedEventArgs<ElementDelta>
	{
		public bool IsFinalState { get; private set; }

		public RoutedElementsChangedEventArgs(RoutedEvent routedEvent, IList<ElementDelta> items, bool isFinalState = true)
			: base(routedEvent, items)
		{
			IsFinalState = isFinalState;
		}
	}
}
