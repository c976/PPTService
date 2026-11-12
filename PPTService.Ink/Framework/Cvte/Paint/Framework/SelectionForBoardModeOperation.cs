using System.Collections.Generic;

namespace Cvte.Paint.Framework
{
	public sealed class SelectionForBoardModeOperation : SelectionForBoardModeOperationBase
	{
		protected override void ClearSelection()
		{
			CurrentSlide.ClearAndSelect();
		}

		protected override void TapSelect(Element element)
		{
			CurrentSlide.ClearAndSelect();
		}

		protected override void DragSelect(ICollection<Element> elements)
		{
			CurrentSlide.Select(elements);
		}
	}
}
