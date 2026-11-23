using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using PPTServiceUltra.Ink.Core.Windows.TextEditorPlus.Core;
using PPTServiceUltra.Ink.Core.Windows.TextEditorPlus.Document;
using PPTServiceUltra.Ink.Core.Windows.TextEditorPlus.Render;
using PPTServiceUltra.Ink.Core.Windows.TextEditorPlus.Utils;

namespace PPTServiceUltra.Ink.Core.Windows.TextEditorPlus.Layout
{
	internal class TextView : FrameworkElement, ILazyInitialize
	{
		private readonly RenderManager _renderManager;

		private TextLayerBase _textLayer;

		private readonly UIElementCollection _layers;

		private ExtensionalLayer _extensionalLayer;

		private MarkerLayer _markerLayer;

		private bool _isEmpty = true;

		protected override int VisualChildrenCount => _layers.Count;

		protected override Visual GetVisualChild(int index)
		{
			return _layers[index];
		}

		internal TextView(RenderManager renderManager)
		{
			_renderManager = renderManager;
			FocusVisualStyle = null;
			_layers = new UIElementCollection(this, this);
			_renderManager.RenderCompleted += RenderManger_OnRenderCompleted;
		}

		public void InitializeUntilEditable()
		{
			_isEmpty = false;
		}

		public void InitializeUntilNotEmpty()
		{
			_markerLayer = new MarkerLayer();
			_renderManager.UpdateMarkerOpacity += (object sender, Dictionary<int, double> renderInfo) =>
			{
				_markerLayer.UpdateMarkerOpacity(renderInfo);
			};
			InsertLayer(_markerLayer);
			_extensionalLayer = new ExtensionalLayer();
			InsertLayer(_extensionalLayer);
			_isEmpty = false;
		}

		private void RenderManger_OnRenderCompleted(object sender, List<ParagraphLayoutData> obj)
		{
			if (!_isEmpty)
			{
				_textLayer.LayoutLineVisuals(obj, _renderManager.DocumentManager.EditorWidth);
				_markerLayer.LayoutMarkerVisuals(obj, (_renderManager.DocumentManager.ArrangingType == ArrangingType.Vertical) ? _renderManager.DocumentManager.EditorWidth : 0.0);
				LayoutExtensionalVisual(obj);
			}
		}

		private void LayoutExtensionalVisual(List<ParagraphLayoutData> data)
		{
			List<ValueTuple<UIElement, Rect>> extensionCharVisuals = new List<ValueTuple<UIElement, Rect>>();
			foreach (ParagraphLayoutData datum in data)
			{
				foreach (LineVisualBase line in datum.Lines)
				{
					(from visual in line.LineRenderData.CharVisuals.OfType<ExtensionCharVisual>()
						where visual.IsHitTestVisible
						select visual).ToList().ForEach((ExtensionCharVisual visual) =>
					{
						Rect item = line.TranslateRect(visual.Bounds, this);
						extensionCharVisuals.Add(new ValueTuple<UIElement, Rect>(visual.View, item));
					});
				}
			}
			_extensionalLayer.LayoutExtensionalVisuals(extensionCharVisuals);
		}

		internal void SwitchTextLayer(ArrangingType arrangingType)
		{
			RemoveLayer(_textLayer);
			switch (arrangingType)
			{
			case ArrangingType.Horizontal:
				_textLayer = new HorizontalTextLayer();
				break;
			case ArrangingType.Vertical:
				_textLayer = new VerticalTextLayer();
				break;
			case ArrangingType.Mongolian:
				_textLayer = new MongolianTextLayer();
				break;
			default:
				throw new ArgumentOutOfRangeException("arrangingType", arrangingType, null);
			}
			_renderManager.CleanParagraphs();
			InsertLayer(_textLayer);
		}

		internal void InsertLayer(Layer layer)
		{
			if (layer == null)
			{
				throw new ArgumentNullException("layer");
			}
			for (int i = 0; i < _layers.Count; i++)
			{
				KnownLayer knownLayer = ((Layer)_layers[i]).KnownLayer;
				if (knownLayer == layer.KnownLayer)
				{
					throw new ArgumentException("layer is already exist");
				}
				if (knownLayer > layer.KnownLayer)
				{
					_layers.Insert(i, layer);
					return;
				}
			}
			_layers.Add(layer);
		}

		internal void RemoveLayer(Layer layer)
		{
			if (_layers.Contains(layer))
			{
				_layers.Remove(layer);
				RemoveVisualChild(layer);
				RemoveLogicalChild(layer);
			}
		}

		internal void UpdateCacheMode(CacheMode cacheMode)
		{
			CacheMode = cacheMode;
			_textLayer.UpdateCacheMode(cacheMode);
		}
	}
}
