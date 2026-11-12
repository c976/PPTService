using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows;
using Cvte.Diagnostics.Annotations;

namespace Cvte.Paint.Framework.Partial
{
	public class ElementPartialData
	{
		public LayoutDirection Direction { get; }

		public IReadOnlyList<Rect> Rect { get; }

		public ElementPartialData(LayoutDirection direction, [NotNull] IEnumerable<Rect> rects)
		{
			if (rects == null)
			{
				throw new ArgumentNullException("rects");
			}
			Direction = direction;
			IReadOnlyList<Rect> readOnlyList = rects as IReadOnlyList<Rect>;
			IReadOnlyList<Rect> readOnlyList3;
			if (readOnlyList == null)
			{
				IReadOnlyList<Rect> readOnlyList2 = rects.ToList();
				readOnlyList3 = readOnlyList2;
			}
			else
			{
				readOnlyList3 = readOnlyList;
			}
			Rect = readOnlyList3;
		}
	}
}
