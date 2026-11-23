using System;

namespace PPTServiceUltra.Paint.Inks
{
	public interface IInkCanvasThreadManager
	{
		void RunInTouchDispatcher(Action action);

		void RunInMainDispatcher(Action action);
	}
}
