using System.Collections.Generic;
using System.Windows;
using PPTServiceUltra.Ink.Core.Diagnostics.Annotations;

namespace PPTServiceUltra.Paint.Framework
{
	public class RoutedInnerCollectionChangedEventArgs<T> : RoutedCollectionChangedEventArgs<T>
	{
		public object Root { get; }

		public object Parent { get; }

		public RoutedInnerCollectionChangedEventArgs([NotNull] RoutedEvent routedEvent, object root, object parent, [NotNull] IList<T> items)
			: base(routedEvent, items)
		{
			Root = root;
			Parent = parent;
		}
	}
}
