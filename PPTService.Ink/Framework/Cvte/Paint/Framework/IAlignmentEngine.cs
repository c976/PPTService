using System.Collections.Generic;
using System.Windows;

namespace Cvte.Paint.Framework
{
	public interface IAlignmentEngine
	{
		void Clear();

		Rect GetAlignmentBounds(IEnumerable<Element> elements);

		void Prepare(Slide slide, IEnumerable<Element> staticElements, IEnumerable<Element> operatingElements);

		AlignResult Align(Rect bounds);

		AlignResult Align(params Rect[] boundsArray);

		AlignResult Align(Rect bounds, Point point);

		void Draw(Rect bounds);

		void Draw(params Rect[] boundsArray);

		void Draw(Rect bounds, Point point, bool mergeBoundsEdgeOverPoint = false);

		void DrawAlignResult(AlignResult result);
	}
}
