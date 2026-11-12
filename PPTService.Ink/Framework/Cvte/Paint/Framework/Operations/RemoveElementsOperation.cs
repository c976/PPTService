using System.Collections.Generic;

namespace Cvte.Paint.Framework.Operations
{
	public class RemoveElementsOperation : ElementsAddOrRemoveOperation
	{
		public RemoveElementsOperation(Slide slide, IEnumerable<Element> elements)
			: base(slide, elements)
		{
		}

		public RemoveElementsOperation(Slide slide, Element element)
			: this(slide, new Element[1] { element })
		{
		}

		protected override void OnRedo()
		{
			RemoveElements();
		}

		protected override void OnUndo()
		{
			AddElements();
		}
	}
}
