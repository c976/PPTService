using System;
using System.Collections.Generic;

namespace Cvte.Paint.Framework
{
	public class ElementInnerSelectionChangedEventArgs : EventArgs
	{
		public object Root { get; }

		public object Parent { get; }

		public IList<object> Items { get; }

		public ElementInnerSelectionChangedEventArgs(object root, object parent, IList<object> items)
		{
			Root = root;
			Parent = parent;
			Items = items;
		}
	}
}
