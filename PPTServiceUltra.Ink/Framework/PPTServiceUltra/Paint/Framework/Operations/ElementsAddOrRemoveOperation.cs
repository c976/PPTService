using System.Collections.Generic;
using PPTServiceUltra.Paint.Framework.Exceptions;
using PPTServiceUltra.Paint.Framework.Settings;

namespace PPTServiceUltra.Paint.Framework.Operations
{
	public abstract class ElementsAddOrRemoveOperation : ElementsOperation
	{
		protected ElementsAddOrRemoveOperation(Slide slide, IEnumerable<Element> elements)
			: base(slide, elements)
		{
		}

		protected ElementsAddOrRemoveOperation(Slide slide, Element element)
			: base(slide, element)
		{
		}

		protected void AddElements()
		{
			GetSlide().Add(Elements);
		}

		protected void RemoveElements()
		{
			Slide slide = GetSlide();
			slide.TryUnselect();
			try
			{
				slide.Remove(Elements);
			}
			catch (ElementNotFoundException exception)
			{
				slide.GetSettings<TraceSettings>().Tracer?.Trace(exception, "UndoRedo", "ElementsAddOrRemoveOperation");
			}
		}
	}
}
