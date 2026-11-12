using System;
using System.Collections.Generic;
using System.Linq;
using Cvte.Diagnostics.Annotations;
using Cvte.Paint.Framework;
using Cvte.Paint.Framework.Operations;

namespace Cvte.Paint.Inks
{
	public sealed class SelectionEraseModeOperation : SelectionForBoardModeOperationBase
	{
		[CanBeNull]
		public Predicate<Element> ErasingFilter { get; set; }

		protected override void ClearSelection()
		{
		}

		protected override void TapSelect(Element element)
		{
			if (CanErase(element))
			{
				Board.GetUndoRedoProvider(CurrentSlide).Insert(new RemoveElementsOperation(CurrentSlide, element));
				CurrentSlide.Remove(element);
			}
		}

		protected override void DragSelect(ICollection<Element> elements)
		{
			elements = elements.Where(CanErase).ToArray();
			if (elements.Any())
			{
				Board.GetUndoRedoProvider(CurrentSlide).Insert(new RemoveElementsOperation(CurrentSlide, elements));
				CurrentSlide.Remove(elements);
			}
		}

		private bool CanErase(Element element)
		{
			return ErasingFilter?.Invoke(element) ?? true;
		}
	}
}
