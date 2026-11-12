using System;
using System.Collections.Generic;
using System.Linq;
using Cvte.Paint.Framework;

namespace Cvte.Paint.Performance
{
	public static class PerformanceCollectorExtensions
	{
		public static IContinuousProgressPerformanceCollector StartElementOperations(this IPerformanceCollector collector, string tag, string action, IEnumerable<Element> operatingElements, Slide slide = null)
		{
			Slide slide2 = slide;
			if (slide2 == null)
			{
				slide2 = Slide.GetSlide(operatingElements.First());
				if (slide2 == null)
				{
					throw new ArgumentException("操作的元素不在任何 Slide 上");
				}
			}
			slide = slide2;
			IContinuousProgressPerformanceCollector continuousProgressPerformanceCollector = collector.Start(tag, action, new OperatingElementsPerformanceBag(slide, operatingElements));
			continuousProgressPerformanceCollector.CollectUIDelay();
			return continuousProgressPerformanceCollector;
		}
	}
}
