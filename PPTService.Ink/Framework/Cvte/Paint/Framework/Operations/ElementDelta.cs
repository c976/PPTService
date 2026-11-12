namespace Cvte.Paint.Framework.Operations
{
	public class ElementDelta
	{
		public Element Element { get; private set; }

		public ElementSaveInfo Before { get; private set; }

		public ElementSaveInfo After { get; private set; }

		public ElementDelta(Element element, ElementSaveInfo before, ElementSaveInfo after)
		{
			After = after;
			Before = before;
			Element = element;
		}
	}
}
