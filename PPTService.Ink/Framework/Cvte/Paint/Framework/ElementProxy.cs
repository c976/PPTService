using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows;
using System.Windows.Documents;
using System.Windows.Media;
using Cvte.Paint.Framework.Level;

namespace Cvte.Paint.Framework
{
	public class ElementProxy : Adorner
	{
		private readonly Lazy<AdornerLayer> _layer;

		private bool _isAttached;

		private readonly VisualBrush[] _elementBrushes;

		private readonly Dictionary<Element, Rect> _elementOriginBoundsDictionary;

		private readonly TranslateTransform _translateTransform;

		public Vector Offset
		{
			get
			{
				return new Vector(_translateTransform.X, _translateTransform.Y);
			}
			set
			{
				_translateTransform.X = value.X;
				_translateTransform.Y = value.Y;
			}
		}

		public ElementProxy(Slide slide, IEnumerable<Element> elements)
			: base(slide)
		{
			ElementProxy elementProxy = this;
			_layer = new Lazy<AdornerLayer>(() => AdornerLayer.GetAdornerLayer(Board.GetRenderSource(slide)), false);
			_elementBrushes = slide.GetLevelSortedElements().Select((Element x) =>
			{
				elementProxy._elementOriginBoundsDictionary[x] = x.Bounds;
				return new VisualBrush(x)
				{
					ViewboxUnits = BrushMappingMode.Absolute
				};
			}).ToArray();
			_translateTransform = new TranslateTransform();
			Opacity = 0.75;
		}

		public ElementProxy(Slide slide, Dictionary<Element, Rect> elementOriginBoundsDictionary)
			: base(slide)
		{
			_layer = new Lazy<AdornerLayer>(() => AdornerLayer.GetAdornerLayer(Board.GetRenderSource(slide)), false);
			_elementBrushes = (from pair in elementOriginBoundsDictionary.OrderBy((KeyValuePair<Element, Rect> x) => x.Key, slide.LevelStorage)
				select new VisualBrush(pair.Key)
				{
					ViewboxUnits = BrushMappingMode.Absolute
				}).ToArray();
			_elementOriginBoundsDictionary = elementOriginBoundsDictionary;
			_translateTransform = new TranslateTransform();
			Opacity = 0.75;
			IsHitTestVisible = false;
		}

		public void Show()
		{
			if (!_isAttached)
			{
				_layer.Value.Add(this);
				_isAttached = true;
			}
		}

		public void Hide()
		{
			if (_isAttached)
			{
				_layer.Value.Remove(this);
				_isAttached = false;
			}
		}

		public override GeneralTransform GetDesiredTransform(GeneralTransform transform)
		{
			return new TransformGroup
			{
				Children = 
				{
					(Transform)_translateTransform,
					transform as Transform
				}
			};
		}

		protected override void OnRender(DrawingContext dc)
		{
			VisualBrush[] elementBrushes = _elementBrushes;
			foreach (VisualBrush visualBrush in elementBrushes)
			{
				Element element = (Element)visualBrush.Visual;
				Rect rect = _elementOriginBoundsDictionary[element];
				Rect descendantBounds = VisualTreeHelper.GetDescendantBounds(element);
				descendantBounds.Offset(rect.Location - default(Point));
				descendantBounds.Offset(0.0 - element.ElementEffect.ShadowBlurRadius, 0.0);
				descendantBounds.Offset(0.0, 0.0 - element.ElementEffect.ShadowBlurRadius);
				descendantBounds.Width += element.ElementEffect.ShadowBlurRadius * 2.0;
				descendantBounds.Height += element.ElementEffect.ShadowBlurRadius * 2.0;
				visualBrush.Viewbox = descendantBounds;
				dc.DrawRectangle(visualBrush, null, descendantBounds);
			}
		}
	}
}
