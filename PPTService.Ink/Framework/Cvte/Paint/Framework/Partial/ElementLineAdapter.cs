using System;

namespace Cvte.Paint.Framework.Partial
{
	public abstract class ElementLineAdapter : IElementPartialAdapter<Element>, IElementPartialAdapter
	{
		public abstract Type ElementType { get; }

		public abstract ElementPartialData GetPartCollection(Element element);
	}
}
