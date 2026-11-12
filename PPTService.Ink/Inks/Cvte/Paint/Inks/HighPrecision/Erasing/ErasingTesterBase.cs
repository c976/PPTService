using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
using System.Windows;
using System.Windows.Ink;
using System.Windows.Input;
using System.Windows.Media;
using Cvte.Collections;
using Cvte.Internal.Ink;

namespace Cvte.Paint.Inks.HighPrecision.Erasing
{
	internal abstract class ErasingTesterBase : IErasingTester
	{
		public ValueTuple<bool, IEnumerable<Stroke>> EraseTest(Stroke stroke)
		{
			if (!CheckCanEraseTest())
			{
				return default(ValueTuple<bool, IEnumerable<Stroke>>);
			}
			if (!stroke.GetBounds().IntersectsWith(GetCurrentBounds()))
			{
				return default(ValueTuple<bool, IEnumerable<Stroke>>);
			}
			StrokeNodeIterator iterator = StrokeNodeIterator.GetIterator(stroke, stroke.DrawingAttributes);
			List<Stroke> list = new List<Stroke>();
			List<StylusPoint> list2 = null;
			bool flag = false;
			int count = iterator.Count;
			for (int i = 0; i < count; i++)
			{
				Rect bounds = iterator[i].GetBounds();
				if (CheckErased(bounds))
				{
					if (list2 != null)
					{
						list.Add(new Stroke(new StylusPointCollection(list2), stroke.DrawingAttributes));
						list2 = null;
					}
					flag = true;
				}
				else
				{
					if (list2 == null)
					{
						list2 = new List<StylusPoint>();
					}
					list2.Add(stroke.StylusPoints[i]);
				}
			}
			if (flag)
			{
				if (list2 != null)
				{
					DrawingAttributes drawingAttributes = stroke.DrawingAttributes.Clone();
					list.Add(new Stroke(new StylusPointCollection(list2), drawingAttributes));
					list2 = null;
				}
				return new ValueTuple<bool, IEnumerable<Stroke>>(true, list);
			}
			return new ValueTuple<bool, IEnumerable<Stroke>>(false, EmptyArray<Stroke>.Value);
		}

		public virtual ValueTuple<bool, Geometry> EraseTest(Geometry elementGeometry, Transform slideToElementTransfrom)
		{
			Geometry geometry = GetEraserGeometry().Clone();
			geometry.Transform = slideToElementTransfrom;
			PathGeometry pathGeometry = Geometry.Combine(geometry, elementGeometry, GeometryCombineMode.Intersect, Transform.Identity);
			if (pathGeometry.IsEmpty())
			{
				return new ValueTuple<bool, Geometry>(false, null);
			}
			return new ValueTuple<bool, Geometry>(true, pathGeometry);
		}

		protected virtual Geometry GetEraserGeometry()
		{
			return Geometry.Empty;
		}

		protected virtual bool CheckErased(Rect strokeNodeBounds)
		{
			if (!CheckErasedPoint(strokeNodeBounds.TopLeft) && !CheckErasedPoint(strokeNodeBounds.TopRight) && !CheckErasedPoint(strokeNodeBounds.BottomLeft))
			{
				return CheckErasedPoint(strokeNodeBounds.BottomRight);
			}
			return true;
		}

		protected abstract bool CheckErasedPoint(Point point);

		protected abstract Rect GetCurrentBounds();

		protected virtual bool CheckCanEraseTest()
		{
			return true;
		}

		public abstract void Move([In]ref EraserTouchEventArgs eraserTouchEventArgs);
	}
}
