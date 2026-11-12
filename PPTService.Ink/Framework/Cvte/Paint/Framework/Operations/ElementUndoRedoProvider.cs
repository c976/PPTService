using Cvte.Documents;

namespace Cvte.Paint.Framework.Operations
{
	public class ElementUndoRedoProvider : UndoRedoProviderDecorator
	{
		public Element TheElement { get; }

		public ElementUndoRedoProvider(Element element, IUndoRedoProvider undoRedoProvider)
			: base(undoRedoProvider)
		{
			TheElement = element;
		}
	}
	public class ElementUndoRedoProvider<T> : UndoRedoProviderDecorator where T : Element
	{
		public T TheElement { get; }

		public ElementUndoRedoProvider(T element, IUndoRedoProvider undoRedoProvider)
			: base(undoRedoProvider)
		{
			TheElement = element;
		}
	}
}
