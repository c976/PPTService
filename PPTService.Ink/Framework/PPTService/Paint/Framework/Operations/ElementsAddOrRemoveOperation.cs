using System.Collections.Generic;
using PPTService.Paint.Framework.Exceptions;
using PPTService.Paint.Framework.Settings;

namespace PPTService.Paint.Framework.Operations
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
