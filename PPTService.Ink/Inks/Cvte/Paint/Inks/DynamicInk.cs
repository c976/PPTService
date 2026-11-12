using System;
using System.Windows;
using System.Windows.Documents;
using System.Windows.Ink;
using System.Windows.Input;
using System.Windows.Media;
using Cvte.Paint.Framework;
using Cvte.Paint.Settings;

namespace Cvte.Paint.Inks
{
	public abstract class DynamicInk : Adorner
	{
		private readonly VisualCollection _visualCollection;

		[Obsolete("这个值没地方使用，请不要设置")]
		protected Pen DrawingPen { get; set; }

		protected StylusPointCollection Points { get; }

		public DateTime CreatedTime { get; private set; }

		public DrawingAttributes DrawingAttributes { get; }

		public bool IsBezierInterpolationEnabled { get; set; }

		public string DeviceId { get; set; }

		public string InkId { get; set; }

		public Ink Ink { get; set; }

		[Obsolete("这个属性耦合 EasiNet 业务，考虑删除")]
		public bool IsLocalInk { get; set; } = true;

		[Obsolete("这个属性耦合 EasiNet 业务，考虑删除")]
		public string ClientID { get; set; }

		internal bool NeedCollectStrokeDetails { get; set; }

		internal StrokePointCollection StrokePointDetailsList { get; } = new StrokePointCollection();

		protected sealed override int VisualChildrenCount => _visualCollection.Count;

		public bool IsPostOptimizationEnabled { get; set; }

		protected DynamicInk(UIElement adornedElement, InkBoardSettings state)
			: this(adornedElement, CreateDrawingAttributes(state))
		{
		}

		protected DynamicInk(UIElement adornedElement, DrawingAttributes drawingAttributes)
			: base(adornedElement)
		{
			CreatedTime = DateTime.Now;
			Points = new StylusPointCollection();
			_visualCollection = new VisualCollection(this);
			DrawingAttributes = drawingAttributes;
			Brush brush = new SolidColorBrush(DrawingAttributes.Color);
			brush.Freeze();
			DrawingPen = new Pen(brush, DrawingAttributes.Width)
			{
				StartLineCap = PenLineCap.Round,
				EndLineCap = PenLineCap.Round
			};
			DrawingPen.Freeze();
		}

		protected virtual void OnAppendPoint(StylusPoint point, bool isLastPoint)
		{
		}

		protected void AddVisual(Visual visual)
		{
			_visualCollection.Add(visual);
		}

		protected void RemoveVisual(Visual visual)
		{
			if (_visualCollection.Contains(visual))
			{
				_visualCollection.Remove(visual);
			}
		}

		protected void ClearVisual()
		{
			_visualCollection.Clear();
		}

		public virtual StylusPointCollection GetPoints()
		{
			return Points;
		}

		public void AppendPoints(StylusPoint[] points)
		{
			for (int i = 0; i < points.Length; i++)
			{
				StylusPoint point = points[i];
				CollectStrokePointDetails(point);
				OnAppendPoint(new StylusPoint(point.X, point.Y, point.PressureFactor), i == points.Length - 1);
			}
		}

		public void AppendInkPoint(StylusPoint point)
		{
			CollectStrokePointDetails(point);
			OnAppendPoint(point, true);
		}

		public abstract Element AsStatic();

		private void CollectStrokePointDetails(StylusPoint point)
		{
			if (NeedCollectStrokeDetails)
			{
				StrokePointDetailsList.Add(point);
			}
		}

		protected sealed override Size ArrangeOverride(Size finalSize)
		{
			return RenderSize;
		}

		protected sealed override Size MeasureOverride(Size availableSize)
		{
			return RenderSize;
		}

		protected sealed override Visual GetVisualChild(int index)
		{
			return _visualCollection[index];
		}

		protected static DrawingAttributes CreateDrawingAttributes(InkBoardSettings state)
		{
			return new DrawingAttributes
			{
				Color = state.InkColor,
				Width = state.InkThickness * state.InkScale,
				Height = state.InkThickness * state.InkScale
			};
		}
	}
}
