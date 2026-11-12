using System.Collections.Generic;

namespace Cvte.Paint.Framework
{
	public interface IElementsService
	{
		void AddElement(Element element);

		void AddElements(IEnumerable<Element> elements);

		void RemoveElement(Element element);

		void RemoveElements(IEnumerable<Element> elements);

		bool ContainsElement(Element element);

		void SelectElements(IEnumerable<Element> elements);

		Element GetElementById(string id);

		void Unselect();
	}
}
