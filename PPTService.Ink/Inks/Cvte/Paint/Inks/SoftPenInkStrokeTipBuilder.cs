using System;
using System.Windows.Input;

namespace Cvte.Paint.Inks
{
	public class SoftPenInkStrokeTipBuilder : DynamicRenderer.InkStrokeTipBuilder
	{
		private StylusPointQueue _strokeTipQueue;

		private StylusPointCollection _stylusPointCollection;

		public override StylusPointCollection StrokeTipCollection
		{
			get
			{
				if (_strokeTipQueue == null || _strokeTipQueue.Count == 0)
				{
					return null;
				}
				if (_stylusPointCollection == null)
				{
					_stylusPointCollection = new StylusPointCollection(_strokeTipQueue[0].Description, 18);
				}
				StylusPointCollection stylusPointCollection = _stylusPointCollection;
				_strokeTipQueue.CopyToStylusPointCollection(stylusPointCollection);
				return stylusPointCollection;
			}
		}

		public override StylusPointCollection AddPoint(StylusPoint stylusPoint)
		{
			if (_strokeTipQueue == null)
			{
				_strokeTipQueue = new StylusPointQueue(18);
			}
			stylusPoint.PressureFactor /= Ratio(9);
			StylusPointQueue strokeTipQueue = _strokeTipQueue;
			for (int i = 1; i <= 8 && strokeTipQueue.Count - i >= 1; i++)
			{
				StylusPoint value = strokeTipQueue[strokeTipQueue.Count - i];
				float val = value.PressureFactor / Ratio(9 - i) * Ratio(10 - i);
				value.PressureFactor = Math.Min(val, 1f);
				strokeTipQueue[strokeTipQueue.Count - i] = value;
			}
			StylusPointCollection stylusPointCollection = null;
			if (strokeTipQueue.Count == 16)
			{
				stylusPointCollection = new StylusPointCollection(stylusPoint.Description);
				StylusPoint item = strokeTipQueue.Dequeue();
				StylusPoint item2 = strokeTipQueue.Dequeue();
				if (item.PressureFactor < item2.PressureFactor)
				{
					item.PressureFactor = item2.PressureFactor;
				}
				stylusPointCollection.Add(item);
				stylusPointCollection.Add(item2);
				for (int j = 2; j < 8; j++)
				{
					stylusPointCollection.Add(strokeTipQueue.Dequeue());
				}
				StylusPoint item3 = strokeTipQueue[0];
				item3.PressureFactor = stylusPointCollection[stylusPointCollection.Count - 1].PressureFactor;
				stylusPointCollection.Add(item3);
			}
			strokeTipQueue.Enqueue(stylusPoint);
			return stylusPointCollection;
		}

		public override void Redraw()
		{
			if (_strokeTipQueue != null && _strokeTipQueue.Count != 0)
			{
				base.Redraw();
			}
		}

		private float Ratio(int level)
		{
			switch (level)
			{
			case 1:
				return 1f;
			case 2:
				return 1.25f;
			case 3:
				return 1.5f;
			case 4:
				return 1.8f;
			case 5:
				return 2.3f;
			case 6:
				return 2.9f;
			case 9:
				return 10f;
			default:
				return (float)level - 2f;
			}
		}
	}
}
