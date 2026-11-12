using System.Collections.ObjectModel;
using System.Linq;
using System.Windows.Controls;

namespace Cvte.Paint.Framework
{
	internal class ElementCollection : Collection<Element>
	{
		private readonly UIElementCollection _innerCollection;

		public ElementCollection(UIElementCollection innerCollection)
		{
			_innerCollection = innerCollection;
		}

		protected override void InsertItem(int index, Element item)
		{
			base.InsertItem(index, item);
			_innerCollection.Insert(index, item);
		}

		protected override void RemoveItem(int index)
		{
			base.RemoveItem(index);
			_innerCollection.RemoveAt(index);
		}

		internal bool ContainsId(string id)
		{
			return _innerCollection.Cast<Element>().Any((Element element) => id.Equals(element.Id));
		}

		internal bool ContainsById(Element element)
		{
			string id = element.Id;
			return ContainsId(id);
		}
	}
}
