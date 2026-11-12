using System;
using System.Collections.Generic;
using System.Windows;
using System.Windows.Ink;
using System.Windows.Input;
using Cvte.Diagnostics.Annotations;
using Cvte.Internal.Ink;

namespace Cvte.Paint.Inks.PointToGeometry
{
	public class InkStrokeNodeIterator
	{
		private readonly List<StrokeNodeData> _cacheStylusPointList;

		private readonly StylusPointCollection _stylusPoints;

		private readonly StrokeNodeOperations _strokeNodeOperations;

		private IPressureFactorToScaleConverter PressureFactorToScaleConverter { get; }

		[CanBeNull]
		private InkStrokeRenderSettings InkStrokeRenderSettings { get; }

		public int Count
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
				return new StrokeNode(_strokeNodeOperations, previousIndex + 1, GetStrokeNodeData(index), (previousIndex == -1) ? StrokeNodeData.Empty : GetStrokeNodeData(previousIndex), index == _stylusPoints.Count - 1);
			}
		}

		private InkStrokeNodeIterator(double inkThickness)
			: this()
		{
			_strokeNodeOperations = StrokeNodeOperations.CreateInstance(GetStylusShape(inkThickness));
		}

		public InkStrokeNodeIterator(StylusPointCollection stylusPoints, double inkThickness)
			: this()
		{
			_stylusPoints = stylusPoints;
			int count = _stylusPoints.Count;
			_cacheStylusPointList = new List<StrokeNodeData>(Math.Max(count, 8));
			_strokeNodeOperations = StrokeNodeOperations.CreateInstance(GetStylusShape(inkThickness));
		}

		public InkStrokeNodeIterator(StylusPointCollection stylusPoints, DrawingAttributes drawingAttributes, [CanBeNull] InkStrokeRenderSettings settings = null)
			: this(settings)
		{
			_stylusPoints = stylusPoints;
			int count = _stylusPoints.Count;
			_cacheStylusPointList = new List<StrokeNodeData>(Math.Max(count, 8));
			_strokeNodeOperations = StrokeNodeOperations.CreateInstance(GetStylusShape(drawingAttributes));
		}

		private InkStrokeNodeIterator(InkStrokeRenderSettings settings = null)
		{
			PressureFactorToScaleConverter = settings?.PressureFactorToScaleConverter ?? new DefaultPressureFactorToScaleConverter();
		}

		private InkStrokeNodeIterator(StylusPointCollection stylusPoints, StrokeNodeOperations strokeNodeOperations)
		{
			_stylusPoints = stylusPoints;
			_strokeNodeOperations = strokeNodeOperations;
			int count = _stylusPoints.Count;
			_cacheStylusPointList = new List<StrokeNodeData>(Math.Max(count, 8));
		}

		public static InkStrokeNodeIterator GetIterator(double inkThickness)
		{
			return new InkStrokeNodeIterator(inkThickness);
		}

		public static InkStrokeNodeIterator GetIterator(StylusPointCollection stylusPoints, double inkThickness)
		{
			return new InkStrokeNodeIterator(stylusPoints, inkThickness);
		}

		public InkStrokeNodeIterator GetIteratorForNextSegment(StylusPointCollection stylusPoints)
		{
			if (_stylusPoints != null && _stylusPoints.Count > 0 && stylusPoints.Count > 0)
			{
				StylusPoint item = stylusPoints[0];
				StylusPoint stylusPoint = _stylusPoints[_stylusPoints.Count - 1];
				item.X = stylusPoint.X;
				item.Y = stylusPoint.Y;
				item.PressureFactor = stylusPoint.PressureFactor;
				stylusPoints.Insert(0, item);
			}
			return new InkStrokeNodeIterator(stylusPoints, _strokeNodeOperations);
		}

		private StrokeNodeData GetStrokeNodeData(int n)
		{
			if (n < _cacheStylusPointList.Count)
			{
				return _cacheStylusPointList[n];
			}
			StylusPoint stylusPoint = _stylusPoints[n];
			StrokeNodeData strokeNodeData = new StrokeNodeData((Point)stylusPoint, GetNormalizedPressureFactor(stylusPoint.PressureFactor));
			if (n == _cacheStylusPointList.Count)
			{
				_cacheStylusPointList.Add(strokeNodeData);
			}
			return strokeNodeData;
		}

		private float GetNormalizedPressureFactor(float stylusPointPressureFactor)
		{
			return PressureFactorToScaleConverter.PressureFactorToScalePressureFactor(stylusPointPressureFactor);
		}

		private static Cvte.Internal.Ink.StylusShape GetStylusShape(double inkThickness)
		{
			return new Cvte.Internal.Ink.EllipseStylusShape(inkThickness, inkThickness);
		}

		private static Cvte.Internal.Ink.StylusShape GetStylusShape(DrawingAttributes drawingAttribute)
		{
			Cvte.Internal.Ink.StylusShape obj = ((drawingAttribute.StylusTip != StylusTip.Rectangle) ? ((Cvte.Internal.Ink.StylusShape)new Cvte.Internal.Ink.EllipseStylusShape(drawingAttribute.Width, drawingAttribute.Height)) : ((Cvte.Internal.Ink.StylusShape)new Cvte.Internal.Ink.RectangleStylusShape(drawingAttribute.Width, drawingAttribute.Height)));
			obj.Transform = drawingAttribute.StylusTipTransform;
			return obj;
		}
	}
}
