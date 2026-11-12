using System;
using System.Collections.Generic;
using System.Linq;
using Cvte.Paint.Framework;
using Cvte.Paint.Framework.Operations;

namespace Cvte.Paint.Inks
{
	public sealed class SynchronousSelectionEraseModeOperation : SelectionForBoardModeOperationBase
	{
		protected override void ClearSelection()
		{
		}

		protected override void TapSelect(Element element)
		{
			Ink ink = element as Ink;
			if (ink != null)
			{
				PointEraseEventHelper.OnOperationSynchronous(null, CreateSynchronousEventArgs(new List<Ink> { ink }));
			}
		}

		protected override void DragSelect(ICollection<Element> elements)
		{
			List<Ink> list = elements.OfType<Ink>().ToList();
			if (list.Any())
			{
				PointEraseEventHelper.OnOperationSynchronous(null, CreateSynchronousEventArgs(list));
			}
		}

		private SelectEraseOperationSynchronousEventArgs CreateSynchronousEventArgs(List<Ink> inks)
		{
			return new SelectEraseOperationSynchronousEventArgs
			{
				DeletedInkIdList = inks.Select((Ink x) => x.Id).ToList(),
				LocalOperation = (object obj) =>
				{
					CurrentSlide.Dispatcher.BeginInvoke((Action)(() =>
					{
						List<string> deleteElementIDList = obj as List<string>;
						if (deleteElementIDList != null)
						{
							List<Ink> list = inks.Where((Ink x) => deleteElementIDList.Contains(x.Id)).ToList();
							if (list.Any())
							{
								CurrentSlide.Remove(list);
								Board.GetUndoRedoProvider(CurrentSlide).Insert(new RemoveElementsOperation(CurrentSlide, list));
							}
						}
					}));
				}
			};
		}
	}
}
