using System;
using System.Collections.Generic;
using System.Windows;
using System.Windows.Media;

namespace PPTService.Ink.Core.Windows.TextEditorPlus.Render
{
	public class DrawingVisualContainer : UIElement
	{
		private readonly List<DrawingVisual> _visuals = new List<DrawingVisual>();

		protected override int VisualChildrenCount => _visuals.Count;

		public void AddChild(DrawingVisual visual)
		{
			if (visual == null)
			{
				throw new ArgumentNullException("visual");
			}
			_visuals.Add(visual);
			AddVisualChild(visual);
		}

		public void AddChildren(List<DrawingVisual> visuals)
		{
			if (visuals == null)
			{
				throw new ArgumentNullException("visuals");
			}
			visuals.ForEach(AddChild);
		}

		protected override Visual GetVisualChild(int index)
		{
			return _visuals[index];
		}

		protected override HitTestResult HitTestCore(PointHitTestParameters hitTestParameters)
		{
			return null;
		}

		protected override GeometryHitTestResult HitTestCore(GeometryHitTestParameters hitTestParameters)
		{
			return null;
		}

		internal DrawingVisualContainer Clone()
		{
			DrawingVisualContainer drawingVisualContainer = new DrawingVisualContainer();
			foreach (DrawingVisual visual in _visuals)
			{
				DrawingGroup drawing = VisualTreeHelper.GetDrawing(visual);
				DrawingVisual drawingVisual = new DrawingVisual();
				DrawingContext drawingContext = drawingVisual.RenderOpen();
				drawingContext.DrawDrawing(drawing);
				drawingContext.Close();
				drawingVisualContainer.AddChild(drawingVisual);
			}
			return drawingVisualContainer;
		}
	}
}
