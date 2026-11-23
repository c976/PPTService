using PPTServiceUltra.Ink.Core.Documents;

namespace PPTServiceUltra.Paint.Framework.Operations
{
	public class SlideUndoRedoProvider : UndoRedoProviderDecorator
	{
		public Slide TheSlide { get; }

		public SlideUndoRedoProvider(Slide slide, IUndoRedoProvider undoRedoProvider)
			: base(undoRedoProvider)
		{
			TheSlide = slide;
		}
	}
}
