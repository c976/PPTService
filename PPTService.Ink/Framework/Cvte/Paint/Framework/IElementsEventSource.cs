using System;

namespace Cvte.Paint.Framework
{
	public interface IElementsEventSource
	{
		event EventHandler<ElementsEventArgs> ElementsAdded;

		event EventHandler<ElementsEventArgs> ElementsRemoved;

		event EventHandler<ElementsEventArgs> SelectionChanged;

		event EventHandler<ElementInnerSelectionChangedEventArgs> ElementInnerSelectionChanged;

		event EventHandler<ElementsChangedEventArgs> ElementsChanged;

		event ElementGroupChangedEventHandler ElementGrouping;

		event ElementGroupChangedEventHandler ElementGrouped;

		event ElementGroupChangedEventHandler ElementUngrouping;

		event ElementGroupChangedEventHandler ElementUngrouped;
	}
}
