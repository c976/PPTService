using System;
using System.Collections.Generic;
using System.Windows;
using System.Windows.Media;

namespace PPTServiceUltra.Ink.Core.Windows.TextEditorPlus.Render
{
	public class UIElementContainer : UIElement
	{
		private readonly List<DrawingVisualContainer> _elements = new List<DrawingVisualContainer>();

		public List<DrawingVisualContainer> Elements => _elements;

		protected override int VisualChildrenCount => _elements.Count;

		public UIElementContainer(bool needCached = false)
		{
			if (needCached)
			{
				CacheMode = new BitmapCache
				{
					SnapsToDevicePixels = true
				};
			}
		}

		public void AddChild(DrawingVisualContainer element)
		{
			if (element == null)
			{
				throw new ArgumentNullException("element");
			}
			_elements.Add(element);
			AddVisualChild(element);
		}

		protected override void ArrangeCore(Rect finalRect)
		{
			foreach (DrawingVisualContainer element in _elements)
			{
				element.Arrange(new Rect(default(Point), default(Size)));
			}
			base.ArrangeCore(finalRect);
		}

		public void AddChildren(List<DrawingVisualContainer> elements)
		{
			if (elements == null)
			{
				throw new ArgumentNullException("elements");
			}
			elements.ForEach(AddChild);
		}

		protected override Visual GetVisualChild(int index)
		{
			return _elements[index];
		}

		protected override HitTestResult HitTestCore(PointHitTestParameters hitTestParameters)
		{
			return null;
		}

		protected override GeometryHitTestResult HitTestCore(GeometryHitTestParameters hitTestParameters)
		{
			return null;
		}
	}
}
