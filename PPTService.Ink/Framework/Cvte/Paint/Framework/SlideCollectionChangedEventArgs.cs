using System.Collections;
using System.Collections.Generic;
using System.Collections.Specialized;

namespace Cvte.Paint.Framework
{
	public class SlideCollectionChangedEventArgs : NotifyCollectionChangedEventArgs
	{
		public IList<int> ChangeIndex { get; internal set; }

		public SlideCollectionChangedEventArgs(NotifyCollectionChangedAction action)
			: base(action)
		{
		}

		public SlideCollectionChangedEventArgs(NotifyCollectionChangedAction action, object changedItem)
			: base(action, changedItem)
		{
		}

		public SlideCollectionChangedEventArgs(NotifyCollectionChangedAction action, object changedItem, int index)
			: base(action, changedItem, index)
		{
		}

		public SlideCollectionChangedEventArgs(NotifyCollectionChangedAction action, IList changedItems)
			: base(action, changedItems)
		{
		}

		public SlideCollectionChangedEventArgs(NotifyCollectionChangedAction action, IList changedItems, int startingIndex)
			: base(action, changedItems, startingIndex)
		{
		}

		public SlideCollectionChangedEventArgs(NotifyCollectionChangedAction action, object newItem, object oldItem)
			: base(action, newItem, oldItem)
		{
		}

		public SlideCollectionChangedEventArgs(NotifyCollectionChangedAction action, object newItem, object oldItem, int index)
			: base(action, newItem, oldItem, index)
		{
		}

		public SlideCollectionChangedEventArgs(NotifyCollectionChangedAction action, IList newItems, IList oldItems)
			: base(action, newItems, oldItems)
		{
		}

		public SlideCollectionChangedEventArgs(NotifyCollectionChangedAction action, IList newItems, IList oldItems, int startingIndex)
			: base(action, newItems, oldItems, startingIndex)
		{
		}

		public SlideCollectionChangedEventArgs(NotifyCollectionChangedAction action, object changedItem, int index, int oldIndex)
			: base(action, changedItem, index, oldIndex)
		{
			ChangeIndex = (IList<int>)changedItem;
		}

		public SlideCollectionChangedEventArgs(NotifyCollectionChangedAction action, IList changedItems, int index, int oldIndex)
			: base(action, changedItems, index, oldIndex)
		{
		}
	}
}
