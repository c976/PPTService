using System;
using System.Collections.Generic;
using System.Linq;
using Cvte.Diagnostics.Annotations;
using Cvte.Paint.Framework.Elements;

namespace Cvte.Paint.Framework
{
	public class ElementGroupChangedEventArgs : EventArgs
	{
		public IGroupElement Group { get; private set; }

		public Element[] Elements { get; private set; }

		public ElementGroupChangedEventArgs([NotNull] IGroupElement group, [NotNull] IEnumerable<Element> elements)
		{
			if (group == null)
			{
				throw new ArgumentNullException("group");
			}
			if (elements == null)
			{
				throw new ArgumentNullException("elements");
			}
			Group = group;
			Elements = (elements as Element[]) ?? elements.ToArray();
		}

		public static ElementGroupChangedEventArgs FromRoutedEventArgs(ElementGroupChangedRoutedEventArgs e)
		{
			return new ElementGroupChangedEventArgs(e.Group, e.Elements);
		}
	}
}
