using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using Cvte.Diagnostics.Annotations;

namespace Cvte.Paint.Framework
{
	public class ElementsEventArgs : EventArgs
	{
		[NotNull]
		public IReadOnlyCollection<Element> Elements { get; }

		[NotNull]
		public Slide Slide { get; }

		public ElementsEventArgs([NotNull] Element element, [NotNull] Slide slide)
		{
			if (element == null)
			{
				throw new ArgumentNullException("element");
			}
			Elements = new Element[1] { element };
			if (slide == null)
			{
				throw new ArgumentNullException("slide");
			}
			Slide = slide;
		}

		public ElementsEventArgs([NotNull][ItemNotNull] IEnumerable<Element> elements, [NotNull] Slide slide)
		{
			if (elements == null)
			{
				throw new ArgumentNullException("elements");
			}
			Elements = new ReadOnlyCollection<Element>(elements.ToList());
			if (slide == null)
			{
				throw new ArgumentNullException("slide");
			}
			Slide = slide;
		}
	}
}
