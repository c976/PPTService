using System.Collections.Generic;
using System.Linq;

namespace Cvte.Paint.Framework.Operations
{
	public abstract class ElementsOperation : ElementOperationBase
	{
		public IReadOnlyList<Element> Elements { get; }

		protected ElementsOperation(Slide slide, IEnumerable<Element> elements)
			: base(slide.Id)
		{
			SlideId = slide.Id;
			Elements = elements.ToArray();
		}

		protected ElementsOperation(Slide slide, Element element)
			: base(slide.Id)
		{
			SlideId = slide.Id;
			Elements = new Element[1] { element };
		}
	}
}
