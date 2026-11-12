using System;
using System.Collections.Generic;
using System.Linq;

namespace Cvte.Paint.Framework.Operations
{
	public class ElementDeltaBuilder
	{
		private readonly Element[] _elements;

		private readonly ElementSaveInfo[] _befores;

		public IEnumerable<Element> Elements => _elements;

		public ElementDeltaBuilder(IEnumerable<Element> elements)
		{
			if (elements == null)
			{
				throw new ArgumentNullException("elements");
			}
			_elements = elements.ToArray();
			if (!_elements.Any())
			{
				throw new ArgumentException("elements 为空。");
			}
			if (_elements.Any((Element x) => x == null))
			{
				throw new ArgumentException("elements 包含 null。");
			}
			_befores = _elements.Select((Element x) => x.GetElementSaveInfo()).ToArray();
		}

		public ElementDeltaBuilder(Element element)
			: this(new Element[1] { element })
		{
		}

		public ElementDelta[] ToElementDeltaArray()
		{
			ElementDelta[] array = new ElementDelta[_elements.Length];
			for (int i = 0; i < _elements.Length; i++)
			{
				Element element = _elements[i];
				ElementSaveInfo before = _befores[i];
				ElementSaveInfo elementSaveInfo = element.GetElementSaveInfo();
				array[i] = new ElementDelta(element, before, elementSaveInfo);
			}
			return array;
		}

		public ElementDelta ToElementDelta()
		{
			if (_elements.Length > 1)
			{
				throw new InvalidOperationException("当前的 ElementDeltaBuilder 实例包含多个 Element 的信息，请调用 ToElementDeltaArray 方法以获取所有的信息。");
			}
			Element element = _elements[0];
			ElementSaveInfo before = _befores[0];
			ElementSaveInfo elementSaveInfo = element.GetElementSaveInfo();
			return new ElementDelta(element, before, elementSaveInfo);
		}
	}
}
