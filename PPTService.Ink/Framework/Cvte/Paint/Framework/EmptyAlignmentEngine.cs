using System.Collections.Generic;
using System.Windows;

namespace Cvte.Paint.Framework
{
	public class EmptyAlignmentEngine : IAlignmentEngine
	{
		private static readonly Rect UselessBounds = new Rect(0.0, 0.0, 0.0, 0.0);

		private static readonly AlignResult UselessResult = new AlignResult(new SingleAlignResult(), new SingleAlignResult());

		AlignResult IAlignmentEngine.Align(Rect bounds, Point point)
		{
			return UselessResult;
		}

		AlignResult IAlignmentEngine.Align(Rect bounds)
		{
			return UselessResult;
		}

		AlignResult IAlignmentEngine.Align(params Rect[] boundsArray)
		{
			return UselessResult;
		}

		void IAlignmentEngine.Clear()
		{
		}

		void IAlignmentEngine.Draw(Rect bounds)
		{
		}

		void IAlignmentEngine.Draw(params Rect[] boundsArray)
		{
		}

		void IAlignmentEngine.Draw(Rect bounds, Point point, bool mergeBoundsEdgeOverPoint)
		{
		}

		void IAlignmentEngine.DrawAlignResult(AlignResult result)
		{
		}

		Rect IAlignmentEngine.GetAlignmentBounds(IEnumerable<Element> elements)
		{
			return UselessBounds;
		}

		void IAlignmentEngine.Prepare(Slide slide, IEnumerable<Element> staticElements, IEnumerable<Element> operatingElements)
		{
		}
	}
}
