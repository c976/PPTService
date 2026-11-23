using System.Collections.Generic;
using System.Linq;
using System.Windows;

namespace PPTServiceUltra.Paint.Framework
{
	public class RoutedCollectionChangedEventArgs<T> : RoutedEventArgs
	{
		public IList<T> Items { get; }

		public RoutedCollectionChangedEventArgs(RoutedEvent routedEvent, IList<T> items)
			: base(routedEvent)
		{
			Items = items.ToArray();
		}
	}
}
