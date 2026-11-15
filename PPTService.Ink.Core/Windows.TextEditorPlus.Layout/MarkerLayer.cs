using System.Collections.Generic;
using System.Linq;
using System.Windows;
using System.Windows.Media;
using PPTService.Ink.Core.Windows.TextEditorPlus.Render;

namespace PPTService.Ink.Core.Windows.TextEditorPlus.Layout
{
	internal class MarkerLayer : Layer
	{
		private readonly Dictionary<int, UIElement> _visuals = new Dictionary<int, UIElement>();

		protected override int VisualChildrenCount => _visuals.Count;

		protected override Visual GetVisualChild(int index)
		{
			return _visuals.Values.ToList()[index];
		}

		internal MarkerLayer()
			: base(KnownLayer.Marker)
		{
		}

		public void LayoutMarkerVisuals(List<ParagraphLayoutData> paragraphLayoutDatas, double editorWidth)
		{
			_visuals.Values.ToList().ForEach(RemoveVisualChild);
			_visuals.Clear();
			for (int i = 0; i < paragraphLayoutDatas.Count; i++)
			{
				if (paragraphLayoutDatas[i].MarkerVisual != null)
				{
					UIElement charView = paragraphLayoutDatas[i].MarkerVisual.CharView;
					AddVisualChild(charView);
					Rect bounds = paragraphLayoutDatas[i].MarkerVisual.Bounds;
					bounds.X += editorWidth;
					charView.Arrange(bounds);
					_visuals.Add(i, charView);
				}
			}
		}

		public void UpdateMarkerOpacity(Dictionary<int, double> opacityList)
		{
			if (!_visuals.Any())
			{
				return;
			}
			foreach (KeyValuePair<int, double> opacity in opacityList)
			{
				_visuals[opacity.Key].Opacity = opacity.Value;
			}
			InvalidateVisual();
		}
	}
}
