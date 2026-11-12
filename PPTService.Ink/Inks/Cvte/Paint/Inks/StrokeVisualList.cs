using System.Collections.Generic;
using System.Windows.Media;
using Cvte.Diagnostics.Annotations;
using Cvte.Paint.Inks.Visual_;

namespace Cvte.Paint.Inks
{
	public class StrokeVisualList : LinkedList<Cvte.Paint.Inks.Visual_.StrokeVisual>
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
					Cvte.Paint.Inks.Visual_.StrokeVisual current = enumerator.Current;
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
