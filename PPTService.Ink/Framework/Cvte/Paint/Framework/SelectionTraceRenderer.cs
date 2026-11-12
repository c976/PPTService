using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows;
using System.Windows.Documents;
using System.Windows.Media;
using Cvte.Diagnostics.Annotations;
using Cvte.Paint.Framework.Settings;

namespace Cvte.Paint.Framework
{
	public class SelectionTraceRenderer
	{
		private class TraceAdorner : Adorner
		{
			private Rect _rect;

			private readonly PolyLineGeometryBuilder _geometryBuilder = new PolyLineGeometryBuilder(false);

			private readonly SelectionStyleSettings _settings;

			public TraceAdorner([NotNull] UIElement adornedElement, SelectionStyleSettings settings)
				: base(adornedElement)
			{
				_settings = settings;
			}

			protected override void OnRender(DrawingContext dc)
			{
				if (!_rect.IsEmpty)
				{
					dc.DrawSnappedRectangle(_settings.RectFill, _settings.RectPen, _rect);
				}
				if (!_geometryBuilder.IsEmpty)
				{
					dc.DrawGeometry(null, _settings.PathPen, _geometryBuilder.Geometry);
				}
			}

			public void RenderRect(Rect rect)
			{
				_rect = rect;
				_geometryBuilder.Reset();
				InvalidateVisual();
			}

			public void AppendPathNode(params Point[] points)
			{
				AppendPathNode((IEnumerable<Point>)points);
			}

			public void AppendPathNode(IEnumerable<Point> points)
			{
				_rect = Rect.Empty;
				foreach (Point point in points)
				{
					_geometryBuilder.Add(point);
				}
				InvalidateVisual();
			}

			public void Clear()
			{
				_rect = Rect.Empty;
				_geometryBuilder.Reset();
				InvalidateVisual();
			}
		}

		[CanBeNull]
		private readonly AdornerLayer _layer;

		[NotNull]
		private readonly TraceAdorner _adorner;

		[NotNull]
		private readonly Func<Point, Point> _translatePoint;

		private Point _rectStart;

		private bool _isRendering;

		public SelectionTraceRenderer([NotNull] Board board)
		{
			if (board == null)
			{
				throw new ArgumentNullException("board");
			}
			_translatePoint = (Point point) => point;
			UIElement renderSource = Board.GetRenderSource(board);
			_adorner = new TraceAdorner(renderSource, board.GetSettings<SelectionStyleSettings>());
			_layer = AdornerLayer.GetAdornerLayer(renderSource);
		}

		public SelectionTraceRenderer([NotNull] Slide slide)
		{
			if (slide == null)
			{
				throw new ArgumentNullException("slide");
			}
			Board board = (Board)slide.Parent;
			_translatePoint = (Point slidePoint) =>
			{
				if (board == null)
				{
					board = (Board)slide.Parent;
				}
				return (board != null) ? slide.TranslatePoint(slidePoint, board) : slidePoint;
			};
			UIElement renderSource = Board.GetRenderSource(slide);
			_adorner = new TraceAdorner(renderSource, slide.GetSettings<SelectionStyleSettings>());
			_layer = AdornerLayer.GetAdornerLayer(renderSource);
		}

		public void Start(Point point)
		{
			Clear();
			_rectStart = _translatePoint(point);
			_adorner.AppendPathNode(_rectStart);
		}

		public void SetRectEnd(Point point)
		{
			Prepare();
			Point point2 = _translatePoint(point);
			_adorner.RenderRect(new Rect(_rectStart, point2));
		}

		public void AddTracePoint(Point point)
		{
			Prepare();
			_adorner.AppendPathNode(_translatePoint(point));
		}

		public void AddTracePoints(IEnumerable<Point> points)
		{
			Prepare();
			_adorner.AppendPathNode(points.Select((Point x) => _translatePoint(x)));
		}

		public void Clear()
		{
			if (_isRendering)
			{
				_isRendering = false;
				_adorner.Clear();
				_layer?.Remove(_adorner);
			}
		}

		private void Prepare()
		{
			if (!_isRendering)
			{
				_isRendering = true;
				_layer?.Add(_adorner);
			}
		}
	}
}
