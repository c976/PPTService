using System;
using System.Windows;
using System.Windows.Ink;
using System.Windows.Input;

namespace Cvte.Internal.Ink
{
	internal class StrokeNodeIterator
	{
		private bool _usePressure;

		private StrokeNodeOperations _operations;

		private StylusPointCollection _stylusPoints;

		internal int Count
		{
			get
			{
				if (_stylusPoints == null)
				{
					return 0;
				}
				return _stylusPoints.Count;
			}
		}

		internal StrokeNode this[int index] => this[index, (index == 0) ? (-1) : (index - 1)];

		internal StrokeNode this[int index, int previousIndex]
		{
			get
			{
				if (_stylusPoints == null || index < 0 || index >= _stylusPoints.Count || previousIndex < -1 || previousIndex >= index)
				{
					throw new IndexOutOfRangeException();
				}
				StylusPoint stylusPoint = _stylusPoints[index];
				StylusPoint stylusPoint2 = ((previousIndex == -1) ? default(StylusPoint) : _stylusPoints[previousIndex]);
				float pressure = 1f;
				float pressure2 = 1f;
				if (_usePressure)
				{
					pressure = GetNormalizedPressureFactor(stylusPoint.PressureFactor);
					pressure2 = GetNormalizedPressureFactor(stylusPoint2.PressureFactor);
				}
				StrokeNodeData nodeData = new StrokeNodeData((Point)stylusPoint, pressure);
				StrokeNodeData lastNodeData = StrokeNodeData.Empty;
				if (previousIndex != -1)
				{
					lastNodeData = new StrokeNodeData((Point)stylusPoint2, pressure2);
				}
				return new StrokeNode(_operations, previousIndex + 1, nodeData, lastNodeData, index == _stylusPoints.Count - 1);
			}
		}

		internal static StrokeNodeIterator GetIterator(Stroke stroke, DrawingAttributes drawingAttributes)
		{
			if (stroke == null)
			{
				throw new ArgumentNullException("stroke");
			}
			if (drawingAttributes == null)
			{
				throw new ArgumentNullException("drawingAttributes");
			}
			return GetIterator(drawingAttributes.FitToCurve ? stroke.GetBezierStylusPoints() : stroke.StylusPoints, drawingAttributes);
		}

		internal static StrokeNodeIterator GetIterator(StylusPointCollection stylusPoints, DrawingAttributes drawingAttributes)
		{
			if (stylusPoints == null)
			{
				throw new ArgumentNullException("stylusPoints");
			}
			if (drawingAttributes == null)
			{
				throw new ArgumentNullException("drawingAttributes");
			}
			StrokeNodeOperations operations = StrokeNodeOperations.CreateInstance(GetStylusShape(drawingAttributes));
			bool usePressure = !drawingAttributes.IgnorePressure;
			return new StrokeNodeIterator(stylusPoints, operations, usePressure);
		}

		internal static float GetNormalizedPressureFactor(float stylusPointPressureFactor)
		{
			return 1.5f * stylusPointPressureFactor + 0.25f;
		}

		internal StrokeNodeIterator(StylusShape nodeShape)
			: this(null, StrokeNodeOperations.CreateInstance(nodeShape), false)
		{
		}

		internal StrokeNodeIterator(DrawingAttributes drawingAttributes)
			: this(null, StrokeNodeOperations.CreateInstance((drawingAttributes == null) ? null : GetStylusShape(drawingAttributes)), !(drawingAttributes == null) && !drawingAttributes.IgnorePressure)
		{
		}

		public static StylusShape GetStylusShape(DrawingAttributes drawingAttribute)
		{
			StylusShape obj = ((drawingAttribute.StylusTip != StylusTip.Rectangle) ? ((StylusShape)new EllipseStylusShape(drawingAttribute.Width, drawingAttribute.Height)) : ((StylusShape)new RectangleStylusShape(drawingAttribute.Width, drawingAttribute.Height)));
			obj.Transform = drawingAttribute.StylusTipTransform;
			return obj;
		}

		internal StrokeNodeIterator(StylusPointCollection stylusPoints, StrokeNodeOperations operations, bool usePressure)
		{
			_stylusPoints = stylusPoints;
			if (operations == null)
			{
				throw new ArgumentNullException("operations");
			}
			_operations = operations;
			_usePressure = usePressure;
		}

		internal StrokeNodeIterator GetIteratorForNextSegment(StylusPointCollection stylusPoints)
		{
			if (stylusPoints == null)
			{
				throw new ArgumentNullException("stylusPoints");
			}
			if (_stylusPoints != null && _stylusPoints.Count > 0 && stylusPoints.Count > 0)
			{
				StylusPoint item = stylusPoints[0];
				StylusPoint stylusPoint = _stylusPoints[_stylusPoints.Count - 1];
				item.X = stylusPoint.X;
				item.Y = stylusPoint.Y;
				item.PressureFactor = stylusPoint.PressureFactor;
				stylusPoints.Insert(0, item);
			}
			return new StrokeNodeIterator(stylusPoints, _operations, _usePressure);
		}

		internal StrokeNodeIterator GetIteratorForNextSegment(Point[] points)
		{
			if (points == null)
			{
				throw new ArgumentNullException("points");
			}
			StylusPointCollection stylusPointCollection = new StylusPointCollection(points);
			if (_stylusPoints != null && _stylusPoints.Count > 0)
			{
				stylusPointCollection.Insert(0, _stylusPoints[_stylusPoints.Count - 1]);
			}
			return new StrokeNodeIterator(stylusPointCollection, _operations, _usePressure);
		}
	}
}
