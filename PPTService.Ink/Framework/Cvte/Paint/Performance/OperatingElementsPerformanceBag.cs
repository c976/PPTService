using System;
using System.Collections.Generic;
using System.Linq;
using Cvte.Diagnostics.Annotations;
using Cvte.Paint.Framework;
using Cvte.Paint.Framework.Elements;

namespace Cvte.Paint.Performance
{
	public class OperatingElementsPerformanceBag
	{
		private readonly Slide _slide;

		private readonly IReadOnlyCollection<Element> _operatingElements;

		public OperatingElementsPerformanceBag([NotNull] Slide slide, IReadOnlyList<Element> operatingElements)
		{
			if (slide == null)
			{
				throw new ArgumentNullException("slide");
			}
			_slide = slide;
			if (operatingElements == null)
			{
				throw new ArgumentNullException("operatingElements");
			}
			_operatingElements = operatingElements;
		}

		public OperatingElementsPerformanceBag([NotNull] Slide slide, IEnumerable<Element> operatingElements)
		{
			if (slide == null)
			{
				throw new ArgumentNullException("slide");
			}
			_slide = slide;
			IReadOnlyCollection<Element> readOnlyCollection = (IReadOnlyCollection<Element>)operatingElements;
			if (readOnlyCollection == null)
			{
				throw new ArgumentNullException("operatingElements");
			}
			_operatingElements = readOnlyCollection;
		}

		public override string ToString()
		{
			return "dynamic: " + FormatElementInfo(_operatingElements) + "\r\nstatic: " + FormatElementInfo(_slide.Elements.Except(_operatingElements));
		}

		private static string FormatElementInfo(Element element)
		{
			IGroupElement groupElement = element as IGroupElement;
			if (groupElement == null)
			{
				return element.GetType().Name;
			}
			return "Group[" + string.Join("+", FormatElementInfo(groupElement.Elements)) + "]";
		}

		private static string FormatElementInfo(IEnumerable<Element> elements)
		{
			return string.Join("+", elements.Select(FormatElementInfo));
		}

		public static implicit operator Func<string>(OperatingElementsPerformanceBag bag)
		{
			return bag.ToString;
		}
	}
}
