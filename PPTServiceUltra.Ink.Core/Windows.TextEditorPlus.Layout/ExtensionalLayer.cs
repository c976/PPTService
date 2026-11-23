using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Windows;
using System.Windows.Media;

namespace PPTServiceUltra.Ink.Core.Windows.TextEditorPlus.Layout
{
	internal class ExtensionalLayer : Layer
	{
		private readonly List<UIElement> _visuals = new List<UIElement>();

		protected override int VisualChildrenCount => _visuals.Count;

		static ExtensionalLayer()
		{
			UIElement.IsHitTestVisibleProperty.OverrideMetadata(typeof(ExtensionalLayer), new UIPropertyMetadata(true));
		}

		protected override Visual GetVisualChild(int index)
		{
			return _visuals[index];
		}

		public ExtensionalLayer()
			: base(KnownLayer.Extensional)
		{
		}

		public void LayoutExtensionalVisuals( List<ValueTuple<UIElement, Rect>> visuals)
		{
			List<UIElement> views = visuals.Select(( ValueTuple<UIElement, Rect> tuple) => tuple.Item1).ToList();
			foreach (UIElement item3 in _visuals.Where((UIElement element) => !views.Contains(element)).ToList())
			{
				RemoveElement(item3);
			}
			foreach (var visual in visuals)
			{
				UIElement item = visual.Item1;
				Rect item2 = visual.Item2;
				if (!_visuals.Contains(item))
				{
					AddElement(item);
				}
				item.Arrange(item2);
			}
		}

		private void AddElement(UIElement view)
		{
			AddVisualChild(view);
			_visuals.Add(view);
		}

		private void RemoveElement(UIElement element)
		{
			RemoveVisualChild(element);
			_visuals.Remove(element);
		}
	}
}
