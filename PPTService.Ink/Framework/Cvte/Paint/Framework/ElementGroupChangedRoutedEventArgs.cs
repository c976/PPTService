using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows;
using Cvte.Diagnostics.Annotations;
using Cvte.Paint.Framework.Elements;

namespace Cvte.Paint.Framework
{
	public class ElementGroupChangedRoutedEventArgs : RoutedEventArgs
	{
		public IGroupElement Group { get; }

		public Element[] Elements { get; }

		public ElementGroupChangedRoutedEventArgs([NotNull] RoutedEvent routedEvent, [NotNull] IGroupElement group, [NotNull] IEnumerable<Element> elements)
		{
			if (routedEvent == null)
			{
				throw new ArgumentNullException("routedEvent");
			}
			if (group == null)
			{
				throw new ArgumentNullException("group");
			}
			if (elements == null)
			{
				throw new ArgumentNullException("elements");
			}
			if (routedEvent != GroupEvents.GroupedEvent && routedEvent != GroupEvents.UngroupedEvent && routedEvent != GroupEvents.GroupingEvent && routedEvent != GroupEvents.UngroupingEvent)
			{
				throw new ArgumentException();
			}
			RoutedEvent = routedEvent;
			Group = group;
			Elements = (elements as Element[]) ?? elements.ToArray();
		}

		protected override void InvokeEventHandler(Delegate genericHandler, object genericTarget)
		{
			((ElementGroupChangedRoutedEventHandler)genericHandler)(genericTarget, this);
		}
	}
}
