using System.Collections.Generic;

namespace Cvte.Paint.Framework.Operations
{
	public class AddElementsOperation : ElementsAddOrRemoveOperation
	{
		public AddElementsOperation(Slide slide, IEnumerable<Element> elements)
			: base(slide, elements)
		{
		}

		public AddElementsOperation(Slide slide, Element element)
			: this(slide, new Element[1] { element })
		{
		}

		protected override void OnUndo()
		{
			RemoveElements();
		}

		protected override void OnRedo()
		{
			AddElements();
		}
	}
}
