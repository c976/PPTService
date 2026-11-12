using System;
using Cvte.Paint.Framework;

namespace Cvte.Paint.Inks
{
	public class SelectEraseMode : CompositeMode, IEraserMode
	{
		private readonly SelectionEraseModeOperation _erasingMode;

		public Predicate<Element> ErasingFilter
		{
			get
			{
				return _erasingMode.ErasingFilter;
			}
			set
			{
				_erasingMode.ErasingFilter = value;
			}
		}

		public SelectEraseMode()
		{
			_erasingMode = new SelectionEraseModeOperation
			{
				ErasingFilter = OnlyInkCanBeErased
			};
			SelectionEraseModeOperation erasingMode = _erasingMode;
			erasingMode.DragCancelEvent = (EventHandler<SelectEventArgs>)Delegate.Combine(erasingMode.DragCancelEvent, new EventHandler<SelectEventArgs>(PointEraseEventHelper.OnPointEraseCanceled));
			SelectionEraseModeOperation erasingMode2 = _erasingMode;
			erasingMode2.DragStartedEvent = (EventHandler<SelectEventArgs>)Delegate.Combine(erasingMode2.DragStartedEvent, new EventHandler<SelectEventArgs>(PointEraseEventHelper.OnPointEraseStarted));
			SelectionEraseModeOperation erasingMode3 = _erasingMode;
			erasingMode3.DragDeltaEvent = (EventHandler<SelectEventArgs>)Delegate.Combine(erasingMode3.DragDeltaEvent, new EventHandler<SelectEventArgs>(PointEraseEventHelper.OnPointErasing));
			SelectionEraseModeOperation erasingMode4 = _erasingMode;
			erasingMode4.DragCompletedEvent = (EventHandler<SelectEventArgs>)Delegate.Combine(erasingMode4.DragCompletedEvent, new EventHandler<SelectEventArgs>(PointEraseEventHelper.OnPointEraseCompleted));
			ModeOperations.Add(_erasingMode);
			ModeOperations.Add(new SelectionEraseModeEventOperation());
		}

		private static bool OnlyInkCanBeErased(Element element)
		{
			return element is Ink;
		}
	}
}
