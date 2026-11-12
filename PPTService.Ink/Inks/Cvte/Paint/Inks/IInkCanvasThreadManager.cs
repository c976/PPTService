using System;

namespace Cvte.Paint.Inks
{
	public interface IInkCanvasThreadManager
	{
		void RunInTouchDispatcher(Action action);

		void RunInMainDispatcher(Action action);
	}
}
