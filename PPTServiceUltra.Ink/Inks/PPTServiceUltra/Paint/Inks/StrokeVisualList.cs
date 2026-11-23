using System.Collections.Generic;
using System.Windows.Media;
using PPTServiceUltra.Ink.Core.Diagnostics.Annotations;
using PPTServiceUltra.Paint.Inks.Visual_;

namespace PPTServiceUltra.Paint.Inks
{
	public class StrokeVisualList : LinkedList<PPTServiceUltra.Paint.Inks.Visual_.StrokeVisual>
	{
		[CanBeNull]
		public DrawingVisual StrokeTipVisual { get; set; }

		public List<DrawingVisual> GetDryingDrawVisual()
		{
			bool flag = StrokeTipVisual != null;
			int num = (flag ? 1 : 0);
			List<DrawingVisual> list = new List<DrawingVisual>(Count + num);
			using (Enumerator enumerator = GetEnumerator())
			{
				while (enumerator.MoveNext())
				{
					PPTServiceUltra.Paint.Inks.Visual_.StrokeVisual current = enumerator.Current;
					list.Add(current);
				}
			}
			if (flag)
			{
				list.Add(StrokeTipVisual);
			}
			return list;
		}
	}
}
