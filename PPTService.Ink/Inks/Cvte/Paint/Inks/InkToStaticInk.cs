using System;
using System.Threading.Tasks;
using System.Windows.Ink;
using Cvte.Documents;
using Cvte.Paint.Framework;
using Cvte.Paint.Inks.Highlighter;

namespace Cvte.Paint.Inks
{
	internal class InkToStaticInk
	{
		private DateTime _createTime;

		private readonly Element _ink;

		private readonly Slide _slide;

		private bool IsInserted { get; set; }

		private bool NeedRest { get; set; }

		private TimeSpan MinDrawingTimeInMilliseconds { get; set; } = TimeSpan.FromMilliseconds(300.0);

		public InkToStaticInk(Element ink, Slide slide)
		{
			_ink = ink;
			_slide = slide;
		}

		public void AddToSlide()
		{
			_createTime = DateTime.Now;
			_slide.Add(_ink);
			InsertAddingOperationDelay();
		}

		public void Rest()
		{
			if (!IsInserted && DateTime.Now - _createTime < MinDrawingTimeInMilliseconds)
			{
				NeedRest = true;
				Stroke stroke = null;
				Ink ink = _ink as Ink;
				if (ink != null)
				{
					stroke = ink.InnerStroke;
				}
				else
				{
					bool flag = _ink is HighlighterInk;
				}
				if (stroke != null && stroke.StylusPoints.Count >= 50)
				{
					NeedRest = false;
				}
				if (NeedRest && _slide.Contains(_ink))
				{
					_slide.Remove(_ink);
				}
			}
		}

		private async void InsertAddingOperationDelay()
		{
			await Task.Delay(MinDrawingTimeInMilliseconds);
			Slide slide = _slide;
			IUndoRedoProvider undoRedoProvider = Board.GetUndoRedoProvider(slide);
			IsInserted = true;
			if (!NeedRest)
			{
				undoRedoProvider?.Insert(new AddInkElementOperation(slide, _ink));
			}
		}
	}
}
